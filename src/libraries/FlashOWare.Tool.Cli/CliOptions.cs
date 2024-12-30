using FlashOWare.Tool.Cli.IO;
using System.CommandLine.Parsing;
using System.Diagnostics;

namespace FlashOWare.Tool.Cli;

internal static class CliOptions
{
    public static Option<FileInfo> Project { get; } = new Option<FileInfo>(["--project", "--proj"], "The path to the project file to operate on (defaults to the current directory if there is exactly one project or solution exclusively).")
        .ExistingOnly();

    public static Option<FileInfo> Solution { get; } = new Option<FileInfo>(["--solution", "--sln"], "The path to the solution (filter) file to operate on (defaults to the current directory if there is exactly one solution or project exclusively).")
        .ExistingOnly();

    public static void AddProjectAndSolutionOptionsTo(Command command)
    {
        command.AddOption(CliOptions.Project);
        command.AddOption(CliOptions.Solution);
    }

    public static ProjectOrSolutionFile GetProjectOrSolutionFileFrom(ParseResult parseResult, IFileSystemAccessor fileSystem)
    {
        ProjectOrSolutionFile? file;

        file = GetProjectOrSolutionFileFrom(parseResult);
        file ??= GetProjectOrSolutionFileFrom(fileSystem);

        return file;
    }

    private static ProjectOrSolutionFile? GetProjectOrSolutionFileFrom(ParseResult parseResult)
    {
        FileInfo? project = parseResult.GetValueForOption(CliOptions.Project);
        FileInfo? solution = parseResult.GetValueForOption(CliOptions.Solution);

        ProjectOrSolutionFile? file;

        if (project is not null)
        {
            if (solution is not null)
            {
                throw new InvalidOperationException("Both 'Project' and 'Solution' are specified. Specify either a 'Project' or a 'Solution', which are mutually exclusive.");
            }

            file = ProjectOrSolutionFile.CreateProject(project);
        }
        else if (solution is not null)
        {
            file = ProjectOrSolutionFile.CreateSolution(solution);
        }
        else
        {
            file = null;
        }

        return file;
    }

    private static ProjectOrSolutionFile GetProjectOrSolutionFileFrom(IFileSystemAccessor fileSystem)
    {
        ProjectOrSolutionFile file;

        FileInfo[] projects = fileSystem.GetProjects();
        FileInfo[] solutions = fileSystem.GetSolutions();

        if (projects.Length > 0)
        {
            if (solutions.Length > 0)
            {
                throw new InvalidOperationException("Specify which project or solution file to use because this folder contains more than one project or solution file.");
            }

            if (projects.Length != 1)
            {
                throw new InvalidOperationException("Specify which project or solution file to use because this folder contains more than one project or solution file.");
            }

            Debug.Assert(projects.Length == 1);
            file = ProjectOrSolutionFile.CreateProject(projects[0]);
        }
        else if (solutions.Length > 0)
        {
            if (solutions.Length != 1)
            {
                throw new InvalidOperationException("Specify which project or solution file to use because this folder contains more than one project or solution file.");
            }

            Debug.Assert(solutions.Length == 1);
            file = ProjectOrSolutionFile.CreateSolution(solutions[0]);
        }
        else
        {
            Debug.Assert(projects.Length == 0 && solutions.Length == 0);
            throw new InvalidOperationException("Specify a project or solution file. The current working directory does not contain a project or solution file.");
        }

        return file;
    }
}
