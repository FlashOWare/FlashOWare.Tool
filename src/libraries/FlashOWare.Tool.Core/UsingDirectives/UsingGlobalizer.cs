using FlashOWare.Tool.Core.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Diagnostics;

namespace FlashOWare.Tool.Core.UsingDirectives;

public static class UsingGlobalizer
{
    public static async Task<Project> GlobalizeAsync(Project project, string localUsing, CancellationToken cancellationToken = default)
    {
        var solution = project.Solution;

        foreach (Document document in project.Documents)
        {
            SyntaxNode? syntaxRoot = await document.GetSyntaxRootAsync();
            if (syntaxRoot is null)
            {
                throw new InvalidOperationException($"{nameof(Document)}.{nameof(Document.SupportsSyntaxTree)} = {document.SupportsSyntaxTree} ({document.Name})");
            }

            var compilationUnit = (CompilationUnitSyntax)syntaxRoot;

            solution = await GlobalizeAsync(solution, project, document, compilationUnit, localUsing);
        }

        var newProject = solution.GetProject(project.Id);
        Debug.Assert(newProject is not null, $"{nameof(ProjectId)} is not an {nameof(ProjectId)} of a {nameof(Project)} that is part of this {nameof(Solution)}.");
        return newProject;
    }

    private static async Task<Solution> GlobalizeAsync(Solution solution, Project project, Document document, CompilationUnitSyntax compilationUnit, string localUsing)
    {
        var options = await document.GetOptionsAsync();

        foreach (UsingDirectiveSyntax usingNode in compilationUnit.Usings)
        {
            if (usingNode.Name.ToString() == localUsing)
            {
                var newRoot = compilationUnit.RemoveNode(usingNode, SyntaxRemoveOptions.KeepNoTrivia);
                Debug.Assert(newRoot is not null, "The root node itself is removed.");

                solution = solution.WithDocumentSyntaxRoot(document.Id, newRoot);

                var syntaxRoot = CSharpSyntaxFactory.GlobalUsingDirectiveRoot(localUsing, options);
                solution = solution.AddDocument(DocumentId.CreateNewId(project.Id), "GlobalUsings.cs", syntaxRoot, new string[] { "Properties" });
            }
        }

        return solution;
    }
}
