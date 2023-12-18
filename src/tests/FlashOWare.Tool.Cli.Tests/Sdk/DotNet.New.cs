using FlashOWare.Tool.Cli.Tests.Diagnostics;
using System.ComponentModel;
using System.Diagnostics;

namespace FlashOWare.Tool.Cli.Tests.Sdk;

public partial class DotNet
{
    internal Task<string> NewAsync(DotNetNewTemplate template)
    {
        return template switch
        {
            DotNetNewTemplate.Unspecified => throw new InvalidOperationException("Template not specified."),
            DotNetNewTemplate.AspNetCoreWebApiNativeAot => NewAspNetCoreWebApiNativeAotAsync(),
            _ => throw new InvalidEnumArgumentException(nameof(template), (int)template, typeof(DotNetNewTemplate)),
        };
    }

    internal async Task<string> NewAspNetCoreWebApiNativeAotAsync()
    {
        string project = "WebApiAotProject";

        using Process process = StartProcess("dotnet", "new", "webapiaot", "--name", project, "--output", _directory.FullName, "--language", "C#", "--framework", "net8.0", "--exclude-launch-settings");

        await process.WaitForSuccessfulExitAsync(TimeSpan.FromSeconds(15));

        return project;
    }
}
