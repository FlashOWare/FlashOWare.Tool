using Microsoft.CodeAnalysis;

namespace FlashOWare.Tool.Core.CodeAnalysis;

public partial class RoslynUtilities
{
    public static void ThrowIfNotCSharp(Project project)
    {
        if (project.Language != LanguageNames.CSharp)
        {
            string message = $"{nameof(Project.Language)} {project.Language} is not supported.";
            throw new InvalidOperationException(message);
        }
    }
}
