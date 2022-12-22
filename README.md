[![GitHub license](https://img.shields.io/badge/license-MIT-blue.svg?style=flat-square)](https://raw.githubusercontent.com/EngRajabi/MediatR.Useful.Behaviors/master/LICENSE)
[![Nuget](https://img.shields.io/nuget/dt/MediatR.Useful.Behavior?label=Nuget.org%20Downloads&style=flat-square&color=blue)](https://www.nuget.org/packages/MediatR.Useful.Behavior)
[![Nuget](https://img.shields.io/nuget/vpre/MediatR.Useful.Behavior.svg?label=NuGet)](https://www.nuget.org/packages/MediatR.Useful.Behavior)


# The best behaviors of Mediatr

We all used mediatr many times. In mediatr, we have a very popular behavior feature that allows us to program aop and cross cutting. We have used it many times.
But it must have happened to you that you did not combine these behaviors together.
Instead of implementing these behaviors each time with our own knowledge, I decided to gather these behaviors together so that we can become better every day and have a collective participation in it.
In this package, I tried to collect the best and most useful behaviors of mediatr. and put all the useful requirements of the program in it.

**Package** - [MediatR.Useful.Behavior](https://www.nuget.org/packages/MediatR.Useful.Behavior/)

Currently, there are 2 very popular and efficient behaviors in this package
- Automatic caching with many features
- Automatic validation



Add the package to your application using


```bash
dotnet add package MediatR.Useful.Behavior
```

How to activate behaviors in the **Startup.cs** file

```csharp
service.AddAllBehaviors();
```