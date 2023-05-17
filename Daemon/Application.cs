using Daemon.Services;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;
using Microsoft.Extensions.DependencyInjection;
using Daemon.Services.Jobs;
using System.Security.Permissions;
using Daemon.Models;

namespace Daemon
{
    public class Application
    {
        private Settings s = new Settings();

        public async Task Run()
        {
            var c = new Client();
            await c.Register();

            await Task.Delay(1);
            await Task.Delay(1);

            var configs = s.ReadConfigs();
            //if (configs == null)
            //    return;
            //if(configs.Count !> 0)
            //    return;

            var schService = new ScheduleService();
            var builder = await schService.GenerateTriggers(configs);
            await builder.RunAsync();

        }
    }
}
