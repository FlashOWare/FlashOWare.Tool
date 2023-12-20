using DiffPlex.DiffBuilder;
using DiffPlex.DiffBuilder.Model;
using FlashOWare.Tool.Cli.Tests.Testing;
using System.Runtime.CompilerServices;
using System.Text;
using Xunit.Sdk;
using TextFile = (string Text, string FilePath);

namespace FlashOWare.Tool.Cli.Tests.IO;

internal sealed class FileSystemExpectation
{
    private readonly List<TextFile> _files = [];

    private readonly DirectoryInfo _directory;
    private readonly Language _language;

    public FileSystemExpectation(DirectoryInfo directory, Language language)
    {
        _directory = directory;
        _language = language;
    }

    public FileSystemExpectation AppendFile(string text, string filePath)
    {
        ThrowIfDuplicateFilePath(filePath);

        TextFile file = (text, filePath);
        _files.Add(file);
        return this;
    }

    public FileSystemExpectation AppendFile(string text, string name, params string[] folders)
    {
        StringBuilder path = new();
        path.AppendJoin(Path.DirectorySeparatorChar, folders);
        path.Append(Path.DirectorySeparatorChar);
        path.Append(name);
        string filePath = path.ToString();

        ThrowIfDuplicateFilePath(filePath, null);

        TextFile file = (text, filePath);
        _files.Add(file);
        return this;
    }

    public FileSystemExpectation AppendDocument(string text, string filePath)
    {
        string extension = _language.GetDocumentExtension(true);
        filePath = PathUtilities.WithExtension(extension, filePath);

        return AppendFile(text, filePath);
    }

    public FileSystemExpectation AppendDocument(string text, string name, params string[] folders)
    {
        string extension = _language.GetDocumentExtension(true);
        name = PathUtilities.WithExtension(extension, name);

        return AppendFile(text, name, folders);
    }

    public FileSystemExpectation AppendProject(string text, string filePath)
    {
        string extension = _language.GetProjectExtension(true);
        filePath = PathUtilities.WithExtension(extension, filePath);

        return AppendFile(text, filePath);
    }

    public FileSystemExpectation AppendProject(string text, string name, params string[] folders)
    {
        string extension = _language.GetProjectExtension(true);
        name = PathUtilities.WithExtension(extension, name);

        return AppendFile(text, name, folders);
    }

    public void Verify()
    {
        FileInfo[] actualFiles = GetFiles();

        AssertFileSystemTree(actualFiles);

        for (int i = 0; i < _files.Count; i++)
        {
            TextFile expectedFile = _files[i];
            FileInfo actualFile = actualFiles.First(actualFile =>
            {
                string relativePath = Path.GetRelativePath(_directory.FullName, actualFile.FullName);
                return relativePath == expectedFile.FilePath;
            });

            string expectedText = expectedFile.Text;
            string actualText = File.ReadAllText(actualFile.FullName);

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

    private void ThrowIfDuplicateFilePath(string filePath, [CallerArgumentExpression(nameof(filePath))] string? paramName = null)
    {
        if (_files.Any(file => file.FilePath == filePath))
        {
            TextFile file = _files.Single(file => file.FilePath == filePath);
            throw new ArgumentException($"A file with the same path has already been added. FilePath: {file.FilePath}", paramName);
        }
    }

    private FileInfo[] GetFiles()
    {
        return _directory.EnumerateFiles("*", SearchOption.AllDirectories)
            .Where((FileInfo file) =>
            {
                string relativePath = Path.GetRelativePath(_directory.FullName, file.FullName);
                return !relativePath.StartsWith($"bin{Path.DirectorySeparatorChar}") && !relativePath.StartsWith($"obj{Path.DirectorySeparatorChar}");
            })
            .ToArray();
    }

    private void AssertFileSystemTree(FileInfo[] actualFiles)
    {
        if (_files.Count != actualFiles.Length || !IsFileSystemTreeEqual(actualFiles))
        {
            StringBuilder message = new($"""
                Expected: {(_files.Count == 1 ? $"{_files.Count} file" : $"{_files.Count} files")}
                Actual:   {(actualFiles.Length == 1 ? $"{actualFiles.Length} file" : $"{actualFiles.Length} files")}
                Hierarchy did not match. Diff shown with expected as baseline:


                """);

            StringBuilder actualText = new();
            foreach (var actualFile in actualFiles)
            {
                string relativePath = Path.GetRelativePath(_directory.FullName, actualFile.FullName);
                actualText.AppendLine(relativePath);
            }

            StringBuilder expectedText = new();
            foreach (TextFile expectedFile in _files)
            {
                expectedText.AppendLine(expectedFile.FilePath);
            }

            Diff(expectedText.ToString(), actualText.ToString(), message);

            throw new XunitException(message.ToString());
        }
    }

    private bool IsFileSystemTreeEqual(FileInfo[] actualFiles)
    {
        return !actualFiles
            .Select(actualFile => Path.GetRelativePath(_directory.FullName, actualFile.FullName))
            .Except(_files.Select(static expectedFile => expectedFile.FilePath))
            .Any();
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
