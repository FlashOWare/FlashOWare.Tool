# FlashOWare.Tool
A .NET tool that facilitates development workflows.

![Icon-Light](https://raw.githubusercontent.com/FlashOWare/FlashOWare.Tool/main/resources/FlashOWare.Tool-Light.png#gh-light-mode-only)![Icon-Dark](https://raw.githubusercontent.com/FlashOWare/FlashOWare.Tool/main/resources/FlashOWare.Tool-Dark.png#gh-dark-mode-only)

## NuGet package
[FlashOWare.Tool](https://www.nuget.org/packages/FlashOWare.Tool)

## Installation
Install the latest pre-release version as a global tool
```console
dotnet tool install --global FlashOWare.Tool --prerelease
```

## Compatibility
- requires the [.NET 6.0](https://dotnet.microsoft.com/download/dotnet/6.0) SDK
- supports the [.NET 7.0](https://dotnet.microsoft.com/download/dotnet/7.0) SDK

## Documentation
[Index.md](./docs/Index.md)

## Changelogs
- [Release Changelog](./docs/CHANGELOG.md)
- [Prerelease Changelog](./docs/CHANGELOG-Prerelease.md)

## Dependencies
### Runtime dependencies (tool)
- [.NET](https://github.com/dotnet/runtime)
- [Microsoft.Build.Locator](https://github.com/microsoft/MSBuildLocator)
- [Roslyn](https://github.com/dotnet/roslyn)
- [System.CommandLine](https://github.com/dotnet/command-line-api)
### Development dependencies (build and test)
- [Basic.Reference.Assemblies](https://github.com/jaredpar/basic-reference-assemblies)
- [Coverlet](https://github.com/coverlet-coverage/coverlet)
- [DiffPlex](https://github.com/mmanela/diffplex)
- [Microsoft.NET.Test.Sdk](https://github.com/microsoft/vstest)
- [NUKE](https://nuke.build)
- [xUnit.net](https://xunit.net)
