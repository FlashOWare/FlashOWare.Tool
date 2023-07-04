using Microsoft.CodeAnalysis;
using System.Diagnostics.CodeAnalysis;

namespace FlashOWare.Tool.Core.UsingDirectives;

public sealed class UsingGlobalizationResult
{
    internal UsingGlobalizationResult()
    {
    }

    [SetsRequiredMembers]
    internal UsingGlobalizationResult(Project project, UsingDirective @using, string targetDocument)
    {
        Project = project;
        Using = @using;
        TargetDocument = targetDocument;
    }

    public required Project Project { get; init; }
    public required UsingDirective Using { get; init; }
    public required string TargetDocument { get; init; }
}
