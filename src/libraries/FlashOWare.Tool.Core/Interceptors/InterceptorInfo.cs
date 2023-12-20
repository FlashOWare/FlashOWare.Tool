using FlashOWare.Tool.Core.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Diagnostics.CodeAnalysis;

namespace FlashOWare.Tool.Core.Interceptors;

public sealed class InterceptorInfo
{
    internal static InterceptorInfo Create(Compilation compilation, MethodDeclarationSyntax node, IMethodSymbol symbol)
    {
        string filePath = RoslynUtilities.GetInterceptorFilePath(node.SyntaxTree, compilation);

        Location location = node.Identifier.GetLocation();
        FileLinePositionSpan span = location.GetLineSpan();
        int line = span.StartLinePosition.Line + 1;
        int character = span.StartLinePosition.Character + 1;

        string method = symbol.ToDisplayString();

        return new InterceptorInfo(filePath, line, character, method);
    }

    private readonly List<InterceptionInfo> _interceptions = [];

    internal InterceptorInfo()
    {
    }

    [SetsRequiredMembers]
    internal InterceptorInfo(string document, int line, int character, string method)
    {
        Document = document;
        Line = line;
        Character = character;
        Method = method;
    }

    [SetsRequiredMembers]
    public InterceptorInfo(string document, int line, int character, string method, params InterceptionInfo[] interceptions)
        : this(document, line, character, method)
    {
        _interceptions.AddRange(interceptions);
    }

    public required string Document { get; init; }
    public required int Line { get; init; }
    public required int Character { get; init; }
    public required string Method { get; init; }
    public IReadOnlyList<InterceptionInfo> Interceptions => _interceptions;

    internal void AddInterception(InterceptionInfo interception)
    {
        _interceptions.Add(interception);
    }

    public override string ToString()
    {
        return $"""{Method} at ("{Document}":{Line}:{Character}) intercepts {_interceptions.Count} {(_interceptions.Count == 1 ? "location" : "locations")}""";
    }
}
