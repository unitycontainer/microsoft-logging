[![Build status](https://ci.appveyor.com/api/projects/status/r97hcdjf377ty6kq/branch/master?svg=true)](https://ci.appveyor.com/project/IoC-Unity/microsoft-logging/branch/master)
[![codecov](https://codecov.io/gh/unitycontainer/microsoft-logging/branch/master/graph/badge.svg)](https://codecov.io/gh/unitycontainer/microsoft-logging)
[![License](https://img.shields.io/badge/license-apache%202.0-60C060.svg)](https://github.com/IoC-Unity/microsoft-logging/blob/master/LICENSE)
[![Build status](https://ci.appveyor.com/api/projects/status/r97hcdjf377ty6kq/branch/master?svg=true)](https://ci.appveyor.com/project/IoC-Unity/microsoft-logging/branch/master) 
[![NuGet](https://img.shields.io/nuget/v/Unity.Microsoft.Logging.svg)](https://www.nuget.org/packages/Unity.Microsoft.Logging)

# Microsoft.Extensions.Logging

Unity extension to integrate with [Microsoft.Extensions.Logging](https://www.nuget.org/packages?q=Microsoft.Extensions.Logging).

## Getting Started

- Reference the [Unity.Microsoft.Logging](https://www.nuget.org/packages/Unity.Microsoft.Logging) package from NuGet.

```shell
Install-Package Unity.Microsoft.Logging 
```

### Create and configure LoggerFactory

```C#
ILoggerFactory loggerFactory = new LoggerFactory();
loggerFactory.AddProvider(new ConsoleLoggerProvider((text, logLevel) => logLevel >= LogLevel.Debug, false));
```

### Get the container

```C#
var container = new UnityContainer();
```

### Register extension and pass it configured factory

```C#
container.AddExtension(new LoggingExtension(loggerFactory));

// Register few types
container.RegisterType<IService, Service>();

var service = container.Resolve<IService>();
```

For more information see [this example](https://github.com/unitycontainer/examples/tree/master/src/Logging/Microsoft.Logging)

## Code of Conduct

This project has adopted the code of conduct defined by the [Contributor Covenant](https://www.contributor-covenant.org/) to clarify expected behavior in our community. For more information, see the [.NET Foundation Code of Conduct](https://www.dotnetfoundation.org/code-of-conduct)

## Contributing

See the [Contributing guide](https://github.com/unitycontainer/unity/blob/master/CONTRIBUTING.md) for more information.

## .NET Foundation

Unity Container is a [.NET Foundation](https://dotnetfoundation.org/projects/unitycontainer) project
