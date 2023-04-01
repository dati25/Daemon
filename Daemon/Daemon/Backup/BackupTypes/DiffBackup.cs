using Daemon.Models;
using Daemon.Backup.Services.SnapshotServices;
using System.IO.Compression;

namespace Daemon.Backup.BackupTypes;

public class DiffBackup : BackupService
{
    private string defaultDirPath;
    private Config config { get; set; }

    public DiffBackup(Config config)
    {
        this.config = config;
        this.defaultDirPath = @$"{config.Destinations!.First().Path}\FooBakCup\config_{config.Id}";
    }

    public void Execute()
    {
        string dirPath = Path.Combine(defaultDirPath, "backup_" + GetBackupNumber(defaultDirPath, config));
        Directory.CreateDirectory(dirPath);

        DirectoryInfo dir = new DirectoryInfo(defaultDirPath);
        DirectoryInfo[] items = dir.GetDirectories();

        Array.Sort(items, delegate (DirectoryInfo d1, DirectoryInfo d2)
        {
            return d1.LastWriteTime.CompareTo(d2.LastWriteTime);
        });

        DirectoryInfo temp = items[0];
        string backupPath = temp.ToString().Substring(temp.ToString().LastIndexOf('\\') + 1);

        DirectoryInfo snapshotDir = new DirectoryInfo(Path.Combine(defaultDirPath, backupPath, ".snapshot"));

        if (!snapshotDir.Exists)
        {
            CreateSnapshot(dirPath);

            config.Sources!.ForEach(source => fs.CopyWithSnapshot(source.Path, dirPath));
        }

        else
        {
            DirectoryInfo dir2 = new DirectoryInfo(defaultDirPath);
            DirectoryInfo[] items2 = dir2.GetDirectories();

            Array.Sort(items2, delegate (DirectoryInfo d1, DirectoryInfo d2)
            {
                return d1.LastWriteTime.CompareTo(d2.LastWriteTime);
            });

            DirectoryInfo temp2 = items2[0];
            string backupPath2 = temp2.ToString().Substring(temp2.ToString().LastIndexOf('\\') + 1);

            SnapshotService sn = new SnapshotService();

            List<Snapshot> snapshot = sn.ReadSnapshot(Path.Combine(defaultDirPath, backupPath2, ".snapshot", "snapshot.txt"));

            config.Sources!.ForEach(source => fs.CopyWithSnapshotCheck(source.Path, dirPath, snapshot));
        }



        if (config.Compress == true)
        {
            ZipFile.CreateFromDirectory(dirPath, dirPath + ".zip");
            Directory.Delete(dirPath, true);
        }

        for (int i = 1; i < config.Destinations!.Count; i++)
        {
            string dest = $@"{config.Destinations[i].Path}\FooBakCup";

            DirectoryInfo d = new DirectoryInfo(dest);
            d.Create();

            d.GetDirectories().ToList().ForEach(item => item.Delete(true));
            d.GetFiles().ToList().ForEach(item => item.Delete());

            fs.Copy(defaultDirPath, dest);
        }
    }
}