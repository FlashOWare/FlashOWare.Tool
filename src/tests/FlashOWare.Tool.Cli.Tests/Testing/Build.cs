namespace FlashOWare.Tool.Cli.Tests.Testing;

internal static class Build
{
    public static string Configuration { get; } = GetConfiguration();

    public static string TFM { get; } = GetTargetFrameworkMoniker();

    private static string GetConfiguration()
    {
#if DEBUG
        return "Debug";
#else
        return "Release";
#endif
    }

    private static string GetTargetFrameworkMoniker()
    {
#if NET6_0
        return "net6.0";
#elif NET7_0
        return "net7.0";
#elif NET8_0
        return "net8.0";
#else
#error TFM not implemented.
#endif
    }
}
