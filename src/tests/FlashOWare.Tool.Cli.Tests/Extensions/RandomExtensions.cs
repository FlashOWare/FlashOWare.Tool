namespace FlashOWare.Tool.Cli.Tests.Extensions;

internal static class RandomExtensions
{
    public static bool NextBoolean(this Random receiver)
    {
        return receiver.Next(2) == 1;
    }
}
