using System.CommandLine.IO;

namespace FlashOWare.Tool.Cli.CommandLine;

internal static class ConsoleExtensions
{
    public static ConsoleColor GetBackgroundColor(this IConsole console)
    {
        return Console.BackgroundColor;
    }

    public static void SetBackgroundColor(this IConsole console, ConsoleColor color)
    {
        Console.BackgroundColor = color;
    }

    public static ConsoleColor GetForegroundColor(this IConsole console)
    {
        return Console.ForegroundColor;
    }

    public static void SetForegroundColor(this IConsole console, ConsoleColor color)
    {
        Console.ForegroundColor = color;
    }

    public static void Write(this IConsole console, ConsoleColor foregroundColor, string value)
    {
        ConsoleColor oldColor = Console.ForegroundColor;
        Console.ForegroundColor = foregroundColor;
        console.Out.Write(value);
        Console.ForegroundColor = oldColor;
    }

    public static void WriteLine(this IConsole console, ConsoleColor foregroundColor, string value)
    {
        ConsoleColor oldColor = Console.ForegroundColor;
        Console.ForegroundColor = foregroundColor;
        console.Out.WriteLine(value);
        Console.ForegroundColor = oldColor;
    }
}
