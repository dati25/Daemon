using Quartz;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Daemon.Models;
using Quartz.Xml;

namespace Daemon.Services.Jobs
{
    public class BackupJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            var map = context.MergedJobDataMap as IDictionary<string, object>;
            var config = (Config)map["config"];

            var backup = new Backup(config);

            backup.Execute();
        }
    }
}
