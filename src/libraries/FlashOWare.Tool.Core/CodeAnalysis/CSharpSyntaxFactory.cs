using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Options;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace FlashOWare.Tool.Core.CodeAnalysis;

internal static partial class CSharpSyntaxFactory
{
    public static CompilationUnitSyntax GlobalUsingDirectiveRoot(string name, DocumentOptionSet options)
    {
        return CompilationUnit()
            .WithUsings(GlobalUsingDirectiveList(name, options))
            .WithEndOfFileToken(Token(SyntaxKind.EndOfFileToken));
    }

    public static CompilationUnitSyntax GlobalUsingDirectivesRoot(IEnumerable<string> names, DocumentOptionSet options)
    {
        return CompilationUnit()
            .WithUsings(GlobalUsingDirectiveList(names, options))
            .WithEndOfFileToken(Token(SyntaxKind.EndOfFileToken));
    }

    private static SyntaxList<UsingDirectiveSyntax> GlobalUsingDirectiveList(string name, DocumentOptionSet options)
    {
        return SingletonList(GlobalUsingDirective(name, options));
    }

    private static SyntaxList<UsingDirectiveSyntax> GlobalUsingDirectiveList(IEnumerable<string> names, DocumentOptionSet options)
    {
        return List(GlobalUsingDirectives(names, options));
    }

    public static UsingDirectiveSyntax GlobalUsingDirective(string name, DocumentOptionSet options)
    {
        return UsingDirective(IdentifierName(name))
            .WithGlobalKeyword(Token(TriviaList(), SyntaxKind.GlobalKeyword, TriviaList(Space)))
            .WithUsingKeyword(Token(TriviaList(), SyntaxKind.UsingKeyword, TriviaList(Space)))
            .WithSemicolonToken(Token(TriviaList(), SyntaxKind.SemicolonToken, EndOfLineList(options)));
    }

    public static IEnumerable<UsingDirectiveSyntax> GlobalUsingDirectives(IEnumerable<string> names, DocumentOptionSet options)
    {
        foreach (var name in names)
        {
            yield return GlobalUsingDirective(name, options);
        }
    }
}

internal static partial class CSharpSyntaxFactory
{
    private static SyntaxTriviaList EndOfLineList(DocumentOptionSet options)
    {
        return TriviaList(EndOfLine(options));
    }

    private static SyntaxTrivia EndOfLine(DocumentOptionSet options)
    {
        string text = options.GetOption(FormattingOptions.NewLine);
        return SyntaxFactory.EndOfLine(text);
    }
}
