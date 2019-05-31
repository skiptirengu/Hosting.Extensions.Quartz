# Hosting.Extensions.Quartz

[![Build status](https://ci.appveyor.com/api/projects/status/hja78a47mohg714g/branch/master?svg=true)](https://ci.appveyor.com/project/skiptirengu/hosting-extensions-quartz/branch/master)

Easily integrate .NET Core's Host with Quartz

## Usage

Check out the [working example](./examples).

To enable the Quartz integration, simply call one of the three `UseQuartz()` extension methods on your HostBuilder.

```c#
new HostBuilder().UseQuartz((context, provider, scheduler) =>
{
    // You can further configure the scheduler instance here, like 
    // add job listeners, trigger listeners, etc.
    // DO NOT call the Start method here as it will be automatically
    // invoked by the hosted service once it has started.
    scheduler.ListenerManager.AddJobListener(
      new JobChainingJobListener("TestListener"), KeyMatcher<JobKey>.KeyEquals(new JobKey("DummyJob"))
    );
})
```

To change Quartz options, add a section to your `appsettings.json` file, or use the `UserQuartz()` method.

```json
{
  "Quartz": {
    "quartz.threadPool.type": "Quartz.Simpl.SimpleThreadPool, Quartz",
    "quartz.threadPool.threadCount": 5,
    "quartz.threadPool.threadPriority": "Normal",
    "quartz.jobStore.type": "Quartz.Simpl.RAMJobStore, Quartz",
    "quartz.jobStore.misfireThreshold": 6000
  }
}
```

or

```c#
new HostBuilder().UseQuartz((context, config) =>
{
    config.Set("quartz.threadPool.threadCount", "2");
})
```

To schedule a new job, call the extension method `AddJobService()` on a `IServiceCollection` instance.

```c#
new HostBuilder().ConfigureServices((services) =>
{
    services.AddJobService<DummyJob>((job, trigger) =>
    {
        job.WithIdentity("job1").WithDescription("Simple job");
        trigger.StartNow().WithSimpleSchedule((x) => x.WithIntervalInSeconds(5).RepeatForever());
    });
    services.AddJobService<IMyJobInterface, MyJob>((job, trigger) =>
    {
        job.WithIdentity("job2").WithDescription("Simple job");
        trigger.StartNow().WithSimpleSchedule((x) => x.WithIntervalInSeconds(60).RepeatForever());
    });
})
```

## License

Licensed under the [Apache License 2.0](./LICENSE) license.
