using FlashOWare.Tool.Core.Interceptors;
using FlashOWare.Tool.Core.UsingDirectives;
using Microsoft.CodeAnalysis;
using System.Text;
using Xunit.Sdk;

namespace FlashOWare.Tool.Core.Tests.Assertions;

internal static partial class ToolAssert
{
    public static void Equal(InterceptorList actual, params InterceptorInfo[] expected)
    {
        Assert.Equal("TestProject", actual.ProjectName);

        if (!expected.SequenceEqual(actual.Interceptors, InterceptorInfoEqualityComparer.Instance))
        {
            StringBuilder message = new();

            message.AppendLine("Expected");
            foreach (InterceptorInfo interceptor in expected)
            {
                AppendInterceptor(message, interceptor);
            }
            message.AppendLine("Actual");
            foreach (InterceptorInfo interceptor in actual.Interceptors)
            {
                AppendInterceptor(message, interceptor);
            }

            throw new XunitException(message.ToString());
        }

        static void AppendInterceptor(StringBuilder message, InterceptorInfo interceptor)
        {
            message.AppendLine($"- {interceptor}");
            foreach (InterceptionInfo interception in interceptor.Interceptions)
            {
                message.AppendLine($"  - {interception}");
            }
        }
    }
}

internal static partial class ToolAssert
{
    public static void Equal(UsingDirective[] expected, UsingCountResult actual)
    {
        Assert.Equal("TestProject", actual.ProjectName);
        Equal(expected, actual.Usings);
    }

    public static Task AssertAsync(UsingGlobalizationResult actual, Project project, string localUsing, int occurrences, string targetDocument)
    {
        return AssertAsync(actual, project, [new(localUsing, occurrences)], targetDocument);
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

file sealed class InterceptorInfoEqualityComparer : IEqualityComparer<InterceptorInfo>
{
    public static InterceptorInfoEqualityComparer Instance { get; } = new InterceptorInfoEqualityComparer();

    private InterceptorInfoEqualityComparer() { }

    public bool Equals(InterceptorInfo? x, InterceptorInfo? y)
    {
        if (ReferenceEquals(x, y))
        {
            return true;
        }

        if (x is null || y is null)
        {
            return false;
        }

        return x.Document == y.Document
            && x.Line == y.Line
            && x.Character == y.Character
            && x.Method == y.Method
            && x.Interceptions.SequenceEqual(y.Interceptions);
    }

    public int GetHashCode(InterceptorInfo obj)
    {
        HashCode hashCode = new();

        hashCode.Add(obj.Document);
        hashCode.Add(obj.Line);
        hashCode.Add(obj.Character);
        hashCode.Add(obj.Method);

        foreach (InterceptionInfo interception in obj.Interceptions)
        {
            hashCode.Add(interception);
        }

        return hashCode.ToHashCode();
    }
}

file sealed class UsingDirectiveEqualityComparer : IEqualityComparer<UsingDirective>
{
    public static UsingDirectiveEqualityComparer Instance { get; } = new UsingDirectiveEqualityComparer();

    private UsingDirectiveEqualityComparer() { }

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
