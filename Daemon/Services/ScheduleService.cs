using Daemon.Models;
using Microsoft.Extensions.Hosting;
using Quartz;
using System.CodeDom.Compiler;
using Microsoft.Extensions.DependencyInjection;
using Daemon.Services.Jobs;
using ConsoleApp1;
using System.Diagnostics;

namespace Daemon.Services
{
    public class ScheduleService
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private IScheduler scheduler;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public async Task<IHost> GenerateJobs(List<Config> configs)
        {
            var builder = GetBuilder();
            var schedulerFactory = builder.Services.GetRequiredService<ISchedulerFactory>();
            this.scheduler = await schedulerFactory.GetScheduler();
            
                

            //await scheduler.DeleteJob(new JobKey("BackupJob", "DaemonJobs"));
            //await scheduler.DeleteJob(new JobKey("UdateJob, DaemonJobs"));

            var job = JobBuilder.Create<BackupJob>()
                    .WithIdentity("BackupJob", "DaemonJobs")
                    .StoreDurably()
                    .Build();

            configs.ForEach(config => this.scheduler.ScheduleJob(job, this.GenerateTrigger(config)));

            var jobs = this.scheduler.GetCurrentlyExecutingJobs();

            var updateJob = JobBuilder.Create<UpdateJob>()
                .WithIdentity("UpdateJob", "DaemonJobs")
                .StoreDurably()
                .Build();


            await scheduler.ScheduleJob(updateJob, this.GetSimpleTrigger("UpdateTrigger5min", "UpdateTriggers", 60));
            return builder;
        }
        public ITrigger GenerateTrigger(Config config)
        {
            var trigger = TriggerBuilder.Create()
                    .WithIdentity($"{config.Name}({config.Id})", "ConfigTriggers")
                    .WithCronSchedule("0 " + config.RepeatPeriod!)
                    .EndAt(DateTime.Parse(config.ExpirationDate!))
                    .UsingJobData(new JobDataMap(new Dictionary<string, Config> { { "config", config } }))
                    .Build();

            return trigger;
        }
        public ITrigger GetSimpleTrigger(string triggerName, string groupName, int intervalSec, int repeatCount = -1)
        {
            return TriggerBuilder.Create()
                .WithIdentity(triggerName, groupName)
                .StartNow()
                .WithSimpleSchedule(x => x
                .WithIntervalInSeconds(intervalSec)
                .WithRepeatCount(repeatCount))
                .UsingJobData(new JobDataMap(new Dictionary<string, ScheduleService> { { "scheduleService", this } }))
                .Build();
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
        private async Task<Config?> GetTrigger(Config config)
        {
            var triggerKey = new TriggerKey($"{config.Name}({config.Id})", "ConfigTriggers");

            var trigger = await this.scheduler.GetTrigger(triggerKey);

            var map = trigger!.JobDataMap as IDictionary<string, object>;

            return (Config)map!["config"];
        }
        public async void UpdateConfigTrigger(Config config)
        {
            Config? activeConfig = await this.GetTrigger(config);
            if (activeConfig == null)
            {
                var jobKey = new JobKey("BackupJob", "DaemonJobs");
                IJobDetail job = await this.scheduler.GetJobDetail(jobKey) ?? throw new ArgumentException();

                await this.scheduler.ScheduleJob(job!, this.GenerateTrigger(config));
                return;
            }

            if (!config.IsEqualConfig(activeConfig!))
            {
                this.RescheduleTrigger(activeConfig!, config);
                return;
            }

        }
        public void DeleteUnassignedConfigs(List<Config> newConfigs)
        {
            newConfigs.ForEach(config =>
            {
                if (this.GetTrigger(config) == null)
                {
                    var triggerKey = new TriggerKey($"{config.Name}({config.Id})", "ConfigTriggers");
                    this.scheduler.UnscheduleJob(triggerKey);
                }
            });


        }
        private async void RescheduleTrigger(Config activeConfig, Config newConfig)
        {
            var triggerKey = new TriggerKey($"{activeConfig.Name}({activeConfig.Id})", "ConfigTriggers");
            await this.scheduler.RescheduleJob(triggerKey, this.GenerateTrigger(newConfig));
        }
    }
}
