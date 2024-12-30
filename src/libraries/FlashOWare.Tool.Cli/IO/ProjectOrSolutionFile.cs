using Microsoft.CodeAnalysis;
using System.Diagnostics.CodeAnalysis;

namespace FlashOWare.Tool.Cli.IO;

internal sealed class ProjectOrSolutionFile
{
    public static ProjectOrSolutionFile CreateProject(FileInfo project)
    {
        return new ProjectOrSolutionFile(project, true);
    }

    public static ProjectOrSolutionFile CreateSolution(FileInfo solution)
    {
        return new ProjectOrSolutionFile(solution, false);
    }

    private readonly FileInfo _file;
    private readonly bool _isProject;

    private ProjectOrSolutionFile(FileInfo file, bool isProject)
    {
        _file = file;
        _isProject = isProject;
    }

    public FileInfo File => _file;
    public string FilePath => _file.FullName;

    public bool IsProject => _isProject;
    public bool IsSolution => !_isProject;

    public FileInfo GetProject()
    {
        if (!_isProject)
        {
            throw new InvalidOperationException($"{_file} is a {nameof(Solution)} file, not a {nameof(Project)} file.");
        }

        return _file;
    }

    public FileInfo GetSolution()
    {
        if (_isProject)
        {
            throw new InvalidOperationException($"{_file} is a {nameof(Project)} file, not a {nameof(Solution)} file.");
        }

        return _file;
    }

    public bool TryGetProject([NotNullWhen(true)] out FileInfo? project)
    {
        if (_isProject)
        {
            project = _file;
            return true;
        }

        project = null;
        return false;
    }

    public bool TryGetSolution([NotNullWhen(true)] out FileInfo? solution)
    {
        if (!_isProject)
        {
            solution = _file;
            return true;
        }

        solution = null;
        return false;
    }
}
