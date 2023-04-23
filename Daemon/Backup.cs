using Daemon.Services;
using Daemon.Models;
using System.IO.Compression;

namespace Daemon;
public class Backup
{
    private Config Config { get; }
    private List<string> DestPaths { get; }

    private readonly FileService _fs = new();
    private readonly SettingsConfig _sc = new();
    private readonly SnapshotService _s = new();

    public Backup(Config config)
    {
        Config = config;
        DestPaths = config.Destinations!.Select(x => Path.Combine(x.Path!, "FooBakCup", $"config_{config.Id}")).ToList();
    }

    public void Execute(bool create = false, bool update = false)
    {
        if (Config.ExpirationDate != null && DateTime.Parse(Config.ExpirationDate) < DateTime.Now) return;
        if (Config.Status != true) return;
        if (Config.Sources == null || Config.Destinations == null) return;

        foreach (var dest in DestPaths)
        {
            var backupNumber = GetBackupNumber(dest);
            var destPath = Path.Combine(dest, "backup_" + backupNumber);
            Directory.CreateDirectory(destPath);

            var retentionBackupNumber = backupNumber - Config.Retention;
            if (retentionBackupNumber >= 0)
            {
                var retentionBackupPath = Path.Combine(dest, "backup_" + retentionBackupNumber);
                DeleteBackup(retentionBackupPath);
            }

            var snapshotPath = Path.Combine(_sc.SnapshotsPath, $"config_{Config.Id}.txt");
            if (File.Exists(snapshotPath))
            {
                var snaps = _s.ReadSnapshot(snapshotPath);
                Parallel.ForEach(Config.Sources, source =>
                {
                    var sourcePath = source.Path!;
                    _fs.Copy(sourcePath, destPath, true, snaps);
                });
            }
            else
            {
                Parallel.ForEach(Config.Sources, source =>
                {
                    var sourcePath = source.Path!;
                    _fs.Copy(sourcePath, destPath);
                });
            }

            if (Config.Compress == true)
            {
                ZipFile.CreateFromDirectory(destPath, destPath + ".zip");
                Directory.Delete(destPath, true);
            }
        }

        if (!create) return;
        {
            var snapshotPath = Path.Combine(_sc.SnapshotsPath, $"config_{Config.Id}.txt");

            if (!Directory.Exists(_sc.SnapshotsPath))
                Directory.CreateDirectory(_sc.SnapshotsPath);

            if (!File.Exists(snapshotPath))
            {
                File.Create(snapshotPath).Close();
                Config.Sources!.ForEach(source => _s.AddToSnapshot(source.Path!, snapshotPath));
            }

            if (!update) return;
            {
                File.WriteAllText(snapshotPath, string.Empty);
                Config.Sources!.ForEach(source => _s.AddToSnapshot(source.Path!, snapshotPath));
            }
        }

    }


    private int GetBackupNumber(string path)
    {
        var d = new DirectoryInfo(path);

        var fileCount = 1;
        var dirCount = 1;

        try
        {
            var items = d.GetFiles();

            Array.Sort(items, (f1, f2) => f1.LastWriteTime.CompareTo(f2.LastWriteTime));

            var name = items[^1].Name;
            var split = name.Split('_');
            var split2 = split[1].Split('.');
            var num = int.Parse(split2[0]);

            fileCount += num;
        }
        catch
        {
            // ignored
        }

        try
        {
            var items = d.GetDirectories();

            Array.Sort(items, (d1, d2) => d1.LastWriteTime.CompareTo(d2.LastWriteTime));

            var name = items[^1].Name;
            var split = name.Split('_');
            var num = int.Parse(split[1]);

            dirCount += num;
        }
        catch
        {
            // ignored
        }

        return fileCount > dirCount ? fileCount : dirCount;
    }

    private void DeleteBackup(string path)
    {
        if (File.Exists(path + ".zip"))
            File.Delete(path + ".zip");
        else if (Directory.Exists(path))
            Directory.Delete(path, true);
    }
}