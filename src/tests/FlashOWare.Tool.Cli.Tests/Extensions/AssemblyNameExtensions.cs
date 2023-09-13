using System.Reflection;

namespace FlashOWare.Tool.Cli.Tests.Extensions;

internal static class AssemblyNameExtensions
{
    public static Version GetVersion(this AssemblyName assemblyName)
    {
        Version? version = assemblyName.Version;

        if (version is null)
        {
            throw new InvalidOperationException($"{nameof(Version)} is null.");
        }

        return version;
    }
}
