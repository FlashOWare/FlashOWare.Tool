using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace FlashOWare.Tool.Core.Interceptors;

internal sealed class InterceptorWalker : CSharpSyntaxWalker
{
    private readonly SemanticModel semanticModel;

    public InterceptorWalker(SemanticModel semanticModel)
    {
        this.semanticModel = semanticModel;
    }

    public List<InterceptorInfo> Interceptors { get; } = new();

    public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
    {
        foreach (AttributeSyntax attribute in node.AttributeLists.SelectMany(static attributeList => attributeList.Attributes))
        {
            SymbolInfo info = semanticModel.GetSymbolInfo(attribute);
            IMethodSymbol constructor = (IMethodSymbol)info.Symbol;
            INamedTypeSymbol attributeType = constructor.ContainingType;

            _ = node.SyntaxTree.FilePath;
            _ = semanticModel.GetSymbolInfo(node);

            if (attributeType.Name == "InterceptsLocationAttribute")
            {
                if (attribute.ArgumentList.Arguments is [
                    { Expression: LiteralExpressionSyntax { Token.Value: string filePath } },
                    { Expression: LiteralExpressionSyntax { Token.Value: int line } },
                    { Expression: LiteralExpressionSyntax { Token.Value: int character } }
                    ])
                {
                    Interceptors.Add(new InterceptorInfo(filePath, line, character));
                }
            }

        }

        base.VisitMethodDeclaration(node);
    }
}
