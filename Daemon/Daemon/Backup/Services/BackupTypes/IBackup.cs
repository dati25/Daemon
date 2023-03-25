using Daemon.Backup.Services;
using Daemon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daemon.Backup.Services.BackupTypes
{
    public abstract class IBackup
    {
        protected FilesService files = new FilesService();



        public void CheckSnapshot(string path, int idConfig)
        {
            // Check jestli snapshot
            // if false - vytvori ho

            if (File.Exists($@"{path}\snapshot_{idConfig}.bbc"))
                return;

            File.Create($@"{path}\snapshot_{idConfig}.bbc");
        }
    }
}
