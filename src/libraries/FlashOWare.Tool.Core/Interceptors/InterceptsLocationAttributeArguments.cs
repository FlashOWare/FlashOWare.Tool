namespace FlashOWare.Tool.Core.Interceptors;

public sealed record class InterceptsLocationAttributeArguments(string FilePath, int Line, int Character)
{
    public override string ToString()
    {
        return $"""("{FilePath}", {Line}, {Character})""";
    }

    public string ToNavigationString()
    {
        return $"{FilePath}:{Line}:{Character}";
    }
}
