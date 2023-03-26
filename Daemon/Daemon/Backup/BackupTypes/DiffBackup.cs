using Daemon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daemon.Backup.BackupTypes
{
    public class DiffBackup : BackupService
    {
        private Config config { get; set; }
        public DiffBackup(Config config)
        {
            this.config = config;
        }
        public void Execute()
        {
            string defaultDirPath = @$"{config.Destinations.First().Path}\FooBakCup\config_{config.Id}";
            string dirPath = string.Join(@"\", defaultDirPath, GetLastBackupNumber(defaultDirPath));





        }



    }
}
