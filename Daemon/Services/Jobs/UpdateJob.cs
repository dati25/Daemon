using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;

namespace Daemon.Services.Jobs
{
    internal class UpdateJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            Settings settings = new Settings();
            settings.Update();
        }
    }
}
