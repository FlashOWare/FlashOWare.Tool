using Microsoft.CodeAnalysis;
using System.Diagnostics;

namespace FlashOWare.Tool.Cli.CodeAnalysis;

internal static class WorkspaceExtensions
{
    public static void ThrowIfCannotApplyChange(this Workspace workspace, ApplyChangesKind feature)
    {
        if (!workspace.CanApplyChange(feature))
        {
            throw new InvalidOperationException($"Change {feature} is not supported.");
        }
    }

    public static void ThrowIfCannotApplyChanges(this Workspace workspace, params ApplyChangesKind[] features)
    {
        Debug.Assert(IsOrdered(features), $"Expected ascending order.");

        foreach (var feature in features)
        {
            workspace.ThrowIfCannotApplyChange(feature);
        }
    }

    public static void ApplyChanges(this Workspace workspace, Solution newSolution)
    {
        if (!workspace.TryApplyChanges(newSolution))
        {
            string message = $"{nameof(Workspace)} has been updated since the {nameof(Solution)} was obtained from the {nameof(Workspace)}.";
            throw new InvalidOperationException(message);
        }
    }

    private static bool IsOrdered(ApplyChangesKind[] features)
    {
        ApplyChangesKind previous = (ApplyChangesKind)default - 1;

        for (int i = 0; i < features.Length; i++)
        {
            ApplyChangesKind current = features[i];

            if (previous >= current)
            {
                return false;
            }

            previous = current;
        }

        return true;
    }
}
