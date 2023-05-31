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
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task Execute(IJobExecutionContext context)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            var map = context.MergedJobDataMap as IDictionary<string, object>;
            var config = (Config)map["config"];

            Settings settings = new();
            var pc = settings.ReadPc();
            if(pc == null) return;  

            var backup = new Backup(config, pc);


            Console.WriteLine($"Backup-Config({config.Id})/{DateTime.Now}");
            return;
            backup.Execute();

            Console.WriteLine($"Backup-Config({config.Id})/{DateTime.Now}");
        }
    }
}
