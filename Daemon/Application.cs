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

namespace Daemon
{
    public class Application
    {
        private Settings s = new Settings();

        public async Task Execute()
        {
            // var c = new Client();
            var s = new Settings();

            // await c.Register();

            // BackupHandler bh = new(s.ReadConfigs());
            // bh.Begin();

            await Task.Delay(1);

            var configs = s.ReadConfigs();

            BackupHandler bh = new(configs);
            bh.Begin();



        }
        public async Task TryingQuartz()
        {
            var builder = this.GetBuilder();

            var schedulerFactory = builder.Services.GetRequiredService<ISchedulerFactory>();
            var scheduler = await schedulerFactory.GetScheduler();

            // define the job and tie it to our HelloJob class
            var job = JobBuilder.Create<HelloJob>()
                .WithIdentity("firstJob", "firstJobGroup")
                .Build();

            // Trigger the job to run now, and then every 40 seconds
            var trigger = TriggerBuilder.Create()
                .WithIdentity("myTrigger", "group1")
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInSeconds(40)
                    .RepeatForever())
                .Build();

            await scheduler.ScheduleJob(job, trigger);


            // will block until the last running job completes
            await builder.RunAsync();
        }
        public IHost GetBuilder()
        {
            var builder = Host.CreateDefaultBuilder()
            .ConfigureServices((cxt, services) =>
            {
                services.AddQuartz(q =>
                {
                    q.UseMicrosoftDependencyInjectionJobFactory();
                });
                services.AddQuartzHostedService(opt =>
                {
                    opt.WaitForJobsToComplete = true;
                });
            }).Build();
            return builder;
        }


    }
}
