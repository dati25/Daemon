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
                    .WithIdentity("BackupJob", "BackupJobs")
                    .Build();

            configs.ForEach(config => scheduler.ScheduleJob(job,this.GenerateTrigger(scheduler,config)));

            return builder;
        }
        public ITrigger GenerateTrigger(IScheduler sch, Config config)
        {
            if (config.ExpirationDate == null) //vsechno null, negetuju configy asi vubec
            {
                config.ExpirationDate = "* * * * *";
            }
            var trigger = TriggerBuilder.Create()
                    .WithIdentity($"{config.Name}({config.Id})", "ConfigTriggers")
                    .WithCronSchedule(config.RepeatPeriod!)
                    .Build();

            sch.Context.Put("config", config);
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
