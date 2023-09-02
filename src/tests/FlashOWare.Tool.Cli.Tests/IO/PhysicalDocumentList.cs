namespace FlashOWare.Tool.Cli.Tests.IO;

internal sealed class PhysicalDocumentList
{
    private readonly List<PhysicalDocument> _documents = new();

    private PhysicalDocumentList() { }

    public PhysicalDocument[] Documents => _documents.ToArray();

    public static PhysicalDocumentList Create(string text, string filePath)
    {
        PhysicalDocumentList list = new();
        list._documents.Add(new PhysicalDocument(text, filePath));
        return list;
    }

    public static PhysicalDocumentList Create(string text, string name, params string[] folders)
    {
        PhysicalDocumentList list = new();
        list._documents.Add(new PhysicalDocument(text, name, folders));
        return list;
    }

    public PhysicalDocumentList Add(string text, string filePath)
    {
        _documents.Add(new PhysicalDocument(text, filePath));
        return this;
    }

    public PhysicalDocumentList Add(string text, string name, params string[] folders)
    {
        _documents.Add(new PhysicalDocument(text, name, folders));
        return this;
    }
}
