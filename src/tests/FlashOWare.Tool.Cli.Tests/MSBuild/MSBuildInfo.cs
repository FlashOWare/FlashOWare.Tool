using Microsoft.Build.Locator;

namespace FlashOWare.Tool.Cli.Tests.MSBuild;

public sealed class MSBuildInfo
{
    private static readonly Lazy<VisualStudioInstance> s_msBuildInstance = new(MSBuildLocator.RegisterDefaults);

    internal static MSBuildInfo Create()
    {
        return new MSBuildInfo(s_msBuildInstance.Value);
    }

    private MSBuildInfo(VisualStudioInstance instance)
    {
        Instance = instance;
    }

    internal VisualStudioInstance Instance { get; }

    internal bool IsEqual(int major, int minor, int patch)
    {
        return Instance.Version == new Version(major, minor, patch);
    }

    internal bool IsInequal(int major, int minor, int patch)
    {
        return Instance.Version != new Version(major, minor, patch);
    }

    internal bool IsLessThan(int major, int minor, int patch)
    {
        return Instance.Version < new Version(major, minor, patch);
    }

    internal bool IsGreaterThan(int major, int minor, int patch)
    {
        return Instance.Version > new Version(major, minor, patch);
    }

    internal bool IsLessThanOrEqual(int major, int minor, int patch)
    {
        return Instance.Version <= new Version(major, minor, patch);
    }

    internal bool IsGreaterThanOrEqual(int major, int minor, int patch)
    {
        return Instance.Version >= new Version(major, minor, patch);
    }
}
