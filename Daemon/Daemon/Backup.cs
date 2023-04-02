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
        private SettingsConfig sc = new();
        private SnapshotService s = new();

        public Backup(Config config)
        {
            Config = config;
            destPaths = config.Destinations!.Select(x => Path.Combine(x.Path, "FooBakCup", $"config_{config.Id}")).ToList();
        }

        public void Execute(bool create = false, bool update = false)
        {
            foreach (string dest in destPaths)
            {
                string destPath = Path.Combine(dest, "backup_" + GetBackupNumber(dest, Config));
                Directory.CreateDirectory(destPath);

                DeleteBackup(Path.Combine(dest, "backup_" + (GetBackupNumber(dest, Config) - Config.Retention - 1)));

                string snapshotPath = Path.Combine(sc.SNAPSHOTSPATH, $"config_{Config.Id}.txt");
                if (File.Exists(snapshotPath))
                {
                    List<Snapshot> snaps = s.ReadSnapshot(snapshotPath);
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

            if (create)
            {
                string snapshotPath = Path.Combine(sc.SNAPSHOTSPATH, $"config_{Config.Id}.txt");

                if (!Directory.Exists(sc.SNAPSHOTSPATH))
                    Directory.CreateDirectory(sc.SNAPSHOTSPATH);

                if (!File.Exists(snapshotPath))
                {
                    File.Create(snapshotPath).Close();
                    Config.Sources!.ForEach(source => s.AddToSnapshot(Config, source.Path, snapshotPath));
                }

                if (update)
                {
                    File.WriteAllText(snapshotPath, string.Empty);
                    Config.Sources!.ForEach(source => s.AddToSnapshot(Config, source.Path, snapshotPath));
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
