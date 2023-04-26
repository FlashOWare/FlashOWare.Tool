namespace FlashOWare.Tool.Core.UsingDirectives;

//TODO: implement IEquatable (for parity with UsingDirective)
//TODO: add required Project Name (to display in CLI output)
//TODO: override ToString

public sealed class UsingCountResult //: IEquatable<UsingCountResult>
{
    private readonly Dictionary<string, UsingDirective> _usings = new();

    internal UsingCountResult()
    {
    }

    //public required string ProjectName { get; init; }
    public IReadOnlyCollection<UsingDirective> Usings => _usings.Values;

    internal void IncrementOrAdd(string identifier)
    {
        if (_usings.TryGetValue(identifier, out UsingDirective usingDirective))
        {
            usingDirective.Occurrences++;
        }
        else
        {
            _usings.Add(identifier, new UsingDirective(identifier));
        }
    }
}
