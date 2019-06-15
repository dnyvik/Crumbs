# Crumbs
WIP

Current config works for testing (gives you a resolver back which you can use to get other). Expect major changes!
```
 CrumbsBootstrapper.Configure()
        .UseServiceCollection(servieCollection)
        .UseInproccessMediation()
        .UseSnapshotAllStrategy()
        .UseJsonSerializers()
        .UseHandlersFrom(Assembly.GetExecutingAssembly())
        .UseSqliteTest(servieCollection, "Data Source=YourNameHere.db")
        .UseDefaultStores()
        .TestRun();
```
