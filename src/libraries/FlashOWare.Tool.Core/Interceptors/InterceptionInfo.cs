namespace FlashOWare.Tool.Core.Interceptors;

public sealed record class InterceptionInfo
{
    private InterceptedCallSiteInfo? _callSite;

    internal InterceptionInfo(InterceptsLocationAttributeArguments attribute)
    {
        Attribute = attribute;
    }

    public InterceptionInfo(string filePath, int line, int character, string method)
    {
        Attribute = new InterceptsLocationAttributeArguments(filePath, line, character);
        _callSite = new InterceptedCallSiteInfo(method);
    }

    public InterceptsLocationAttributeArguments Attribute { get; }
    public InterceptedCallSiteInfo CallSite
    {
        get
        {
            return _callSite is null
                ? throw new InvalidOperationException($"{nameof(CallSite)} is unbound.")
                : _callSite;
        }
    }
    internal bool IsBound => _callSite is not null;

    internal void Bind(InterceptedCallSiteInfo callSite)
    {
        _callSite = _callSite is not null
            ? throw new InvalidOperationException($"{nameof(CallSite)} is already bound.")
            : callSite;
    }

    public override string ToString()
    {
        return $"Intercepts location {Attribute}: {(_callSite?.ToString() ?? "<unbound>")}";
    }
}
