using FlashOWare.Tool.Core.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Diagnostics;

namespace FlashOWare.Tool.Core.UsingDirectives;

public static class UsingGlobalizer
{
    private const string DefaultTargetDocument = "GlobalUsings.cs";

    public static async Task<UsingGlobalizationResult> GlobalizeAsync(Project project, string localUsing, CancellationToken cancellationToken = default)
    {
        RoslynUtilities.ThrowIfNotCSharp(project, LanguageVersion.CSharp10, LanguageFeatures.GlobalUsingDirective);

        Compilation? compilation = await project.GetCompilationAsync(cancellationToken);
        if (compilation is null)
        {
            throw new InvalidOperationException($"{nameof(Project)}.{nameof(Project.SupportsCompilation)} = {project.SupportsCompilation} ({project.Name})");
        }

        var usingDirective = new UsingDirective(localUsing);

        if (RoslynUtilities.IsGeneratedCode(compilation))
        {
            return new UsingGlobalizationResult(project, usingDirective, DefaultTargetDocument);
        }

        var solution = project.Solution;

        foreach (Document document in project.Documents)
        {
            if (RoslynUtilities.IsGeneratedCode(document))
            {
                continue;
            }

            SyntaxNode? syntaxRoot = await document.GetSyntaxRootAsync(cancellationToken);
            if (syntaxRoot is null)
            {
                throw new InvalidOperationException($"{nameof(Document)}.{nameof(Document.SupportsSyntaxTree)} = {document.SupportsSyntaxTree} ({document.Name})");
            }

            var compilationUnit = (CompilationUnitSyntax)syntaxRoot;

            RoslynUtilities.ThrowIfContainsError(compilationUnit);

            if (RoslynUtilities.IsGeneratedCode(compilationUnit))
            {
                continue;
            }

            cancellationToken.ThrowIfCancellationRequested();

            solution = await GlobalizeAsync(solution, project.Id, document, compilationUnit, localUsing, usingDirective, cancellationToken);
        }

        var newProject = solution.GetProject(project.Id);
        Debug.Assert(newProject is not null, $"{nameof(ProjectId)} is not a {nameof(ProjectId)} of a {nameof(Project)} that is part of this {nameof(Solution)}.");
        return new UsingGlobalizationResult(newProject, usingDirective, DefaultTargetDocument);
    }

    private static async Task<Solution> GlobalizeAsync(Solution solution, ProjectId projectId, Document document, CompilationUnitSyntax compilationUnit, string localUsing, UsingDirective result, CancellationToken cancellationToken)
    {
        var options = await document.GetOptionsAsync(cancellationToken);

        foreach (UsingDirectiveSyntax usingNode in compilationUnit.Usings)
        {
            if (usingNode.Alias is not null ||
                !usingNode.StaticKeyword.IsKind(SyntaxKind.None) ||
                !usingNode.GlobalKeyword.IsKind(SyntaxKind.None))
            {
                continue;
            }

            if (usingNode.Name.ToString() == localUsing)
            {
                var newRoot = compilationUnit.RemoveNode(usingNode, SyntaxRemoveOptions.KeepLeadingTrivia);
                Debug.Assert(newRoot is not null, "The root node itself is removed.");

                solution = solution.WithDocumentSyntaxRoot(document.Id, newRoot);

                Project? project = solution.GetProject(projectId);
                Debug.Assert(project is not null, $"{nameof(ProjectId)} is not a {nameof(ProjectId)} of a {nameof(Project)} that is part of this {nameof(Solution)}.");

                if (project.Documents.SingleOrDefault(static document => document.Name == DefaultTargetDocument && document.Folders.Count == 0) is { } globalUsings)
                {
                    SyntaxNode? globalUsingsSyntaxRoot = await globalUsings.GetSyntaxRootAsync(cancellationToken);
                    if (globalUsingsSyntaxRoot is null)
                    {
                        throw new InvalidOperationException($"{nameof(Document)}.{nameof(Document.SupportsSyntaxTree)} = {globalUsings.SupportsSyntaxTree} ({globalUsings.Name})");
                    }

                    var globalUsingsCompilationUnit = (CompilationUnitSyntax)globalUsingsSyntaxRoot;
                    if (!globalUsingsCompilationUnit.Usings.Any(usingDirective => usingDirective.Name.ToString() == localUsing))
                    {
                        var node = CSharpSyntaxFactory.GlobalUsingDirective(localUsing, options);
                        var newUsings = globalUsingsCompilationUnit.Usings.Add(node);
                        var newGlobalUsingsRoot = globalUsingsCompilationUnit.WithUsings(newUsings);
                        solution = solution.WithDocumentSyntaxRoot(globalUsings.Id, newGlobalUsingsRoot);
                    }
                }
                else
                {
                    var syntaxRoot = CSharpSyntaxFactory.GlobalUsingDirectiveRoot(localUsing, options);
                    solution = solution.AddDocument(DocumentId.CreateNewId(projectId), DefaultTargetDocument, syntaxRoot);
                }

                result.Occurrences++;
            }
        }

        return solution;
    }
}
