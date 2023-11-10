# flashoware using globalize

Available since [v1.0.0].

## Synopsis
```console
flashoware using globalize [<USINGS>...] [--proj|--project <project>] [--force]

flashoware using globalize -?|-h|--help
```

## Description

Move top-level using directives to global using directives in a C# project.

## Arguments

`<USINGS>`  
The names of the top-level using directives to convert to global using directives. If usings are not specified, the command will globalize all top-level directives.

## Options

- `--proj|--project <project>`  
The path to the project file to operate on (defaults to the current directory if there is only one project).

- `--force`  
Forces all top-level using directives to be globalized when no usings are specified.

- `-?|-h|--help`  
Show help and usage information.

[v1.0.0]: ../CHANGELOG.md#vNext
