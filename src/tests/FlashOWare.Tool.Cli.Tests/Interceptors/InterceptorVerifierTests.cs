using FlashOWare.Tool.Cli.Tests.CommandLine.IO;
using FlashOWare.Tool.Cli.Tests.Testing;

namespace FlashOWare.Tool.Cli.Tests.Interceptors;

public class InterceptorVerifierTests : IntegrationTests
{
    [Fact]
    public async Task Check_SdkVersion_SupportedOrNotSupported()
    {
        //Arrange
        Version current = MSBuild.Instance.Version;
        //Act
        await RunAsync("interceptor", "check");
        //Assert
        if (MSBuild.IsGreaterThanOrEqual(8, 0, 100))
        {
            Console.VerifyContains($"> {current} supports the 'interceptors' experimental feature by adding '<InterceptorsPreviewNamespaces>$(InterceptorsPreviewNamespaces);MyNamespace</InterceptorsPreviewNamespaces>' to your project.");
        }
        else if (MSBuild.IsGreaterThanOrEqual(7, 0, 400))
        {
            Console.VerifyContains($"> {current} supports the 'interceptors' experimental feature by adding '<Features>InterceptorsPreview</Features>' to your project.");
        }
        else
        {
            Console.VerifyContains($"> {current} does not support the 'interceptors' experimental feature.");
        }
        Console.VerifyEndsWithOutput($"Current: {current}");
        Result.Verify(ExitCodes.Success);
    }
}
