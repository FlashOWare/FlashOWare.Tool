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
using Nuke.Common.Utilities.Collections;
using Nuke.Common.Tools.DotNet;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

[GitHubActions(
    "publish",
        GitHubActionsImage.WindowsServer2022,
        OnPushBranches = new[] { "publish" },
        InvokedTargets = new[] { nameof(Publish) },
        ImportSecrets = new[] { nameof(NuGetApiKey) })]
[GitHubActions(
    "ci",
        GitHubActionsImage.WindowsServer2022,
        OnPushBranches = new[] { "main" },
        OnPullRequestBranches = new[] { "main" },
        //FetchDepth = 1,
        InvokedTargets = new[] { nameof(Test), nameof(Pack) })]
// [GitHubActions(
//     "ci",
//         GitHubActionsImage.Ubuntu2204,
//         On = new[] { GitHubActionsTrigger.Push, GitHubActionsTrigger.PullRequest})]
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

    // nuke --solution1 <value>
    // env var: SOLUTION1 / NUKE_SOLUTION1
    // parameters.json
    [Parameter] readonly string Solution1 = "default.sln";

    [Parameter] readonly string Version;

    [Solution] readonly Solution Solution;

    [Parameter] [Secret] readonly string NuGetApiKey;

    [Parameter] readonly string NuGetSource;

    Target Restore => _ => _
        .Executes(() =>
        {
            // dotnet restore <solution>
            DotNet($"restore {Solution.Path}");
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            // dotnet build <solution> --configuration Debug/Release
            DotNetBuild(_ => _
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .SetNoRestore(FinishedTargets.Contains(Restore)));

            // nuke compile --skip (skip all dependencies)
            // nuke publish --skip restore (execute compile and publish)
            // nuke publish compile --skip (execute compile and publish)
        });

    //      v- root directory
    // <parent>/artifacts/test-results   (unix)
    // <parent>\artifacts\test-results
    AbsolutePath ArtifactsDirectory => RootDirectory / "artifacts";
    AbsolutePath TestResultsDirectory => ArtifactsDirectory / "test-results";
    AbsolutePath PackageDirectory => ArtifactsDirectory / "package";

    Target Test => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            //dotnet test sln --no-build --results-directory <path>
           DotNetTest(_ => _
               .SetProjectFile(Solution)
               .SetConfiguration(Configuration)
               //.SetFilter(EnvironmentInfo.IsWin ? "" : "only-non-win-tests")
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
