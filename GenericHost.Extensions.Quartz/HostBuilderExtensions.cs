using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using System;

namespace GenericHost.Extensions.Quartz
{
    public static class HostBuilderExtensions
    {
        public static IHostBuilder UseQuartz(this IHostBuilder builder, Action<HostBuilderContext, ISchedulerFactory> configure = null)
        {
            builder.ConfigureServices((context, collection) =>
            {
                collection.AddSingleton<IJobFactory, JobFactory>();
                collection.AddSingleton((provider) =>
                {
                    var factory = new StdSchedulerFactory();
                    configure?.Invoke(context, factory);

                    var scheduler = factory.GetScheduler().Result;
                    scheduler.JobFactory = provider.GetRequiredService<IJobFactory>();
                    return scheduler;
                });
                collection.AddHostedService<QuartzHostedService>();
            });

            return builder;
        }
    }
}
