# Crumbs
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
