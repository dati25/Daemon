using Daemon.Backup;
using Daemon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daemon
{
    public class Application
    {
        private BackupHandler handler { get; set; } = new BackupHandler();
        public void Execute()
        {
            handler.Begin();
        }

    }
}
