using Microsoft.Build.Locator;

namespace FlashOWare.Tool.Cli;

internal static class CliContext
{
    public static SemaphoreSlim MSBuildMutex { get; } = new(1, 1);

    public static bool IsTest { get; private set; }
    public static VisualStudioInstance MSBuild { get; private set; } = null!;

    public static void InitializeApp(VisualStudioInstance msBuild)
    {
        MSBuild = msBuild;
    }

    public static void InitializeTest(VisualStudioInstance msBuild)
    {
        IsTest = true;

        MSBuild ??= msBuild;
    }

    public static void Dispose()
    {
        if (IsTest)
        {
            return;
        }

        MSBuildMutex.Dispose();
    }
}
