namespace FlashOWare.Tool.Cli;

internal static class Context
{
    public static SemaphoreSlim MSBuildMutex { get; } = new(1, 1);
}
