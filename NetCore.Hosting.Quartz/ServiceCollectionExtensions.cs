using Microsoft.Extensions.DependencyInjection;
using Quartz;
using System;

namespace NetCore.Hosting.Quartz
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddJobService<TService, TImplementation>(this IServiceCollection collection, Action<JobBuilder, TriggerBuilder> builder)
            where TService : class, IJob
            where TImplementation : class, TService
        {
            collection.AddSingleton<TService, TImplementation>();
            RegisterJob<TService>(collection, builder);
            return collection;
        }

        public static IServiceCollection AddJobService<TService>(this IServiceCollection collection, Action<JobBuilder, TriggerBuilder> builder)
            where TService : class, IJob
        {
            collection.AddSingleton<TService>();
            RegisterJob<TService>(collection, builder);
            return collection;
        }

        private static void RegisterJob<TService>(IServiceCollection collection, Action<JobBuilder, TriggerBuilder> builder) where TService : class, IJob
        {
            var jobBuilder = JobBuilder.Create(typeof(TService));
            var triggerBuilder = TriggerBuilder.Create();
            builder(jobBuilder, triggerBuilder);
            collection.AddSingleton<IJobSchedule>(new JobSchedule(jobBuilder.Build(), triggerBuilder.Build()));
        }
    }
}
