using FlashOWare.Tool.Core.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Diagnostics;

namespace FlashOWare.Tool.Core.Interceptors;

public static class InterceptorLocator
{
    public static async Task<InterceptorList> ListAsync(Project project, CancellationToken cancellationToken = default)
    {
        RoslynUtilities.ThrowIfNotCSharp(project);

        Compilation compilation = await RoslynUtilities.GetCompilationAsync(project, cancellationToken);

        RoslynUtilities.ThrowIfContainsError(compilation);

        cancellationToken.ThrowIfCancellationRequested();

        List<InterceptorInfo> interceptors = await FindInterceptorsAsync(project, compilation, cancellationToken);
        InterceptorList result = await FindInterceptedCallsAsync(project, compilation, interceptors, cancellationToken);

        return result;
    }

    private static async Task<List<InterceptorInfo>> FindInterceptorsAsync(Project project, Compilation compilation, CancellationToken cancellationToken)
    {
        List<InterceptorInfo> interceptors = [];

        IEnumerable<SourceGeneratedDocument> generated = await project.GetSourceGeneratedDocumentsAsync(cancellationToken);

        foreach (Document document in project.Documents.Concat(generated))
        {
            SyntaxTree syntaxTree = await RoslynUtilities.GetSyntaxTreeAsync(document, cancellationToken);
            string filePath = RoslynUtilities.GetInterceptorFilePath(syntaxTree, compilation);

            if (!Path.GetExtension(filePath).Equals(".cs", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            SemanticModel semanticModel = await RoslynUtilities.GetSemanticModelAsync(document, cancellationToken);
            InterceptorWalker walker = new(compilation, semanticModel, cancellationToken);

            CompilationUnitSyntax compilationUnit = RoslynUtilities.GetCompilationUnitRoot(syntaxTree, cancellationToken);
            walker.VisitCompilationUnit(compilationUnit);

            interceptors.AddRange(walker.Interceptors);
        }

        return interceptors;
    }

    private static async Task<InterceptorList> FindInterceptedCallsAsync(Project project, Compilation compilation, List<InterceptorInfo> interceptors, CancellationToken cancellationToken)
    {
        foreach (InterceptorInfo interceptor in interceptors)
        {
            foreach (InterceptionInfo interception in interceptor.Interceptions)
            {
                foreach (SyntaxTree syntaxTree in compilation.SyntaxTrees)
                {
                    string filePath = RoslynUtilities.GetInterceptorFilePath(syntaxTree, compilation);

                    if (interception.Attribute.FilePath == filePath)
                    {
                        SourceText text = await syntaxTree.GetTextAsync(cancellationToken);

                        TextLine line = text.Lines[interception.Attribute.Line - 1];
                        TextSpan span = new(line.Start + interception.Attribute.Character - 1, 0);

                        SyntaxNode syntaxRoot = await syntaxTree.GetRootAsync(cancellationToken);
                        SyntaxNode node = syntaxRoot.FindNode(span);

                        SemanticModel semanticModel = compilation.GetSemanticModel(syntaxTree);
                        SymbolInfo info = semanticModel.GetSymbolInfo(node, cancellationToken);
                        ISymbol? symbol = info.Symbol;
                        Debug.Assert(symbol is IMethodSymbol, $"Expected {nameof(ISymbol)} '{symbol}' to be of type {nameof(IMethodSymbol)}. Actual type is {symbol.GetType()}.");
                        var method = (IMethodSymbol)symbol;

                        InterceptedCallSiteInfo callSite = new(method.ToDisplayString());
                        interception.Bind(callSite);

                        break;
                    }
                }

                cancellationToken.ThrowIfCancellationRequested();
            }

            cancellationToken.ThrowIfCancellationRequested();
        }

        InterceptorList result = new(project.Name);
        result.AddRange(interceptors);

        return result;
    }
}
