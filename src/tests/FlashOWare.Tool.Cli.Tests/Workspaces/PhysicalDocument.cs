using FlashOWare.Tool.Cli.Tests.IO;
using FlashOWare.Tool.Cli.Tests.Testing;

namespace FlashOWare.Tool.Cli.Tests.Workspaces;

internal sealed class PhysicalDocument
{
    public PhysicalDocument(string text, string directory, string file)
    {
        Text = text;
        Directory = directory;
        FullName = Path.Combine(directory, file);
    }

    public string Text { get; }
    public string Directory { get; }
    public string FullName { get; }

    public static PhysicalDocument Create(string text, DirectoryInfo directory, string fileName, Language language)
    {
        string extension = language.GetDocumentExtension(true);
        fileName = PathUtilities.WithExtension(extension, fileName);

        return new PhysicalDocument(text, directory.FullName, fileName);
    }

    public static PhysicalDocument Create(string text, DirectoryInfo directory, string fileName, string[] folders, Language language)
    {
        if (folders.Length == 0)
        {
            throw new ArgumentException($"{nameof(folders.Length)} of {nameof(folders)} is 0.", nameof(folders));
        }

        string folder = folders.Aggregate(directory.FullName, static (aggregate, element) => Path.Combine(aggregate, element));

        string extension = language.GetDocumentExtension(true);
        fileName = PathUtilities.WithExtension(extension, fileName);

        return new PhysicalDocument(text, folder, fileName);
    }
}
