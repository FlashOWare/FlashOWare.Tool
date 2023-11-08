using System.Diagnostics.CodeAnalysis;

namespace FlashOWare.Tool.Core.Interceptors;

public sealed class InterceptorList
{
    internal InterceptorList()
    {
    }

    [SetsRequiredMembers]
    public InterceptorList(string result)
    {
        Result = result;
    }

    public required string Result { get; init; }
}
