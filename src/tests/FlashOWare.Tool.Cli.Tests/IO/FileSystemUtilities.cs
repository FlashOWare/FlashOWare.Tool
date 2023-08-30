namespace FlashOWare.Tool.Cli.Tests.IO;

internal static class FileSystemUtilities
{
    public static string RepositoryRootDirectory { get; } = GetDirectoryContainingDirectory(".git").FullName;

    public static string ArtifactsDirectory { get; } = Path.Combine(RepositoryRootDirectory, "artifacts");

    public static string ScratchDirectory { get; } = Path.Combine(ArtifactsDirectory, "scratch");

    public static DirectoryInfo CreateScratchDirectory(string configuration, string tfm, string prefix, int suffix)
    {
        string time = DateTime.Now.ToString("yyyy-MM-dd'T'HH-mm-ss.fffffff");
        string name = $"{prefix}_{time}_{suffix}";

        string path = Path.Combine(ScratchDirectory, configuration, tfm, name);
        return Directory.CreateDirectory(path);
    }

    private static DirectoryInfo GetDirectoryContainingDirectory(string directoryName)
    {
        var current = new DirectoryInfo(Directory.GetCurrentDirectory());

        do
        {
            foreach (DirectoryInfo directory in current.EnumerateDirectories())
            {
                if (directory.Name == directoryName)
                {
                    return current;
                }
            }

            current = current.Parent;
        } while (current is not null);

        throw new InvalidOperationException($"{nameof(Directory)} not found: '{directoryName}'");
    }

    private static DirectoryInfo GetDirectoryContainingFile(string fileName)
    {
        var current = new DirectoryInfo(Directory.GetCurrentDirectory());

        do
        {
            foreach (FileInfo file in current.EnumerateFiles())
            {
                if (file.Name == fileName)
                {
                    return current;
                }
            }

            current = current.Parent;
        } while (current is not null);

        throw new InvalidOperationException($"{nameof(File)} not found: '{fileName}'");
    }
}
