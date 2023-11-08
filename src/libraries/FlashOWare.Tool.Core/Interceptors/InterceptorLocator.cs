using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace FlashOWare.Tool.Core.Interceptors;

public static class InterceptorLocator
{
    //TODO
    // display which methods are intercepted
    // display by which method these are intercepted

    public static async Task<InterceptorList> ListAsync(Project project, CancellationToken cancellationToken = default)
    {
        StringBuilder result = new();

        Dictionary<string, List<InterceptorInfo>> interceptors = await FindInterceptorsAsync(project, cancellationToken);

        foreach (var item in interceptors.Values.SelectMany(static list => list))
        {
            result.AppendLine(item.ToString());
        }

        //TODO: naive
        foreach (Document document in project.Documents)
        {
            if (!interceptors.TryGetValue(document.FilePath, out List<InterceptorInfo>? list))
            {
                continue;
            }

            foreach (InterceptorInfo interceptor in list)
            {
                SyntaxNode? syntaxRoot = await document.GetSyntaxRootAsync();

                SourceText text = await document.GetTextAsync();
                TextLine line = text.Lines[interceptor.Line - 1];

                TextSpan span = new(line.Start + interceptor.Character - 1, 0);
                SyntaxNode node = syntaxRoot.FindNode(span);

                result.AppendLine(node.ToString());
            }
        }

        return new InterceptorList(result.ToString());
    }

    private static async Task<Dictionary<string, List<InterceptorInfo>>> FindInterceptorsAsync(Project project, CancellationToken cancellationToken)
    {
        Dictionary<string, List<InterceptorInfo>> interceptors = new();

        foreach (Document document in project.Documents)
        {
            //TODO: defensive ... does the path actually exist?
            if (Path.GetExtension(document.FilePath).ToLower() != ".cs")
            {
                continue;
            }

            SemanticModel? semanticModel = await document.GetSemanticModelAsync();
            InterceptorWalker walker = new(semanticModel);
            SyntaxNode? syntaxRoot = await document.GetSyntaxRootAsync();
            var compilationUnit = (CompilationUnitSyntax)syntaxRoot;
            walker.VisitCompilationUnit(compilationUnit);

            foreach (InterceptorInfo interceptor in walker.Interceptors)
            {
                //TODO: Linq-ify
                if (interceptors.TryGetValue(interceptor.FilePath, out List<InterceptorInfo>? list))
                {
                    list.Add(interceptor);
                }
                else
                {
                    list = new List<InterceptorInfo>
                    {
                        interceptor
                    };
                    interceptors.Add(interceptor.FilePath, list);
                }
            }

            //interceptors.Add(document.FilePath!, );
        }

        return interceptors;
    }
}
