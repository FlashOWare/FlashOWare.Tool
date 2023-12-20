namespace FlashOWare.Tool.Cli;

internal static class CliOptions
{
    public static Option<FileInfo> Project { get; } = new Option<FileInfo>(["--project", "--proj"], "The path to the project file to operate on (defaults to the current directory if there is only one project).")
        .ExistingOnly();
}
