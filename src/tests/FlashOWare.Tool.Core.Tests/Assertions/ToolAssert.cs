using FlashOWare.Tool.Core.UsingDirectives;
using Microsoft.CodeAnalysis;
using Xunit.Sdk;

namespace FlashOWare.Tool.Core.Tests.Assertions;

internal static class ToolAssert
{
    public static void Equal(UsingDirective[] expected, UsingCountResult actual)
    {
        Assert.Equal("TestProject", actual.ProjectName);

        if (!expected.SequenceEqual(actual.Usings, UsingDirectiveEqualityComparer.Instance))
        {
            string message = $"""
                Expected: [{String.Join<UsingDirective>(", ", expected)}]
                Actual:   [{String.Join<UsingDirective>(", ", actual.Usings)}]
                """;
            throw new XunitException(message);
        }
    }

    public static async Task AssertAsync(UsingGlobalizationResult actual, Project project, string localUsing, int occurrences, string targetDocument)
    {
        await RoslynAssert.EqualAsync(project, actual.Project);
        Assert.Equal(localUsing, actual.Using.Name);
        Assert.Equal(occurrences, actual.Using.Occurrences);
        Assert.Equal(targetDocument, actual.TargetDocument);
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
