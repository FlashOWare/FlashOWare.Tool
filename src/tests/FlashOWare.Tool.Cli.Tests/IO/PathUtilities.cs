using System.Diagnostics;

namespace FlashOWare.Tool.Cli.Tests.IO;

internal static class PathUtilities
{
    public static string WithExtension(string extension, string path)
    {
        Debug.Assert(extension.StartsWith('.'), $"Extension '{extension}' does not include the period.");

        return path.EndsWith(extension, StringComparison.InvariantCulture)
            ? path
            : String.Concat(path, extension);
    }
}
