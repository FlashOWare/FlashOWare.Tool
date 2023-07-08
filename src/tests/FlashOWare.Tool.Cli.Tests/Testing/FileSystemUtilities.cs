namespace FlashOWare.Tool.Cli.Tests.Testing;

internal static class FileSystemUtilities
{
    public static string TestProject { get; } = GetTestProject();

    private static string GetTestProject()
    {
        var testDirectory = GetTestDirectory();
        return Path.Combine(testDirectory.FullName, "ProjectUnderTest.NetCore", "ProjectUnderTest.NetCore.csproj");
    }

    private static DirectoryInfo GetTestDirectory()
    {
        var current = new DirectoryInfo(Directory.GetCurrentDirectory());
        var testDirectory = Path.Combine("src", "tests");

        while (!current.FullName.EndsWith(testDirectory, StringComparison.InvariantCulture))
        {
            if (current.Parent is null)
            {
                throw new InvalidOperationException($"Test directory not found: '{testDirectory}'");
            }
            current = current.Parent;
        }

        return current;
    }
}
