using DiffPlex.DiffBuilder;
using DiffPlex.DiffBuilder.Model;
using Microsoft.CodeAnalysis;
using System.Diagnostics;
using System.Text;
using Xunit.Sdk;

namespace FlashOWare.Tool.Core.Tests.Assertions;

internal static class RoslynAssert
{
    public static async Task EqualAsync(Project expected, Project actual)
    {
        if (ReferenceEqualityComparer.Instance.Equals(expected, actual))
        {
            string message = $"{nameof(expected)} and {nameof(actual)} are {nameof(ReferenceEquals)}";
            throw new XunitException(message);
        }

        if (expected.Name != actual.Name)
        {
            string message = $"""
                Expected: {expected.Name}
                Actual:   {actual.Name}
                """;
            throw new XunitException(message);
        }

        var expectedDocuments = expected.Documents.ToArray();
        var actualDocuments = actual.Documents.ToArray();

        if (expectedDocuments.Length != actualDocuments.Length)
        {
            string message = $"""
                Expected: {CreateLengthMessage(expectedDocuments)}
                Actual:   {CreateLengthMessage(actualDocuments)}
                """;
            throw new XunitException(message);
        }

        for (int i = 0; i < expectedDocuments.Length; i++)
        {
            var expectedDocument = expectedDocuments[i];
            var actualDocument = actualDocuments[i];

            string expectedDocumentPath = GetPath(expectedDocument);
            string actualDocumentPath = GetPath(actualDocument);

            var expectedText = await GetNormalizedTextAsync(expectedDocument);
            var actualText = await GetNormalizedTextAsync(actualDocument);

            bool hasTextChanged = expectedText != actualText;
            if (hasTextChanged || expectedDocumentPath != actualDocumentPath)
            {
                if (!hasTextChanged)
                {
                    throw new XunitException($"""
                        Expected: {expectedDocumentPath}
                        Actual:   {actualDocumentPath}
                        Content did match. No diff to be shown:

                        {actualText}
                        
                        """);
                }

                var message = new StringBuilder($"""
                    Expected: {expectedDocumentPath}
                    Actual:   {actualDocumentPath}
                    Content did not match. Diff shown with expected as baseline:

                    """);

                var diff = InlineDiffBuilder.Diff(expectedText, actualText, false, false, null);

                foreach (var line in diff.Lines)
                {
                    _ = line.Type switch
                    {
                        ChangeType.Inserted => message.Append('+'),
                        ChangeType.Deleted => message.Append('-'),
                        _ => message.Append(' '),
                    };
                    _ = message.AppendLine(line.Text);
                }

                throw new XunitException(message.ToString());
            }
        }
    }

    private static string CreateLengthMessage(Document[] documents)
    {
        return documents.Length == 1
            ? $"{documents.Length} {nameof(Document)}"
            : $"{documents.Length} {nameof(Document)}s";
    }

    private static string GetPath(Document document)
    {
        Debug.Assert(document.FilePath is null, $"Expected no document file: {document.FilePath}");
        return String.Join(Path.DirectorySeparatorChar, document.Folders.Append(document.Name));
    }

    private static async Task<string> GetNormalizedTextAsync(Document document)
    {
        var sourceText = await document.GetTextAsync(CancellationToken.None);
        return sourceText.ToString().ReplaceLineEndings();
    }
}
