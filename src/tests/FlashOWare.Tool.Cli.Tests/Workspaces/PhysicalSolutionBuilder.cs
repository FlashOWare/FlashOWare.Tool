using FlashOWare.Tool.Cli.Tests.Sdk;
using FlashOWare.Tool.Cli.Tests.Testing;
using Microsoft.CodeAnalysis.CSharp;
using System.Diagnostics;

namespace FlashOWare.Tool.Cli.Tests.Workspaces;

internal sealed class PhysicalSolutionBuilder
{
    private readonly List<Action<PhysicalProjectBuilder>> _actions = [];
    private readonly List<(Language Language, ProjectKind Kind, TargetFramework Tfm, LanguageVersion? LangVersion)> _options = [];
    private string _solutionName;

    private readonly DirectoryInfo _directory;
    private readonly DotNet _dotnet;

    public PhysicalSolutionBuilder(DirectoryInfo directory)
    {
        _directory = directory;
        _dotnet = new DotNet(directory);

        _solutionName = SolutionOptions.Default.Name;
    }

    public PhysicalSolutionBuilder WithSolutionName(string name)
    {
        _solutionName = name;
        return this;
    }

    public PhysicalSolutionBuilder AddCSharpProject(ProjectKind kind, TargetFramework tfm, Action<PhysicalProjectBuilder> action)
    {
        _actions.Add(action);
        _options.Add((Language.CSharp, kind, tfm, null));
        return this;
    }

    public PhysicalSolutionBuilder AddCSharpProject(ProjectKind kind, TargetFramework tfm, LanguageVersion langVersion, Action<PhysicalProjectBuilder> action)
    {
        _actions.Add(action);
        _options.Add((Language.CSharp, kind, tfm, langVersion));
        return this;
    }

    public PhysicalSolutionBuilder AddVisualBasicProject(ProjectKind kind, TargetFramework tfm, Action<PhysicalProjectBuilder> action)
    {
        _actions.Add(action);
        _options.Add((Language.VisualBasic, kind, tfm, null));
        return this;
    }

    public async Task<PhysicalSolution> InitializeAsync()
    {
        string solutionFile = await _dotnet.NewAsync(DotNetNewTemplate.SolutionFile, _solutionName);
        PhysicalSolution solution = PhysicalSolution.Create(_directory, _solutionName);

        Debug.Assert(_actions.Count == _options.Count);

        for (int index = 0; index < _actions.Count; index++)
        {
            var action = _actions[index];
            var option = _options[index];

            string projectName = $"{ProjectOptions.Default.Name}{index}";

            DirectoryInfo projectDirectory = _directory.CreateSubdirectory(projectName);

            var builder = new PhysicalProjectBuilder(projectDirectory, option.Language)
                .WithProjectName(projectName);
            action.Invoke(builder);
            builder.Initialize(option.Kind, option.Tfm, option.LangVersion);

            await _dotnet.SlnAddAsync(solution.File, projectName);
        }

        return solution;
    }
}
