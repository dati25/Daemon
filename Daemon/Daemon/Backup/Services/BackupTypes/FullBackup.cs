using Daemon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daemon.Backup.Services.BackupTypes
{
    public class FullBackup : IBackup
    {
        private Config config { get; set; }
        public FullBackup(Config config)
        {
            this.config = config;
        }
        public void Execute()
        {
            string defaultDirPath = @$"{config.Destinations.First().Path}\FooBakCup\Backup_{config.id}";

            string dirPath = String.Join(@"\", defaultDirPath, GetLastBackupNumber(defaultDirPath));
            Directory.CreateDirectory(dirPath);

            this.CheckSnapshot(defaultDirPath, config.id);

            config.Sources.ForEach(source => DoBackup(source.Path, dirPath, true));

            for (int i = 1; i < config.Destinations.Count; i++)
            {
                DoBackup(defaultDirPath, @$"{config.Destinations.First().Path}\FooBakCup\Backup_{config.id}", false);
            }
        }
        private void DoBackup(string sourcePath, string dirPath, bool first)
        {
            files.Copy(sourcePath, dirPath);
            if (first)
            {
                //zapise do snapshotu
            }
        }
        private void BackupDestination(string defaultDirPath, string destPath)
        {
            files.Copy(defaultDirPath, destPath);
        }
        private int GetLastBackupNumber(string path)
        {
            DirectoryInfo dir = new DirectoryInfo(path);


            List<DirectoryInfo> dirs = dir.GetDirectories().ToList();
            if (dirs.Count == 0)
                return 0;

            string name = dirs.Last().Name;

            return int.Parse(name.Substring(name.LastIndexOf('_')));
        }

    }
}
