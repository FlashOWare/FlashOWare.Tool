using DiffPlex.DiffBuilder;
using DiffPlex.DiffBuilder.Model;
using Microsoft.CodeAnalysis;
using System.Text;
using Xunit.Sdk;

namespace FlashOWare.Tool.Core.Tests.Assertions;

internal static class RoslynAssert
{
    public static async Task EqualAsync(Project expected, Project actual)
    {
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

            var expectedSource = await expectedDocument.GetTextAsync(CancellationToken.None);
            var actualSource = await actualDocument.GetTextAsync(CancellationToken.None);

            var expectedText = expectedSource.ToString().ReplaceLineEndings();
            var actualText = actualSource.ToString().ReplaceLineEndings();

            if (expectedDocument.Name != actualDocument.Name || expectedText != actualText)
            {
                var message = new StringBuilder($"""
                    Expected: {expectedDocument.Name}
                    Actual:   {actualDocument.Name}
                    Content did not match. Diff shown with expected as baseline:

                    """);

                var diff = InlineDiffBuilder.Diff(expectedText, actualText);

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
}
