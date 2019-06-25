# Crumbs

[![Build status](https://ci.appveyor.com/api/projects/status/xx22nmyb3fcs0c3b?svg=true)](https://ci.appveyor.com/project/dnyvik/crumbs)


WIP

For service/console:
```csharp
 CrumbsBootstrapper.Configure()
        .UseServiceCollection(servieCollection)
        .UseInproccessMediation()
        .UseSnapshotAllStrategy()
        .UseJsonSerializers()
        .UseHandlersFrom(Assembly.GetExecutingAssembly())
        .UseSqlite("Data Source=YourNameHere.db")
        .UseDefaultStores()
        .TestRun();
```
For ASP.NET Core:
```csharp
 servieCollection.AddCrumbs((options) =>
            {
                options.UseInproccessMediation()
                .UseSnapshotAllStrategy()
                .UseJsonSerializers()
                .UseHandlersFrom(Assembly.GetExecutingAssembly())
                .UseSqlite("Data Source=YourNameHere.db")
                .UseDefaultStores();
            });
```

