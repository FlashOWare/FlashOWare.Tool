using FlashOWare.Tool.Core.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Diagnostics;

namespace FlashOWare.Tool.Core.UsingDirectives;

//TODO: Initial Release Version (MVP)
//create new "GlobalUsings.cs" if not existing
//- Directory/Name?
//  - in "Properties" directory?
//    - e.g. .NET Framework: <Compile Include="Properties\AssemblyInfo.cs" />
//  - or in "root"
//    - e.g. .NET 7 "xUnit Test Project" template: "\Usings.cs"
//- File-Name?
//  - e.g. from .NET 7 "xUnit" template: ".\Usings.cs"
//  - e.g. from ImplicitUsings: "\obj\Debug\net7.0\MyProject.GlobalUsings.g.cs"
//Append if exists

//top-level usings only, no nested usings within namespaces
//no using alias (neither type nor namespace alias)
//no using static
//no global usings
//Ignore auto-generated documents and auto-generated code
//check C# project and LangVersion 10 or greater
//support cancellation via CancellationToken
//check no compiler errors

public static class UsingGlobalizer
{
    public static async Task<Project> GlobalizeAsync(Project project, string localUsing)
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
