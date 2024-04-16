using System.Text;

namespace FlashOWare.Tool.Cli.Tests.Text;

internal static class Encodings
{
    public static Encoding UTF8NoBOM { get; } = new UTF8Encoding(false, true);
}
