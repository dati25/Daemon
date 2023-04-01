using Daemon.Models;
using System.IO.Compression;

namespace Daemon.Backup.BackupTypes;

public class FullBackup : BackupService
{
    public Config Config { get; set; }
    public List<string> destPaths { get; set; }

    public FullBackup(Config config)
    {
        Config = config;
        destPaths = config.Destinations!.Select(x => Path.Combine(x.Path, "FooBakCup", $"config_{config.Id}")).ToList();
    }

    public void Execute()
    {
        foreach (string dest in destPaths)
        {
            string destPath = Path.Combine(dest, "backup_" + GetBackupNumber(dest, Config));
            Directory.CreateDirectory(destPath);

            DeleteBackup(Path.Combine(dest, "backup_" + (GetBackupNumber(dest, Config) - Config.Retention - 1)));

            Config.Sources!.ForEach(source => fs.Copy(source.Path, destPath));

            if (Config.Compress == true)
            {
                ZipFile.CreateFromDirectory(destPath, destPath + ".zip");
                Directory.Delete(destPath, true);
            }
        }
    }
}