using FlashOWare.Tool.Cli.Tests.CodeAnalysis;
using FlashOWare.Tool.Cli.Tests.Testing;
using Microsoft.CodeAnalysis.CSharp;
using NuGet.Packaging;
using System.ComponentModel;
using System.Diagnostics;

namespace FlashOWare.Tool.Cli.Tests.Workspaces;

internal sealed class PhysicalProjectBuilder
{
    private readonly List<PhysicalDocument> _documents = [];
    private readonly List<PackageReference> _packages = [];
    private string _projectName;

    private readonly DirectoryInfo _directory;
    private readonly Language _language;

    private int _count;

    public PhysicalProjectBuilder(DirectoryInfo directory, Language language)
    {
        if (!Enum.IsDefined(language))
        {
            throw new InvalidEnumArgumentException(nameof(language), (int)language, typeof(Language));
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

    public PhysicalProjectBuilder AddPackage(PackageReference package)
    {
        _packages.Add(package);
        return this;
    }

    public PhysicalProject Initialize(ProjectKind kind, TargetFramework tfm, LanguageVersion? langVersion = null)
    {
        if (!Enum.IsDefined(kind))
        {
            throw new InvalidEnumArgumentException(nameof(kind), (int)kind, typeof(ProjectKind));
        }

        foreach (PhysicalDocument document in _documents)
        {
            document.Write();
        }

        PhysicalProject project = PhysicalProject.Create(_directory, _projectName, _language);

        string[] files = kind is ProjectKind.Classic
            ? _documents.Select((PhysicalDocument document) => Path.GetRelativePath(_directory.FullName, document.FullName)).ToArray()
            : Array.Empty<string>();

        if (_language is Language.CSharp)
        {
            string projectText = kind is ProjectKind.Classic
                ? ProjectText.CreateNonSdk(tfm, langVersion.DefaultIfNull(tfm), files, _packages)
                : ProjectText.Create(tfm, langVersion, _packages);

            project.Write(projectText);
        }
        else
        {
            Debug.Assert(_language is Language.VisualBasic, $"Expected: {Language.VisualBasic}, Actual: {_language}");

            string projectText = kind is ProjectKind.Classic
                ? throw new NotImplementedException($"{Language.VisualBasic} non-SDK .NET Framework project is not implemented.")
                : ProjectText.CreateVisualBasic(tfm, langVersion.HasValue ? throw new NotImplementedException($"{Language.VisualBasic} {nameof(Microsoft.CodeAnalysis.VisualBasic.LanguageVersion)} is not implemented.") : null, _packages);

            project.Write(projectText);
        }

        return project;
    }

    private string CreateName()
    {
        int incremented = Interlocked.Increment(ref _count);
        return $"Test{incremented}";
    }
}
