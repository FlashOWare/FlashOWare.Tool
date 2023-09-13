namespace FlashOWare.Tool.Cli.Tests.Workspaces;

internal sealed class ProjectOptions
{
    private ProjectOptions()
    {
        Name = "TestProject";
        Properties = "Properties";
        RootNamespace = "TestProject";
        AssemblyName = "TestProject";
        ProjectGuid = Guid.NewGuid();
        AssemblyGuid = Guid.NewGuid();
    }

    public static ProjectOptions Default { get; } = new ProjectOptions();

    public string Name { get; }
    public string Properties { get; }
    public string RootNamespace { get; }
    public string AssemblyName { get; }
    public Guid ProjectGuid { get; }
    public Guid AssemblyGuid { get; }
}
