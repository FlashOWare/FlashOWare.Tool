# FlashOWare.Tool
Prerelease Changelog

[goto Release_Changelog;](./CHANGELOG.md)

## [vNext]

## [1.0.0-prerelease2] - 2023-12-20
### Tool
- **Added** `flashoware interceptor` command
  - with `-?|-h|--help` option to get help and usage information
- **Added** `flashoware interceptor check` command to show whether interceptors are supported
  - with `-?|-h|--help` option to get help and usage information
- **Added** `flashoware interceptor list` command to find and list interceptors
  - with `--proj|--project <project>` option to set the project file to operate on
    - if not specified, searches the current working directory for a single project file
  - with `--group|--group-by-interceptors` option to group the result by interceptors
    - if not specified, group the result by the intercepted locations
  - with `-?|-h|--help` option to get help and usage information
- **Fixed** `NullReferenceException` in `flashoware using globalize` command when existing `GlobalUsings.cs` contains a _C# 12.0_ _using type alias_ and a _global using directive_ is added

### Package
- **Added** _.NET 8.0_ tool target.

### Dependencies
- **Updated** `Microsoft.Build.Locator`: `1.5.5` --> `1.6.10`
- **Updated** `Microsoft.CodeAnalysis.CSharp.Workspaces`: `4.5.0` --> `4.8.0`
- **Updated** `Microsoft.CodeAnalysis.Workspaces.MSBuild`: `4.5.0` --> `4.8.0`
  - **Security** `System.Drawing.Common`: `4.7.0` --> `4.7.2`

## [1.0.0-prerelease1] - 2023-11-12
### Tool
- **Added** optional `<USINGS>` arguments to `flashoware using count` command to count and list only specified top-level using directives instead of all.
- **Added** `--force` option to `flashoware using globalize` command to enable globalizing all top-level using directives when no `<USINGS>` arguments are specified.
- **Changed** all _project_ option aliases from `-p` to `--proj`! **(BREAKING CHANGE)**
- **Changed** all _project_ options to validate that project file is existing.
- **Changed** all _project_ options to be optional
  - searches the current working directory for a single project file if not specified
- **Changed** `<USING>` argument of `flashoware using globalize` command to be optional.
- **Changed** `<USING>` argument of `flashoware using globalize` command to `<USINGS>` arguments globalizing one or more top-level using directives.
- **Fixed** `flashoware using globalize` command only globalizing the last of duplicate top-level using directives
  - now removes all duplicate top-level using directives when globalized to match the count of the result

### Package
- **Added** `README.md` file.
- **Changed** `Icon.png` from dark theme to light theme if pre-release.
- **Changed** Release Notes linking to either the stable or the pre-release `CHANGELOG.md`.

## [1.0.0-prerelease0] - 2023-09-13
### Tool
- **Added** `flashoware` root command (supports cancellation)
  - with `-!|-a|--about` option to get application information
  - with `-#|-i|--info` option to get environment information
  - with `--version` option to get version information
  - with `-?|-h|--help` option to get help and usage information
- **Added** `flashoware using` command
  - with `-?|-h|--help` option to get help and usage information
- **Added** `flashoware using count` command to count and list all top-level using directives
  - with `-p|--project <project>` option to set the project file to operate on
  - with `-?|-h|--help` option to get help and usage information
- **Added** `flashoware using globalize` command to change a top-level using directive to a global using directive
  - with `<USING>` argument to set the name of the top-level using directive to convert to a global using directive
  - with `-p|--project <project>` option to set the project file to operate on
  - with `-?|-h|--help` option to get help and usage information

### Package
- **Added** .NET tool targeting _.NET 6.0_ and _.NET 7.0_.

### Dependencies
- **Added** `Microsoft.Build.Locator`: `1.5.5`
- **Added** `Microsoft.CodeAnalysis.CSharp.Workspaces`: `4.5.0`
- **Added** `Microsoft.CodeAnalysis.Workspaces.MSBuild`: `4.5.0`
- **Added** `System.CommandLine`: `2.0.0-beta4.22272.1`

[vnext]: https://github.com/FlashOWare/FlashOWare.Tool/compare/v1.0.0-prerelease2...HEAD
[1.0.0-prerelease2]: https://github.com/FlashOWare/FlashOWare.Tool/compare/v1.0.0-prerelease1...v1.0.0-prerelease2
[1.0.0-prerelease1]: https://github.com/FlashOWare/FlashOWare.Tool/compare/v1.0.0-prerelease0...v1.0.0-prerelease1
[1.0.0-prerelease0]: https://github.com/FlashOWare/FlashOWare.Tool/releases/tag/v1.0.0-prerelease0
