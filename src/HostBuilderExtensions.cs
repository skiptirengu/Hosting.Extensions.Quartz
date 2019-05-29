using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz.Impl;
using Quartz.Spi;
using System;
using System.Collections.Specialized;
using System.Linq;

namespace Hosting.Extensions.Quartz
{
    public static class HostBuilderExtensions
    {
        /// <summary>
        /// Registers the Scheduler, JobFactory and the QuartzHostedService on the ServiceCollection
        /// </summary>
        /// <param name="builder">The NET Core HostBuilder</param>
        /// <param name="configure">A function that will receive the HostBuilderContext and the StdSchedulerFactory config collection</param>
        /// <returns>The HostBuilder itselft</returns>
        public static IHostBuilder UseQuartz(this IHostBuilder builder, Action<HostBuilderContext, NameValueCollection> configure = null)
        {
            builder.ConfigureServices((context, collection) =>
            {
                var config = new NameValueCollection();
                context.Configuration.GetSection("Quartz").GetChildren().ToList().ForEach((x) => config.Set(x.Key, x.Value));
                collection.AddHostedService<QuartzHostedService>();
                collection.AddSingleton<IJobFactory, JobFactory>();
                collection.AddSingleton((provider) =>
                {
                    configure?.Invoke(context, config);
                    var factory = new StdSchedulerFactory(config);

                    var scheduler = factory.GetScheduler().Result;
                    scheduler.JobFactory = provider.GetRequiredService<IJobFactory>();
                    return scheduler;
                });
            });

            return builder;
        }
    }
}
