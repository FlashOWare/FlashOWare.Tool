using FlashOWare.Tool.Core.UsingDirectives;
using Xunit.Sdk;

namespace FlashOWare.Tool.Core.Tests.Assertions;

internal static class ToolAssert
{
    public static void Equal(UsingDirective[] expected, UsingCountResult actual)
    {
        Assert.Equal("TestProject", actual.ProjectName);

        if (!expected.SequenceEqual(actual.Usings, EqualityComparerUsingDirective.Instance))
        {
            string message = $"""
                Expected: [{String.Join<UsingDirective>(", ", expected)}]
                Actual:   [{String.Join<UsingDirective>(", ", actual.Usings)}]
                """;
            throw new XunitException(message);
        }
    }
}

file sealed class EqualityComparerUsingDirective : IEqualityComparer<UsingDirective>
{
    public static EqualityComparerUsingDirective Instance { get; } = new EqualityComparerUsingDirective();

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
