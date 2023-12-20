using Microsoft.CodeAnalysis;
using System.Diagnostics.CodeAnalysis;

namespace FlashOWare.Tool.Core.Interceptors;

public sealed class InterceptorList
{
    private readonly List<InterceptorInfo> _interceptors = [];

    internal InterceptorList()
    {
    }

    [SetsRequiredMembers]
    internal InterceptorList(string projectName)
    {
        ProjectName = projectName;
    }

    public required string ProjectName { get; init; }
    public IReadOnlyList<InterceptorInfo> Interceptors => _interceptors;

    internal void Add(InterceptorInfo interceptor)
    {
        _interceptors.Add(interceptor);
    }

    internal void AddRange(List<InterceptorInfo> interceptors)
    {
        _interceptors.AddRange(interceptors);
    }

    public override string ToString()
    {
        int interceptions = _interceptors.SelectMany(static interceptor => interceptor.Interceptions).Count();
        if (interceptions == 0)
        {
            return $"{nameof(Project)} {ProjectName} contains no interceptors";
        }
        else
        {
            return $"{nameof(Project)} {ProjectName} contains {_interceptors.Count} {(_interceptors.Count == 1 ? "interceptor" : "interceptors")} with {interceptions} {(interceptions == 1 ? "interception" : "interceptions")}";
        }
    }
}
