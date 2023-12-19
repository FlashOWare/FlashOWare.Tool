using System.Diagnostics;

namespace FlashOWare.Tool.Cli.Tests.Diagnostics;

internal static class ProcessExtensions
{
    public static async Task WaitForSuccessfulExitAsync(this Process process, TimeSpan timeout)
    {
        using CancellationTokenSource cts = new(timeout);

        try
        {
            await process.WaitForExitAsync(cts.Token);
        }
        catch (TaskCanceledException tce)
        {
            process.Kill();
            await process.WaitForExitAsync(CancellationToken.None);
            throw new TimeoutException($"The operation has timed out after {timeout}.", tce);
        }
        catch (OperationCanceledException oce)
        {
            process.Kill();
            await process.WaitForExitAsync(CancellationToken.None);
            throw new TimeoutException($"The operation has timed out after {timeout}.", oce);
        }

        Debug.Assert(process.HasExited, "The process has not terminated.");

        if (process.ExitCode != 0)
        {
            string output = await process.StandardOutput.ReadToEndAsync(cts.Token);
            throw new InvalidOperationException($"{nameof(Process.ExitCode)}: {process.ExitCode}{Environment.NewLine}{output}");
        }
    }
}
