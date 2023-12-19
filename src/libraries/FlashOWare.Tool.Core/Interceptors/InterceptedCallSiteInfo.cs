namespace FlashOWare.Tool.Core.Interceptors;

public sealed record class InterceptedCallSiteInfo(string Method)
{
    public override string ToString()
    {
        return Method;
    }
}
