# FlashOWare.Tool
Prerelease Changelog

[goto Release_Changelog;](./CHANGELOG.md)

## [vNext]

## [1.0.0-prerelease1] - 2023-11-11
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

[vnext]: https://github.com/FlashOWare/FlashOWare.Tool/compare/v1.0.0-prerelease1...HEAD
[1.0.0-prerelease1]: https://github.com/FlashOWare/FlashOWare.Tool/compare/v1.0.0-prerelease0...v1.0.0-prerelease1
[1.0.0-prerelease0]: https://github.com/FlashOWare/FlashOWare.Tool/releases/tag/v1.0.0-prerelease0
