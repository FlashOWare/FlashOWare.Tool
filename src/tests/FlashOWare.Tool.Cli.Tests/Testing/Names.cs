namespace FlashOWare.Tool.Cli.Tests.Testing;

internal static class Names
{
    public const string Project = "TestProject";
    public const string Properties = "Properties";

    public const string CSharpProject = "TestProject.csproj";
    public const string VisualBasicProject = "TestProject.vbproj";

    public const string CSharpFileExtension = "cs";
    public const string CSharpProjectExtension = "csproj";

    public const string VisualBasicFileExtension = "vb";
    public const string VisualBasicProjectExtension = "vbproj";

    public static string CSharpDocument(string name)
    {
        return $"{name}.{CSharpFileExtension}";
    }

    public static string VisualBasicDocument(string name)
    {
        return $"{name}.{VisualBasicFileExtension}";
    }
}
