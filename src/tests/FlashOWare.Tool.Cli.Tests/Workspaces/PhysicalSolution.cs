namespace FlashOWare.Tool.Cli.Tests.Workspaces;

internal sealed class PhysicalSolution
{
    private const string FileExtension = "sln";

    public PhysicalSolution(string name)
    {
        Name = name;
    }

    public string Name { get; }

    public string GetFileName()
    {
        return $"{Name}.{FileExtension}";
    }
}
