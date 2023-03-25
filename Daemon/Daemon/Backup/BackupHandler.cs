using Daemon.Backup.Services.BackupTypes;
using Daemon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Daemon.Backup
{
    public class BackupHandler
    {
        private List<Config> configs;
        public BackupHandler()
        {
            //Getting configs
        }

        public void Begin()
        {
            foreach (var config in configs)
            {
                this.DistributeConfigs(config.Type.ToLower());
            }

        }
        public void DistributeConfigs(string type)
        {
            switch (type)
            {
                case "full":

                    FullBackup full = new FullBackup(new Config());

                    full.Execute();
                    break;
                case "diff":

                    break;
                case "incr":

                    break;
            }

        }
    }
}
