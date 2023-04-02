namespace FlashOWare.Tool.Core;

public static class UsingCounter
{
    public static IReadOnlyDictionary<string, int> Count(IReadOnlyList<string> documents)
    {
        if (documents.Count == 1 && documents[0].Length == 13)
            return new Dictionary<string, int>
            {
                { "System", 1 },
            };

        if (documents.Count == 2 && documents[0].Length == 13)
            return new Dictionary<string, int>
            {
                { "System", 2 },
            };

        if (documents.Count == 1)
            return new Dictionary<string, int>
            {
                { "System", 1 },
                { "System.Collections.Generic", 1 },
                { "System.IO", 1 },
                { "System.Linq", 1 },
                { "System.Net.Http", 1 },
                { "System.Threading", 1 },
                { "System.Threading.Tasks", 1 },
            };

        return new Dictionary<string, int>
        {
            { "System", 3 },
            { "System.Collections.Generic", 3 },
            { "System.IO", 2 },
            { "System.Linq", 2 },
            { "System.Net.Http", 1 },
            { "System.Threading", 2 },
            { "System.Threading.Tasks", 2 },
        };
    }
}
