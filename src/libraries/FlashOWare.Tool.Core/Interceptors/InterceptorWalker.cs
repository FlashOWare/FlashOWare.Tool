using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Diagnostics;

namespace FlashOWare.Tool.Core.Interceptors;

internal sealed class InterceptorWalker : CSharpSyntaxWalker
{
    private readonly Compilation _compilation;
    private readonly SemanticModel _semanticModel;
    private readonly CancellationToken _cancellationToken;

    public InterceptorWalker(Compilation compilation, SemanticModel semanticModel, CancellationToken cancellationToken)
    {
        _compilation = compilation;
        _semanticModel = semanticModel;
        _cancellationToken = cancellationToken;
    }

    public List<InterceptorInfo> Interceptors { get; } = [];

    public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
    {
        InterceptorInfo? interceptor = null;

        IMethodSymbol? symbol = _semanticModel.GetDeclaredSymbol(node, _cancellationToken);
        Debug.Assert(symbol is not null, $"{nameof(InterceptorWalker)}.{nameof(VisitMethodDeclaration)}: {nameof(Microsoft.CodeAnalysis.CSharp.CSharpExtensions.GetDeclaredSymbol)}");

        ImmutableArray<AttributeData> attributes = symbol.GetAttributes();

        foreach (AttributeSyntax attribute in node.AttributeLists.SelectMany(static attributeList => attributeList.Attributes))
        {
            Location location = attribute.GetLocation();
            AttributeData data = attributes.Single(data =>
            {
                SyntaxReference? reference = data.ApplicationSyntaxReference;
                Debug.Assert(reference is not null, $"Reference to the {nameof(SyntaxNode)} of attribute '{data}' not found.");

                SyntaxNode syntaxNode = reference.GetSyntax(_cancellationToken);
                return syntaxNode.GetLocation() == location;
            });

            INamedTypeSymbol? type = data.AttributeClass;
            Debug.Assert(type is not null, $"{nameof(AttributeData)} '{data}' has no {nameof(AttributeData.AttributeClass)}.");

            if (type.ContainingNamespace.ToDisplayString() == "System.Runtime.CompilerServices" && type.Name == "InterceptsLocationAttribute")
            {
                if (data.ConstructorArguments is [
                    { IsNull: false, Kind: TypedConstantKind.Primitive, Type.SpecialType: SpecialType.System_String, Value: string filePath },
                    { IsNull: false, Kind: TypedConstantKind.Primitive, Type.SpecialType: SpecialType.System_Int32, Value: int line },
                    { IsNull: false, Kind: TypedConstantKind.Primitive, Type.SpecialType: SpecialType.System_Int32, Value: int character },
                ])
                {
                    interceptor ??= InterceptorInfo.Create(_compilation, node, symbol);

                    InterceptsLocationAttributeArguments args = new(filePath, line, character);
                    InterceptionInfo interception = new(args);
                    interceptor.AddInterception(interception);
                }
            }
        }

        if (interceptor is not null)
        {
            Interceptors.Add(interceptor);
        }

        base.VisitMethodDeclaration(node);
    }
}
