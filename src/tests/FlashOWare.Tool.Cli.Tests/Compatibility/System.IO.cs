namespace System.IO;

#if !NET7_0_OR_GREATER
internal static class StreamReaderCompatibility
{
    public static Task<string> ReadToEndAsync(this StreamReader streamReader, CancellationToken cancellationToken)
    {
        return streamReader.ReadToEndAsync();
    }
}
#endif
