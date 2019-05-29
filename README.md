# GenericHost.Extensions.Quartz

[![Build status](https://ci.appveyor.com/api/projects/status/hja78a47mohg714g/branch/master?svg=true)](https://ci.appveyor.com/project/skiptirengu/hosting-extensions-quartz/branch/master)

Integrate Quartz .NET with .NET Core's Host

## Usage

To enable the Quartz integration, simply call the extension method `UseQuartz()` on your HostBuilder.

```c#
new HostBuilder().UseQuartz((context, config) =>
{
    config.Set("quartz.threadPool.threadCount", "2");
})
```

The callback is opitional and can be used to customize Quartz default options. You can also set the options in your `appsettings.json` file.

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

To schedule a new job, call the extension method `AddJobService()` from an `IServiceCollection` instance.

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

Licensed under the [Apache License 2.0](./LICENSE.md) license.