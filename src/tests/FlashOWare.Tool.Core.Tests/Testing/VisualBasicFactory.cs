using Basic.Reference.Assemblies;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using System.Diagnostics;

namespace FlashOWare.Tool.Core.Tests.Testing;

internal static partial class VisualBasicFactory
{
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

    public static Project CreateProjectUnchecked(params (string Name, string Text)[] documents)
    {
        using var workspace = new AdhocWorkspace();
        var solution = workspace.CurrentSolution;

        var projectId = ProjectId.CreateNewId();
        solution = solution.AddProject(projectId, "TestProject", "TestAssembly", LanguageNames.VisualBasic);

        foreach (var document in documents)
        {
            var documentId = DocumentId.CreateNewId(projectId);
            solution = solution.AddDocument(documentId, document.Name, document.Text);
        }

        Project? project = solution.GetProject(projectId);
        Debug.Assert(project is not null, $"{nameof(ProjectId)} {projectId} is not an id of a project that is part of this solution.");
        project = project.AddMetadataReferences(ReferenceAssemblies.Net60);
        project = project.WithCompilationOptions(new VisualBasicCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
        project = project.WithParseOptions(new VisualBasicParseOptions(LanguageVersion.VisualBasic16_9));

        return project;
    }

    public static async Task<Project> CreateProjectCheckedAsync(params (string Name, string Text)[] documents)
    {
        Project project = CreateProjectUnchecked(documents);
        await RoslynFactory.CheckAsync(project);
        return project;
    }
}

internal static partial class VisualBasicFactory
{
    public static Compilation CreateCompilationUnchecked(params string[] documents)
    {
        var parseOptions = new VisualBasicParseOptions(LanguageVersion.VisualBasic16_9);
        IEnumerable<SyntaxTree> syntaxTrees = documents.Select(document => VisualBasicSyntaxTree.ParseText(document, parseOptions));
        IEnumerable<MetadataReference> references = ReferenceAssemblies.Net60;
        var compilationOptions = new VisualBasicCompilationOptions(OutputKind.DynamicallyLinkedLibrary);

        var compilation = VisualBasicCompilation.Create("AssemblyName", syntaxTrees, references, compilationOptions);
        return compilation;
    }

    public static Compilation CreateCompilationChecked(params string[] documents)
    {
        var compilation = CreateCompilationUnchecked(documents);
        RoslynFactory.Check(compilation);
        return compilation;
    }
}

internal static partial class VisualBasicFactory
{
    public static Document CreateDocumentUnchecked(string name, string text)
    {
        using var workspace = new AdhocWorkspace();

        var project = workspace.AddProject("TestProject", LanguageNames.VisualBasic);
        project = project.AddMetadataReferences(ReferenceAssemblies.Net60);
        project = project.WithCompilationOptions(new VisualBasicCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
        project = project.WithParseOptions(new VisualBasicParseOptions(LanguageVersion.VisualBasic16_9));

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

internal static partial class VisualBasicFactory
{
    public static CompilationUnitSyntax CreateSyntaxRootUnchecked(string text)
    {
        var options = new VisualBasicParseOptions(LanguageVersion.VisualBasic16_9);

        SyntaxTree syntaxTree = VisualBasicSyntaxTree.ParseText(text, options);

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
