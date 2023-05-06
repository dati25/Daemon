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

            var configs = await this.GetSettings();
            if(configs == null)
                return;

            var schService = new ScheduleService();
            var builder = await schService.GenerateTriggers(configs);
            await builder.RunAsync();
        }


        public async Task<List<Config>> GetSettings()
        {
            var c = new Client();

            await c.Register();

            return s.ReadConfigs()!;
        }
        public async Task TryingQuartz()
        {
            var builder = this.GetBuilder();

            var schedulerFactory = builder.Services.GetRequiredService<ISchedulerFactory>();
            var scheduler = await schedulerFactory.GetScheduler();

            // define the job and tie it to our HelloJob class
            var job = JobBuilder.Create<BackupJob>()
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
        public ITrigger GetSimpleTrigger(string triggerName, string groupName, int interval, int repeatCount = 0)
        {
            return TriggerBuilder.Create()
                .WithIdentity(triggerName, groupName)
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInSeconds(interval)
                    .WithRepeatCount(repeatCount))
                .Build();
        }
        public ITrigger GetCronTrigger(string triggerName, string groupName, string cronExpression)
        {
            return TriggerBuilder.Create()
                    .WithIdentity(triggerName, groupName)
                    .ForJob("BackupJob")
                    .WithCronSchedule(cronExpression)
                    .Build();
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
