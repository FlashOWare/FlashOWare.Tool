namespace FlashOWare.Tool.Cli.Tests.Workspaces;

internal sealed class SolutionOptions
{
    public static SolutionOptions Default { get; } = new SolutionOptions();

    private SolutionOptions()
    {
        Name = "TestSolution";
    }

    public string Name { get; }
}
