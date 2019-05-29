using Microsoft.Extensions.DependencyInjection;
using Quartz;
using System;

namespace Hosting.Extensions.Quartz
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds a job to the scheduler
        /// </summary>
        /// <typeparam name="TService">Abstract service</typeparam>
        /// <typeparam name="TImplementation">Service implementation</typeparam>
        /// <param name="collection">The NET Core dependency injection service</param>
        /// <param name="builder">A function the will receive the JobBuilder and TriggerBuilder to customize the job's behavior</param>
        /// <returns></returns>
        public static IServiceCollection AddJobService<TService, TImplementation>(this IServiceCollection collection, Action<JobBuilder, TriggerBuilder> builder)
            where TService : class
            where TImplementation : class, TService, IJob
        {
            collection.AddSingleton<TService, TImplementation>();
            RegisterJob<TService>(collection, builder);
            return collection;
        }

        /// <summary>
        /// Adds a job to the scheduler
        /// </summary>
        /// <typeparam name="TService">Service implementation</typeparam>
        /// <param name="collection">The NET Core dependency injection service</param>
        /// <param name="builder"></param>
        /// <param name="builder">A function the will receive the JobBuilder and TriggerBuilder to customize the job's behavior</param>
        public static IServiceCollection AddJobService<TService>(this IServiceCollection collection, Action<JobBuilder, TriggerBuilder> builder)
            where TService : class, IJob
        {
            collection.AddSingleton<TService>();
            RegisterJob<TService>(collection, builder);
            return collection;
        }

        private static void RegisterJob<TService>(IServiceCollection collection, Action<JobBuilder, TriggerBuilder> builder) where TService : class
        {
            var jobBuilder = JobBuilder.Create(typeof(TService));
            var triggerBuilder = TriggerBuilder.Create();
            builder(jobBuilder, triggerBuilder);
            collection.AddSingleton<IJobSchedule>(new JobSchedule(jobBuilder.Build(), triggerBuilder.Build()));
        }
    }
}
