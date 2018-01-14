[![Build status](https://ci.appveyor.com/api/projects/status/r97hcdjf377ty6kq/branch/master?svg=true)](https://ci.appveyor.com/project/IoC-Unity/microsoft-logging/branch/master)
[![codecov](https://codecov.io/gh/unitycontainer/microsoft-logging/branch/master/graph/badge.svg)](https://codecov.io/gh/unitycontainer/microsoft-logging)
[![License](https://img.shields.io/badge/license-apache%202.0-60C060.svg)](https://github.com/IoC-Unity/microsoft-logging/blob/master/LICENSE)
[![Build status](https://ci.appveyor.com/api/projects/status/r97hcdjf377ty6kq/branch/master?svg=true)](https://ci.appveyor.com/project/IoC-Unity/microsoft-logging/branch/master) 
[![NuGet](https://img.shields.io/nuget/v/Unity.Microsoft.Logging.svg)](https://www.nuget.org/packages/Unity.Microsoft.Logging)


# Microsoft.Extensions.Logging
Unity extension to integrate with Microsoft.Extensions.Logging.

## Getting Started
- Reference the `Unity.Microsoft.Logging` package from NuGet.
```
Install-Package Unity.Microsoft.Logging 
```

## Registration:
- Add `LoggingExtension` extension to the container

```C#
container = new UnityContainer();
container.AddNewExtension<LoggingExtension>()
```
- Where required add `ILogger` or  `ILogger<T>` interface to resolved constructor. 

```C#
public class LoggedType
{
    public LoggedType(ILogger<LoggedType> log)
    {
    }
  ...
}
```
- Log normally...


