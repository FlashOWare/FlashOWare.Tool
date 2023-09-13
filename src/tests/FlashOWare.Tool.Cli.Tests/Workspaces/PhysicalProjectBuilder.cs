using FlashOWare.Tool.Cli.Tests.Testing;
using Microsoft.CodeAnalysis.CSharp;

namespace FlashOWare.Tool.Cli.Tests.Workspaces;

internal sealed class PhysicalProjectBuilder
{
    private readonly List<PhysicalDocument> _documents = new();
    private string _projectName;

    private readonly DirectoryInfo _directory;
    private readonly Language _language;

    private int _count;

    public PhysicalProjectBuilder(DirectoryInfo directory, Language language)
    {
        if (language is Language.VisualBasic)
        {
            throw new NotSupportedException("Visual Basic is not supported.");
        }
        if (language is Language.FSharp)
        {
            throw new NotSupportedException("F# is not a supported compile target for the Roslyn compiler.");
        }

        _directory = directory;
        _language = language;

        _projectName = ProjectOptions.Default.Name;
    }

    public PhysicalProjectBuilder WithProjectName(string name)
    {
        _projectName = name;
        return this;
    }

    public PhysicalProjectBuilder AddDocument(string text)
    {
        string name = CreateName();
        PhysicalDocument document = PhysicalDocument.Create(text, _directory, name, _language);
        _documents.Add(document);
        return this;
    }

    public PhysicalProjectBuilder AddDocument(string text, string name)
    {
        PhysicalDocument document = PhysicalDocument.Create(text, _directory, name, _language);
        _documents.Add(document);
        return this;
    }

    public PhysicalProjectBuilder AddDocument(string text, string name, params string[] folders)
    {
        PhysicalDocument document = PhysicalDocument.Create(text, _directory, name, folders, _language);
        _documents.Add(document);
        return this;
    }

    public PhysicalProject Initialize(ProjectKind kind, TargetFramework tfm, LanguageVersion langVersion)
    {
        foreach (PhysicalDocument document in _documents)
        {
            Directory.CreateDirectory(document.Directory);
            File.WriteAllText(document.FullName, document.Text);
        }

        PhysicalProject project = PhysicalProject.Create(_directory, _projectName, _language);

        string[] files = kind is ProjectKind.Classic
            ? _documents.Select((PhysicalDocument document) => Path.GetRelativePath(_directory.FullName, document.FullName)).ToArray()
            : Array.Empty<string>();

        File.WriteAllText(project.File.FullName, Projects.CreateProject(kind, tfm, langVersion, files));

        return project;
    }

    private string CreateName()
    {
        int incremented = Interlocked.Increment(ref _count);
        return $"Test{incremented}";
    }
}
