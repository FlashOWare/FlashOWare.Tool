using Basic.Reference.Assemblies;
using FlashOWare.Tool.Core.Tests.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Diagnostics;

namespace FlashOWare.Tool.Core.Tests.Testing;

internal sealed partial class ProjectBuilder
{
    private readonly Workspace _workspace;
    private Solution _solution;
    private readonly ProjectId _projectId;

    private readonly CompilationOptions _compilationOptions;
    private ParseOptions _parseOptions;

    private ProjectBuilder(string language, CompilationOptions compilationOptions, ParseOptions parseOptions)
    {
        _workspace = new AdhocWorkspace();
        _solution = _workspace.CurrentSolution;
        _projectId = ProjectId.CreateNewId("Test-Project-Id");

        _compilationOptions = compilationOptions;
        _parseOptions = parseOptions;

        _solution = _solution.AddProject(_projectId, "TestProject", "TestAssembly", language);
    }

    public ProjectBuilder AddDocument(string name, string text)
    {
        var documentId = DocumentId.CreateNewId(_projectId, $"Test-Document-Id: {name}");
        _solution = _solution.AddDocument(documentId, name, text);
        return this;
    }

    public ProjectBuilder AddDocument(string name, string text, params string[] folders)
    {
        string directoryPath = folders.Aggregate(String.Empty, static (accumulate, folder) => Path.Combine(accumulate, folder));
        string filePath = Path.Combine(directoryPath, name);
        var documentId = DocumentId.CreateNewId(_projectId, $"Test-Document-Id: {name}");
        _solution = _solution.AddDocument(documentId, name, text, folders, filePath);
        return this;
    }

    public ProjectBuilder AddAnalyzer<TAnalyzer>()
        where TAnalyzer : DiagnosticAnalyzer, new()
    {
        DiagnosticAnalyzer analyzer = new TAnalyzer();
        AnalyzerReference analyzerReference = new AdhocAnalyzerReference(analyzer);

        _solution = _solution.AddAnalyzerReference(_projectId, analyzerReference);
        return this;
    }

    public ProjectBuilder AddGenerator<TGenerator>()
        where TGenerator : IIncrementalGenerator, new()
    {
        IIncrementalGenerator generator = new TGenerator();
        AnalyzerReference analyzerReference = new AdhocAnalyzerReference(generator);

        _solution = _solution.AddAnalyzerReference(_projectId, analyzerReference);
        return this;
    }

    public ProjectBuilder WithFeature(string key, string value)
    {
        Dictionary<string, string> features = new(1)
        {
            { key, value },
        };
        _parseOptions = _parseOptions.WithFeatures(features);
        return this;
    }

    public Project BuildUnchecked()
    {
        Project? project = _solution.GetProject(_projectId);
        Debug.Assert(project is not null, $"{nameof(ProjectId)} {_projectId} is not an id of a project that is part of this solution.");
        project = project.AddMetadataReferences(ReferenceAssemblies.Net60);
        project = project.WithCompilationOptions(_compilationOptions);
        project = project.WithParseOptions(_parseOptions);

        _workspace.Dispose();
        return project;
    }

    public async Task<Project> BuildCheckedAsync()
    {
        Project project = BuildUnchecked();
        await RoslynFactory.CheckAsync(project);
        return project;
    }
}
