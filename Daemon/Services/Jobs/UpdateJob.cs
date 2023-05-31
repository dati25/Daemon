using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Daemon.Models;
using Quartz;

namespace Daemon.Services.Jobs
{
    internal class UpdateJob : IJob
    {
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task Execute(IJobExecutionContext context)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            var settings = new Settings();

            var map = context.MergedJobDataMap as IDictionary<string, object>;
            var scheduleService = (ScheduleService)map!["scheduleService"];

            settings.Update(scheduleService);
            //Test jenom
            var triggerList = scheduleService.GetAllTriggers();

            if (triggerList.Count > 0)
            {
                triggerList.ForEach(x => Console.WriteLine(x.Key.Name));

            }
            else
            {
                Console.WriteLine("zadny");
            }//////

            Console.WriteLine($"Update-{DateTime.Now}");
        }
    }
}
