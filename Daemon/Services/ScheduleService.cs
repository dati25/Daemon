using Daemon.Models;
using Microsoft.Extensions.Hosting;
using Quartz;
using System.CodeDom.Compiler;
using Microsoft.Extensions.DependencyInjection;
using Daemon.Services.Jobs;
namespace Daemon.Services
{
    public class ScheduleService
    {
        public async Task<IHost> GenerateTriggers(List<Config> configs)
        {
            var builder = GetBuilder();
            var schedulerFactory = builder.Services.GetRequiredService<ISchedulerFactory>();
            var scheduler = await schedulerFactory.GetScheduler();

            var job = JobBuilder.Create<BackupJob>()
                    .WithIdentity("BackupJob", "DaemonJobs")
                    .Build();

            configs.ForEach(config => scheduler.ScheduleJob(job,this.GenerateTrigger(scheduler,config)));

            var updateJob = JobBuilder.Create<UpdateJob>()
                .WithIdentity("UpdateJob", "DaemonJobs")
                .Build();

            await scheduler.ScheduleJob(job, this.GetSimpleTrigger("UpdateTrigger5min", "UpdateTriggers", 300));
            return builder;
        }
        public ITrigger GetSimpleTrigger(string triggerName, string groupName, int intervalSec, int repeatCount = -1)
        {
            return TriggerBuilder.Create()
                .WithIdentity(triggerName, groupName)
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInSeconds(intervalSec)
                    .WithRepeatCount(repeatCount))
                .Build();
        }
        public ITrigger GenerateTrigger(IScheduler sch, Config config)
        {
            var trigger = TriggerBuilder.Create()
                    .WithIdentity($"{config.Name}({config.Id})", "ConfigTriggers")
                    .WithCronSchedule("0 " + config.RepeatPeriod!)
                    .EndAt(DateTime.Parse(config.ExpirationDate!))
                    .UsingJobData(new JobDataMap(new Dictionary<string, Config> { { "config", config } }))
                    .Build();

            return trigger;
        }
        private IHost GetBuilder()
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
