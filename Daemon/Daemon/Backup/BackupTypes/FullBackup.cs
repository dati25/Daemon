using Daemon.Models;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daemon.Backup.BackupTypes
{
    public class FullBackup : BackupService
    {
        private string defaultDirPath;
        private Config config { get; set; }
        public FullBackup(Config config)
        {
            this.config = config;
			this.defaultDirPath = @$"{config.Destinations.First().Path}\FooBakCup\config_{config.Id}";
		}
        public void Execute()
        {
            string dirPath = string.Join(@"\", defaultDirPath, "backup_"+GetLastBackupNumber(defaultDirPath));
			Directory.CreateDirectory(dirPath);

            SnapshotExists(dirPath, config.Id);

            config.Sources.ForEach(source => DoBackup(source.Path, dirPath, true));
			CompressFiles(dirPath);

            BackupDestinations();
		}
        private void DoBackup(string sourcePath, string dirPath, bool first)
        {
            files.Copy(sourcePath, dirPath);
        }

        private void BackupDestinations()
        {
			for (int i = 1; i < config.Destinations.Count; i++)
			{
				string dest = $@"{config.Destinations[i].Path}\FooBakCup\config_{config.Id}";
				if (config.Compress == true)
                    dest += ".zip";

			    files.Copy(defaultDirPath, dest);
				
			}
		}

        private void CompressFiles(string dirPath)
        {
            if (config.Compress == true)
            {
                ZipFile.CreateFromDirectory(dirPath, dirPath + ".zip");
                Directory.Delete(dirPath, true);
            }
        }

    }
}
