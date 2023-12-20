using Basic.Reference.Assemblies;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Diagnostics;
using TextFile = (string Name, string Text);

namespace FlashOWare.Tool.Core.Tests.Testing;

internal static partial class CSharpFactory
{
    public static Project CreateProjectUnchecked()
    {
        return CreateProjectUnchecked(Array.Empty<string>());
    }

    public static Task<Project> CreateProjectCheckedAsync()
    {
        return CreateProjectCheckedAsync(Array.Empty<string>());
    }

    public static Project CreateProjectUnchecked(params string[] documents)
    {
        int index = 0;
        return CreateProjectUnchecked(documents.Select(text => ($"TestDocument{index++}.cs", text)).ToArray());
    }

    public static Task<Project> CreateProjectCheckedAsync(params string[] documents)
    {
        int index = 0;
        return CreateProjectCheckedAsync(documents.Select(text => ($"TestDocument{index++}.cs", text)).ToArray());
    }

    public static Project CreateProjectUnchecked(params TextFile[] documents)
    {
        using var workspace = new AdhocWorkspace();
        var solution = workspace.CurrentSolution;

        var projectId = ProjectId.CreateNewId("Test-Project-Id");
        solution = solution.AddProject(projectId, "TestProject", "TestAssembly", LanguageNames.CSharp);

        foreach (TextFile document in documents)
        {
            var documentId = DocumentId.CreateNewId(projectId, $"Test-Document-Id: {document.Name}");
            solution = solution.AddDocument(documentId, document.Name, document.Text);
        }

        Project? project = solution.GetProject(projectId);
        Debug.Assert(project is not null, $"{nameof(ProjectId)} {projectId} is not an id of a project that is part of this solution.");
        project = project.AddMetadataReferences(ReferenceAssemblies.Net60);
        project = project.WithCompilationOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
        project = project.WithParseOptions(new CSharpParseOptions(LanguageVersion.CSharp10));

        return project;
    }

    public static async Task<Project> CreateProjectCheckedAsync(params TextFile[] documents)
    {
        Project project = CreateProjectUnchecked(documents);
        await RoslynFactory.CheckAsync(project);
        return project;
    }
}

internal static partial class CSharpFactory
{
    public static Compilation CreateCompilationUnchecked(params string[] documents)
    {
        var parseOptions = new CSharpParseOptions(LanguageVersion.CSharp10);
        IEnumerable<SyntaxTree> syntaxTrees = documents.Select(document => CSharpSyntaxTree.ParseText(document, parseOptions));
        IEnumerable<MetadataReference> references = ReferenceAssemblies.Net60;
        var compilationOptions = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);

        var compilation = CSharpCompilation.Create("AssemblyName", syntaxTrees, references, compilationOptions);
        return compilation;
    }

    public static Compilation CreateCompilationChecked(params string[] documents)
    {
        var compilation = CreateCompilationUnchecked(documents);
        RoslynFactory.Check(compilation);
        return compilation;
    }
}

internal static partial class CSharpFactory
{
    public static Document CreateDocumentUnchecked(string name, string text)
    {
        using var workspace = new AdhocWorkspace();

        var project = workspace.AddProject("TestProject", LanguageNames.CSharp);
        project = project.AddMetadataReferences(ReferenceAssemblies.Net60);
        project = project.WithCompilationOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
        project = project.WithParseOptions(new CSharpParseOptions(LanguageVersion.CSharp10));

        var document = project.AddDocument(name, text);
        return document;
    }

    public static async Task<Document> CreateDocumentCheckedAsync(string name, string text)
    {
        var document = CreateDocumentUnchecked(name, text);
        await RoslynFactory.CheckAsync(document.Project);
        return document;
    }
}

internal static partial class CSharpFactory
{
    public static CompilationUnitSyntax CreateSyntaxRootUnchecked(string text)
    {
        var options = new CSharpParseOptions(LanguageVersion.CSharp10);

        SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(text, options);

        Debug.Assert(syntaxTree.HasCompilationUnitRoot, $"{nameof(SyntaxTree)} doesn't have a root with {nameof(SyntaxKind)} {SyntaxKind.CompilationUnit}: {syntaxTree}");
        var root = syntaxTree.GetCompilationUnitRoot();
        return root;
    }

    public static CompilationUnitSyntax CreateSyntaxRootChecked(string text)
    {
        var root = CreateSyntaxRootUnchecked(text);
        RoslynFactory.Check(root);
        return root;
    }
}
