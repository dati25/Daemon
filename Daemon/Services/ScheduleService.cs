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
#pragma warning restore CS8618
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

            await this.scheduler.AddJob(job, true);

            if (configs != null)
                configs.ForEach(config =>
                {
                    this.scheduler.ScheduleJob(this.GenerateTrigger(config, job));
                    Console.WriteLine(config.Name);
                });

            //var jobs = this.scheduler.GetCurrentlyExecutingJobs();

            var updateJob = JobBuilder.Create<UpdateJob>()
                .WithIdentity("UpdateJob", "DaemonJobs")
                .StoreDurably()
                .Build();

            await scheduler.ScheduleJob(updateJob, this.GetSimpleTrigger($"UpdateTrigger5min", "UpdateTriggers", 30));

            await this.scheduler.Start();
            return builder;
        }
        public ITrigger GenerateTrigger(Config config, IJobDetail job)
        {
            return TriggerBuilder.Create()
                    .WithIdentity($"Config({config.Id})", "ConfigTriggers")
                    .WithCronSchedule("0 " + config.RepeatPeriod)
                    .EndAt(DateTime.Parse(config.ExpirationDate!))
                    .UsingJobData(new JobDataMap(new Dictionary<string, Config> { { "config", config } }))
                    .ForJob(job)
                    .WithPriority(1)
                    .Build();
        }
        public ITrigger GetSimpleTrigger(string triggerName, string groupName, int intervalSec, int repeatCount = -1)
        {
            return TriggerBuilder.Create()
                .WithIdentity(triggerName, groupName)
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
            var triggerKey = new TriggerKey($"Config({config.Id})", "ConfigTriggers");

            var trigger = await this.scheduler.GetTrigger(triggerKey);
            if (trigger == null)
                return null;

            var map = trigger!.JobDataMap as IDictionary<string, object>;

            return (Config)map!["config"];
        }
        private async Task<IJobDetail?> GetJob()
        {
            var jobKey = new JobKey("BackupJob", "DaemonJobs");
            return await this.scheduler.GetJobDetail(jobKey)!;
        }
        public async Task UpdateConfigTrigger(Config config)
        {
            Config? activeConfig = await this.GetTrigger(config);
            if (activeConfig == null)
            {
                var job = await this.GetJob();

                this.scheduler.ScheduleJob(this.GenerateTrigger(config, job!)).GetAwaiter();
                return;
            }

            if (!config.IsEqualConfig(activeConfig!))
                await this.RescheduleTrigger(activeConfig!, config);

        }
        public void DeleteUnassignedConfigs()
        {
            Settings settings = new();
            var newConfigs = settings.ReadConfigs();

            if (newConfigs == null)
                return;

            if(newConfigs.Count == 0)
            {
                this.GetAllTriggers().ForEach(trigger => this.scheduler.UnscheduleJob(trigger.Key));
                return;
            }

            var activeTriggers = this.GetAllTriggers();

            if (activeTriggers.Count == 0)
                return;

            foreach (var config in newConfigs)
            {
                var newTriggerKey = new TriggerKey($"Config({config.Id})", "ConfigTriggers");
                if (activeTriggers.Any(trigger => trigger.Key.Name == newTriggerKey.Name))
                    continue;

                this.scheduler.UnscheduleJob(newTriggerKey);
            }
        }
        private async Task RescheduleTrigger(Config activeConfig, Config newConfig)
        {
            var job = await this.GetJob();
            var triggerKey = new TriggerKey($"Config({activeConfig.Id})", "ConfigTriggers");
            await this.scheduler.RescheduleJob(triggerKey, this.GenerateTrigger(newConfig, job!));
        }
        public List<ITrigger> GetAllTriggers()
        {
            List<string> list = new();
            var jobKey = new JobKey("BackupJob", "DaemonJobs");
            var TriggerList = this.scheduler.GetTriggersOfJob(jobKey).Result;
            return TriggerList.ToList();
        }
    }
}
