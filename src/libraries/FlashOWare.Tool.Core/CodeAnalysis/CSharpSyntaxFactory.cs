using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Options;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace FlashOWare.Tool.Core.CodeAnalysis;

internal static class CSharpSyntaxFactory
{
    public static CompilationUnitSyntax GlobalUsingDirectiveRoot(string name, DocumentOptionSet options)
    {
        return CompilationUnit()
            .WithUsings(GlobalUsingDirectiveList(name, options))
            .WithEndOfFileToken(Token(SyntaxKind.EndOfFileToken));
    }

    private static SyntaxList<UsingDirectiveSyntax> GlobalUsingDirectiveList(string name, DocumentOptionSet options)
    {
        return SingletonList(GlobalUsingDirective(name, options));
    }

    public static UsingDirectiveSyntax GlobalUsingDirective(string name, DocumentOptionSet options)
    {
        return UsingDirective(IdentifierName(name))
            .WithGlobalKeyword(Token(TriviaList(), SyntaxKind.GlobalKeyword, TriviaList(Space)))
            .WithUsingKeyword(Token(TriviaList(), SyntaxKind.UsingKeyword, TriviaList(Space)))
            .WithSemicolonToken(Token(TriviaList(), SyntaxKind.SemicolonToken, EndOfLineList(options)));
    }

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
