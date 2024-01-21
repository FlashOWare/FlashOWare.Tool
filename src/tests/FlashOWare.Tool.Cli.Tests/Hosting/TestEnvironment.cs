namespace FlashOWare.Tool.Cli.Tests.Hosting;

internal static class TestEnvironment
{
    private static readonly Lazy<bool> s_isContinuousIntegration = new(IsContinuousIntegrationCore);

    public static bool IsContinuousIntegration => s_isContinuousIntegration.Value;

    private static bool IsContinuousIntegrationCore()
    {
        string? value = Environment.GetEnvironmentVariable(EnvironmentVariables.GitHubActions);
        return value == bool.TrueString;
    }
}
