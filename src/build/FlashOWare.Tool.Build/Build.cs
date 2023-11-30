using System;
using System.Linq;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.Execution;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

[GitHubActions(
    "ci",
        GitHubActionsImage.MacOs12,
        GitHubActionsImage.Ubuntu2204,
        GitHubActionsImage.WindowsServer2022,
        AutoGenerate = false,
        OnPushBranches = new[] { "main" },
        OnPullRequestBranches = new[] { "main" },
        FetchDepth = 1,
        InvokedTargets = new[] { nameof(Test), nameof(Pack) })]
[GitHubActions(
    "publish",
        GitHubActionsImage.Ubuntu2204,
        AutoGenerate = false,
        OnPushBranches = new[] { "publish" },
        FetchDepth = 1,
        InvokedTargets = new[] { nameof(Publish) },
        ImportSecrets = new[] { nameof(NuGetApiKey) })]
class Build : NukeBuild
{
    public static int Main() => Execute<Build>(x => x.Compile);

    [GitRepository] readonly GitRepository GitRepository;

    [Solution] readonly Solution Solution;

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Parameter] readonly string VersionPrefix;
    [Parameter] readonly string VersionSuffix;

    [Parameter] readonly string NuGetSource;

    [Parameter][Secret] readonly string NuGetApiKey;

    AbsolutePath ArtifactsDirectory => RootDirectory / "artifacts";
    AbsolutePath TestResultsDirectory => ArtifactsDirectory / "test-results";
    AbsolutePath PackageDirectory => ArtifactsDirectory / "package";
    AbsolutePath NuGetConfigFile => RootDirectory / "nuget.config";

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

    Target Restore => _ => _
        .Executes(() =>
        {
            DotNetRestore(_ => _
                .SetProjectFile(Solution)
                .SetConfigFile(NuGetConfigFile)
                .SetNoCache(IsServerBuild));
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            DotNetBuild(_ => _
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .SetNoRestore(FinishedTargets.Contains(Restore))
                .EnableNoLogo());
        });

    Target Test => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            DotNetTest(_ => _
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .SetNoBuild(FinishedTargets.Contains(Compile))
                .EnableNoLogo()
                .SetResultsDirectory(TestResultsDirectory));
        });

    Target Pack => _ => _
        .DependsOn(Compile)
        .Requires(() => VersionPrefix)
        .Requires(() => VersionSuffix)
        .Executes(() =>
        {
            DotNetPack(_ => _
                .SetProject(Solution)
                .SetConfiguration(Configuration)
                .SetNoBuild(FinishedTargets.Contains(Compile))
                .EnableNoLogo()
                .SetVersionPrefix(VersionPrefix)
                .SetVersionSuffix(VersionSuffix)
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
