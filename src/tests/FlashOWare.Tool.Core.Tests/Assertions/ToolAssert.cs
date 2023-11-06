using FlashOWare.Tool.Core.UsingDirectives;
using Microsoft.CodeAnalysis;
using Xunit.Sdk;

namespace FlashOWare.Tool.Core.Tests.Assertions;

internal static class ToolAssert
{
    public static void Equal(UsingDirective[] expected, UsingCountResult actual)
    {
        Assert.Equal("TestProject", actual.ProjectName);
        Equal(expected, actual.Usings);
    }

    public static Task AssertAsync(UsingGlobalizationResult actual, Project project, string localUsing, int occurrences, string targetDocument)
    {
        return AssertAsync(actual, project, new UsingDirective[] { new(localUsing, occurrences) }, targetDocument);
    }

    public static async Task AssertAsync(UsingGlobalizationResult actual, Project project, UsingDirective[] usings, string targetDocument)
    {
        int occurrences = usings.Aggregate(0, static (sum, directive) => sum + directive.Occurrences);

        await RoslynAssert.EqualAsync(project, actual.Project);
        Equal(usings, actual.Usings);
        Assert.Equal(targetDocument, actual.TargetDocument);
        Assert.Equal(occurrences, actual.Occurrences);
    }

    private static void Equal(UsingDirective[] expected, IReadOnlyCollection<UsingDirective> actual)
    {
        if (!expected.SequenceEqual(actual, UsingDirectiveEqualityComparer.Instance))
        {
            string message = $"""
                Expected: [{String.Join<UsingDirective>(", ", expected)}]
                Actual:   [{String.Join<UsingDirective>(", ", actual)}]
                """;
            throw new XunitException(message);
        }
    }
}

file sealed class UsingDirectiveEqualityComparer : IEqualityComparer<UsingDirective>
{
    public static UsingDirectiveEqualityComparer Instance { get; } = new UsingDirectiveEqualityComparer();

    public bool Equals(UsingDirective? x, UsingDirective? y)
    {
        if (ReferenceEquals(x, y))
        {
            return true;
        }

        if (x is null || y is null)
        {
            return false;
        }

        return x.Name == y.Name
            && x.Occurrences == y.Occurrences;
    }

    public int GetHashCode(UsingDirective obj)
    {
        return HashCode.Combine(obj.Name, obj.Occurrences);
    }
}
