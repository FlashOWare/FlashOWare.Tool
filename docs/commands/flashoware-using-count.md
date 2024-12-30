# flashoware using count

Available since [v1.0.0].

## Synopsis
```console
flashoware using count [<USINGS>...] [--proj|--project <project>] [--sln|--solution <solution>]

flashoware using count -?|-h|--help
```

## Description

Count and list the top-level using directives of a C# project.

## Arguments

`<USINGS>`  
The names of the top-level using directives to count. If usings are not specified, the command will list all top-level directives.

## Options

- `--proj|--project <project>`  
The path to the project file to operate on (defaults to the current directory if there is exactly one project or solution exclusively).

- `--sln|--solution <solution>`  
The path to the solution (filter) file to operate on (defaults to the current directory if there is exactly one solution or project exclusively).

- `-?|-h|--help`  
Show help and usage information.

[v1.0.0]: ../CHANGELOG.md#vNext
