using Daemon.Services;
using Daemon.Client;
using Daemon.Models;
using System.IO.Compression;

namespace Daemon
{
    public class Backup
    {
        public Config Config { get; set; }
        public List<string> destPaths { get; set; }

        private FileService fs = new();

        public Backup(Config config)
        {
            Config = config;
            destPaths = config.Destinations!.Select(x => Path.Combine(x.Path, "FooBakCup", $"config_{config.Id}")).ToList();
        }

        public void Execute(bool snapshot, bool check)
        {
            if (snapshot)
            {
                SnapshotService s = new();
                Config.Sources!.ForEach(source => s.AddToSnapshot(Config, source.Path));
            }

            foreach (string dest in destPaths)
            {
                string destPath = Path.Combine(dest, "backup_" + GetBackupNumber(dest, Config));
                Directory.CreateDirectory(destPath);

                DeleteBackup(Path.Combine(dest, "backup_" + (GetBackupNumber(dest, Config) - Config.Retention - 1)));

                if (check)
                {
                    SnapshotService s = new();
                    SettingsConfig sc = new();

                    List<Snapshot> snaps = s.ReadSnapshot(Path.Combine(sc.SNAPSHOTSPATH, $"config_{Config.Id}.txt"));

                    Config.Sources!.ForEach(source => fs.Copy(source.Path, destPath, true, snaps));
                }
                else
                    Config.Sources!.ForEach(source => fs.Copy(source.Path, destPath));

                if (Config.Compress == true)
                {
                    ZipFile.CreateFromDirectory(destPath, destPath + ".zip");
                    Directory.Delete(destPath, true);
                }
            }
        }


        public int GetBackupNumber(string path, Config config)
        {
            DirectoryInfo d = new DirectoryInfo(path);

            int fileCount = 1;
            int dirCount = 1;

            try
            {
                FileInfo[] items = d.GetFiles();
                int backups = items.Length + 1;

                Array.Sort(items, delegate (FileInfo f1, FileInfo f2)
                {
                    return f1.LastWriteTime.CompareTo(f2.LastWriteTime);
                });

                string name = items[items.Length - 1].Name;
                string[] split = name.Split('_');
                string[] split2 = split[1].Split('.');
                int num = int.Parse(split2[0]);

                fileCount += num;
            }
            catch { }

            try
            {
                DirectoryInfo[] items = d.GetDirectories();
                int backups = items.Length + 1;

                Array.Sort(items, delegate (DirectoryInfo d1, DirectoryInfo d2)
                {
                    return d1.LastWriteTime.CompareTo(d2.LastWriteTime);
                });

                string name = items[items.Length - 1].Name;
                string[] split = name.Split('_');
                int num = int.Parse(split[1]);

                dirCount += num;
            }
            catch { }

            if (fileCount > dirCount)
                return fileCount;
            else
                return dirCount;
        }

        public void DeleteBackup(string path)
        {
            if (File.Exists(path + ".zip"))
                File.Delete(path + ".zip");

            if (Directory.Exists(path))
                Directory.Delete(path, true);
        }
    }
}
