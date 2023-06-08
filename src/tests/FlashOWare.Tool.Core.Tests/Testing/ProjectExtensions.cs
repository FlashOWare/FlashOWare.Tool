using Microsoft.CodeAnalysis;
using System.Diagnostics;

namespace FlashOWare.Tool.Core.Tests.Testing;

internal static class ProjectExtensions
{
    public static Project WithDocumentName(this Project project, Index index, string name)
    {
        DocumentId documentId = project.DocumentIds[index];

        var solution = project.Solution.WithDocumentName(documentId, name);

        Project? newProject = solution.GetProject(project.Id);
        Debug.Assert(newProject is not null, $"{nameof(ProjectId)} {newProject.Id} is not an id of a project that is part of this solution.");

        return newProject;
    }

    public static async Task<Project> WithDocumentNameAsync(this Task<Project> task, Index index, string name)
    {
        var project = await task;
        return project.WithDocumentName(index, name);
    }
}
