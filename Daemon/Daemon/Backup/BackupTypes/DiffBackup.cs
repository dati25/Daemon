using Daemon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daemon.Backup.BackupTypes
{
    public class DiffBackup : IBackup
    {
        private Config config { get; set; }
        public DiffBackup(Config config)
        {
            this.config = config;
        }
        public void Execute()
        {
            //this.CheckSnapshot($"snapshot_{config.id}.bbc");
        }



    }
}
