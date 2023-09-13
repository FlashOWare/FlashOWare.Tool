using DiffPlex.DiffBuilder;
using DiffPlex.DiffBuilder.Model;
using FlashOWare.Tool.Cli.Tests.Testing;
using System.Text;
using Xunit.Sdk;

namespace FlashOWare.Tool.Cli.Tests.IO;

internal sealed class FileSystemExpectation
{
    private readonly List<(string Text, string FilePath)> _files = new();

    private readonly DirectoryInfo _directory;
    private readonly Language _language;

    public FileSystemExpectation(DirectoryInfo directory, Language language)
    {
        _directory = directory;
        _language = language;
    }

    public FileSystemExpectation AppendFile(string text, string name)
    {
        var file = (text, name);
        _files.Add(file);
        return this;
    }

    public FileSystemExpectation AppendFile(string text, string name, params string[] folders)
    {
        StringBuilder path = new();
        path.AppendJoin(Path.DirectorySeparatorChar, folders);
        path.Append(Path.DirectorySeparatorChar);
        path.Append(name);

        var file = (text, path.ToString());
        _files.Add(file);
        return this;
    }
    public FileSystemExpectation AppendDocument(string text, string name)
    {
        string extension = _language.GetDocumentExtension(true);
        name = PathUtilities.WithExtension(extension, name);

        return AppendFile(text, name);
    }

    public FileSystemExpectation AppendDocument(string text, string name, params string[] folders)
    {
        string extension = _language.GetDocumentExtension(true);
        name = PathUtilities.WithExtension(extension, name);

        return AppendFile(text, name, folders);
    }

    public FileSystemExpectation AppendProject(string text, string name)
    {
        string extension = _language.GetProjectExtension(true);
        name = PathUtilities.WithExtension(extension, name);

        return AppendFile(text, name);
    }

    public FileSystemExpectation AppendProject(string text, string name, params string[] folders)
    {
        string extension = _language.GetProjectExtension(true);
        name = PathUtilities.WithExtension(extension, name);

        return AppendFile(text, name, folders);
    }

    public void Verify()
    {
        FileInfo[] files = GetFiles();

        AssertFileSystemTree(files);

        for (int i = 0; i < _files.Count; i++)
        {
            var expectedFile = _files[i];
            var actualFile = files[i];

            var expectedText = expectedFile.Text;
            var actualText = File.ReadAllText(actualFile.FullName);

            if (expectedText != actualText)
            {
                var message = new StringBuilder($"""
                    Expected: {expectedFile.FilePath}
                    Actual:   {Path.GetRelativePath(_directory.FullName, actualFile.FullName)}
                    Content did not match. Diff shown with expected as baseline:


                    """);

                Diff(expectedText, actualText, message);

                throw new XunitException(message.ToString());
            }
        }
    }

    private FileInfo[] GetFiles()
    {
        return _directory.EnumerateFiles("*", SearchOption.AllDirectories)
            .Where((file) =>
            {
                string path = Path.GetRelativePath(_directory.FullName, file.FullName);
                return !path.StartsWith($"bin{Path.DirectorySeparatorChar}") && !path.StartsWith($"obj{Path.DirectorySeparatorChar}");
            })
            .ToArray();
    }

    private void AssertFileSystemTree(FileInfo[] actual)
    {
        if (_files.Count != actual.Length || !IsFileSystemTreeEqual(actual))
        {
            StringBuilder message = new($"""
                Expected: {(_files.Count == 1 ? $"{_files.Count} file" : $"{_files.Count} files")}
                Actual:   {(actual.Length == 1 ? $"{actual.Length} file" : $"{actual.Length} files")}
                Hierarchy did not match. Diff shown with expected as baseline:


                """);

            StringBuilder actualText = new();
            foreach (var info in actual)
            {
                string path = Path.GetRelativePath(_directory.FullName, info.FullName);
                actualText.AppendLine(path);
            }

            StringBuilder expectedText = new();
            foreach (var file in _files)
            {
                expectedText.AppendLine(file.FilePath);
            }

            Diff(expectedText.ToString(), actualText.ToString(), message);

            throw new XunitException(message.ToString());
        }
    }

    private bool IsFileSystemTreeEqual(FileInfo[] files)
    {
        for (int i = 0; i < _files.Count; i++)
        {
            var expectedFile = _files[i];
            var actualFile = files[i];

            string relativePath = Path.GetRelativePath(_directory.FullName, actualFile.FullName);

            if (expectedFile.FilePath != relativePath)
            {
                return false;
            }
        }

        return true;
    }

    private static void Diff(string expected, string actual, StringBuilder destination)
    {
        var diff = InlineDiffBuilder.Diff(expected, actual, false, false, null);

        foreach (var line in diff.Lines)
        {
            _ = line.Type switch
            {
                ChangeType.Inserted => destination.Append('+'),
                ChangeType.Deleted => destination.Append('-'),
                _ => destination.Append(' '),
            };
            _ = destination.AppendLine(line.Text);
        }
    }
}
