using Quartz;
using RealEaseApp.Jobs;

namespace RealEaseApp.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddQuartzJobs(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddQuartz(q =>
        {
            q.UseMicrosoftDependencyInjectionJobFactory();
            q.AddQuartzJob<ScrapeJob>(configuration);
        });

        services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
        return services;
    }

    private static IServiceCollectionQuartzConfigurator AddQuartzJob<T>(this IServiceCollectionQuartzConfigurator q, IConfiguration configuration) where T : IJob
    {
        var jobName = typeof(T).Name;
        var jobKey = new JobKey(jobName);
        var cronSchedule = configuration.GetSection($"Quartz:{jobName}").Value;
        if (string.IsNullOrEmpty(cronSchedule))
        {
            throw new Exception($"No Quartz.NET Cron schedule found for job with jobName {jobName}");
        }

        q.AddJob<T>(opts => opts.WithIdentity(jobKey));
        q.AddTrigger(opts => opts
            .WithIdentity($"trigger-{jobName}")
            .ForJob(jobKey)
            // see https://www.quartz-scheduler.net/documentation/quartz-3.x/tutorial/crontriggers.html
            .WithCronSchedule(cronSchedule));
        return q;
    }
}