using Daemon.Models;
using System.IO.Compression;

namespace Daemon.Backup.BackupTypes;

public class FullBackup : BackupService
{
    private string defaultDirPath;
    private Config config { get; set; }

    public FullBackup(Config config)
    {
        this.config = config;
        this.defaultDirPath = @$"{config.Destinations!.First().Path}\FooBakCup\config_{config.Id}";
    }

    public void Execute()
    {
        string dirPath = Path.Combine(defaultDirPath, "backup_" + GetBackupNumber(defaultDirPath, config));
        Directory.CreateDirectory(dirPath);

        config.Sources!.ForEach(source => files.Copy(source.Path, dirPath));

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

            files.Copy(defaultDirPath, dest);
        }
    }
}