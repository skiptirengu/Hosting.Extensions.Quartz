using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using System;
using System.Linq;

namespace Hosting.Extensions.Quartz
{
    public static class HostBuilderExtensions
    {
        /// <summary>
        /// Registers the Scheduler, JobFactory, QuartzConfigCollection and the QuartzHostedService on the service collection
        /// </summary>
        /// <returns>The HostBuilder itselft</returns>
        public static IHostBuilder UseQuartz(this IHostBuilder builder)
        {
            return UseQuartz(builder, null, null);
        }

        /// <summary>
        /// Registers the Scheduler, JobFactory, QuartzConfigCollection and the QuartzHostedService on the service collection
        /// </summary>
        /// <param name="builder">The NET Core HostBuilder</param>
        /// <param name="configure">A function that will receive the HostBuilderContext and the StdSchedulerFactory config collection</param>
        /// <returns>The HostBuilder itselft</returns>
        public static IHostBuilder UseQuartz(
            this IHostBuilder builder,
            Action<HostBuilderContext, QuartzConfigCollection> configure = null)
        {
            return UseQuartz(builder, configure);
        }

        /// <summary>
        /// Registers the Scheduler, JobFactory, QuartzConfigCollection and the QuartzHostedService on the service collection
        /// </summary>
        /// <param name="builder">The NET Core HostBuilder</param>
        /// <param name="configure">A function that will receive the HostBuilderContext, the ServiceProvider and the IScheduler instance</param>
        /// <returns>The HostBuilder itselft</returns>
        public static IHostBuilder UseQuartz(
            this IHostBuilder builder,
            Action<HostBuilderContext, IServiceProvider, IScheduler> configure)
        {
            return UseQuartz(builder, null, configure);
        }

        /// <summary>
        /// Registers the Scheduler, JobFactory, QuartzConfigCollection and the QuartzHostedService on the service collection
        /// </summary>
        /// <param name="builder">The NET Core HostBuilder</param>
        /// <param name="configureFactory">A function that will receive the HostBuilderContext and the StdSchedulerFactory config collection</param>
        /// <param name="configureScheduler">A function that will receive the HostBuilderContext, the ServiceProvider and the IScheduler instance</param>
        /// <returns>The HostBuilder itselft</returns>
        public static IHostBuilder UseQuartz(
            this IHostBuilder builder,
            Action<HostBuilderContext, QuartzConfigCollection> configureFactory = null,
            Action<HostBuilderContext, IServiceProvider, IScheduler> configureScheduler = null)
        {
            builder.ConfigureServices((context, collection) =>
            {
                var config = new QuartzConfigCollection();
                context.Configuration.GetSection("Quartz").GetChildren().ToList().ForEach((x) => config.Set(x.Key, x.Value));
                collection.AddSingleton(config);
                collection.AddHostedService<QuartzHostedService>();
                collection.AddSingleton<IJobFactory, JobFactory>();
                collection.AddSingleton((provider) =>
                {
                    configureFactory?.Invoke(context, config);
                    var factory = new StdSchedulerFactory(config);
                    var scheduler = factory.GetScheduler().Result;
                    scheduler.JobFactory = provider.GetRequiredService<IJobFactory>();
                    configureScheduler?.Invoke(context, provider, scheduler);
                    return scheduler;
                });
            });

            return builder;
        }
    }
}
