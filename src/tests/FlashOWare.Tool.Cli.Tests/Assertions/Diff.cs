using DiffPlex.DiffBuilder;
using DiffPlex.DiffBuilder.Model;
using FlashOWare.Tool.Cli.Tests.IO;
using System.Text;
using Xunit.Sdk;

namespace FlashOWare.Tool.Cli.Tests.Assertions;

internal static class Diff
{
    public static void Assert(PhysicalDocument[] expected, PhysicalDocument[] actual, DirectoryInfo directory)
    {
        if (expected.Length != actual.Length)
        {
            string message = $"""
                Expected: {CreateLengthMessage(expected)}
                Actual:   {CreateLengthMessage(actual)}
                """;
            throw new XunitException(message);
        }

        for (int i = 0; i < expected.Length; i++)
        {
            var expectedDocument = expected[i];
            var actualDocument = actual[i];

            string expectedDocumentPath = expectedDocument.FilePath;
            string actualDocumentPath = Path.GetRelativePath(directory.FullName, actualDocument.FilePath);

            var expectedText = expectedDocument.Text;
            var actualText = actualDocument.Text;

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

    private static string CreateLengthMessage(PhysicalDocument[] documents)
    {
        return documents.Length == 1
            ? $"{documents.Length} {nameof(PhysicalDocument)}"
            : $"{documents.Length} {nameof(PhysicalDocument)}s";
    }
}
