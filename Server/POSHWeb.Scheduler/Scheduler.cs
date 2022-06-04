using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace POSHWeb.Scheduler
{
    public static class Scheduler
    {
        public static void AddQuartz(this IServiceCollection services)
        {
            services.AddQuartz(q =>
            {
                q.SchedulerId = "Scheduler-Core";
                q.UseMicrosoftDependencyInjectionJobFactory();
                q.UseSimpleTypeLoader();
                q.UseInMemoryStore();
                q.UseDefaultThreadPool(tp =>
                {
                    tp.MaxConcurrency = 30;
                });
                q.UseTimeZoneConverter();
            });

            services.AddQuartzServer(options =>
            {
                options.AwaitApplicationStarted = true;
                options.WaitForJobsToComplete = true;
            });
        }

    }
}