using System;
using System.Linq;
using Nuke.Common;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

[GitHubActions(
    "ci",
        GitHubActionsImage.MacOs12,
        GitHubActionsImage.Ubuntu2204,
        GitHubActionsImage.WindowsServer2022,
        OnPushBranches = new[] { "main" },
        OnPullRequestBranches = new[] { "main" },
        FetchDepth = 1,
        InvokedTargets = new[] { nameof(Test), nameof(Pack) })]
[GitHubActions(
    "publish",
        GitHubActionsImage.Ubuntu2204,
        OnPushBranches = new[] { "publish" },
        FetchDepth = 1,
        InvokedTargets = new[] { nameof(Publish) },
        ImportSecrets = new[] { nameof(NuGetApiKey) })]
class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode

    public static int Main () => Execute<Build>(x => x.Compile);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [GitRepository] readonly GitRepository GitRepository;

    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
            DotNetClean(_ => _
                .SetProject(Solution)
                .SetConfiguration(Configuration)
                .EnableNoLogo());

            ArtifactsDirectory.DeleteDirectory();
        });

    [Solution] readonly Solution Solution;

    [Parameter] readonly string Version;

    [Parameter] readonly string NuGetSource;

    [Parameter] [Secret] readonly string NuGetApiKey;

    Target Restore => _ => _
        .Executes(() =>
        {
            DotNetRestore(_ => _
                .SetProjectFile(Solution));
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            DotNetBuild(_ => _
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .SetNoRestore(FinishedTargets.Contains(Restore)));
        });

    AbsolutePath ArtifactsDirectory => RootDirectory / "artifacts";
    AbsolutePath TestResultsDirectory => ArtifactsDirectory / "test-results";
    AbsolutePath PackageDirectory => ArtifactsDirectory / "package";

    Target Test => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
           DotNetTest(_ => _
               .SetProjectFile(Solution)
               .SetConfiguration(Configuration)
               .SetNoBuild(FinishedTargets.Contains(Compile))
               .SetResultsDirectory(TestResultsDirectory));
        });

    Target Pack => _ => _
        .DependsOn(Compile)
        .Requires(() => Version)
        .Executes(() =>
        {
            DotNetPack(_ => _
                .SetProject(Solution)
                .SetConfiguration(Configuration)
                .SetNoBuild(FinishedTargets.Contains(Compile))
                .EnableNoLogo()
                .SetVersion(Version)
                .SetOutputDirectory(PackageDirectory)
                .SetProcessExitHandler(p => p.ExitCode switch
                {
                    0 or 1 => null,
                    _ => p.AssertZeroExitCode(),
                }));
        });

    Target Publish => _ => _
        .Requires(() => NuGetApiKey)
        .Requires(() => NuGetSource)
        .DependsOn(Clean)
        .DependsOn(Test)
        .DependsOn(Pack)
        .Executes(() =>
        {
            DotNetNuGetPush(_ => _
                .SetTargetPath(PackageDirectory.GetFiles().Single())
                .SetApiKey(NuGetApiKey)
                .SetSource(NuGetSource));
        });
}
