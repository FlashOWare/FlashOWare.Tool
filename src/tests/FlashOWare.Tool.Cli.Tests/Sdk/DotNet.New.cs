using FlashOWare.Tool.Cli.Tests.Diagnostics;
using System.ComponentModel;
using System.Diagnostics;

namespace FlashOWare.Tool.Cli.Tests.Sdk;

public partial class DotNet
{
    private static readonly DotNetCliOptions s_noRestore = new() { NoRestore = true };

    internal Task<string> NewAsync(DotNetNewTemplate template)
    {
        return NewAsync(template, false);
    }

    internal Task<string> NewAsync(DotNetNewTemplate template, bool norestore)
    {
        DotNetCliOptions options = norestore ? s_noRestore : DotNetCliOptions.None;

        return template switch
        {
            DotNetNewTemplate.Unspecified => throw new InvalidOperationException("Template not specified."),
            DotNetNewTemplate.AspNetCoreWebApiNativeAot => NewAspNetCoreWebApiNativeAotAsync(options),
            _ => throw new InvalidEnumArgumentException(nameof(template), (int)template, typeof(DotNetNewTemplate)),
        };
    }

    private async Task<string> NewAspNetCoreWebApiNativeAotAsync(DotNetCliOptions options)
    {
        string project = "WebApiAotProject";

        using Process process = StartProcess("dotnet", ["new", "webapiaot", "--name", project, "--output", _directory.FullName, "--language", "C#", "--framework", "net8.0", "--exclude-launch-settings"], options);

        await process.WaitForSuccessfulExitAsync(TimeSpan.FromSeconds(15));

        return project;
    }
}
