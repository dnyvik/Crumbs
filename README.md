# Crumbs

[![Build status](https://ci.appveyor.com/api/projects/status/xx22nmyb3fcs0c3b?svg=true)](https://ci.appveyor.com/project/dnyvik/crumbs)


WIP

Use current config for testing (returns a resolver). Expect major changes!
```
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
