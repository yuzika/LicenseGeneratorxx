public class EncryptionProgress
{
    public int Percentage { get; }
    public string Stage { get; }
    public long BytesProcessed { get; }
    public long TotalBytes { get; }

    public EncryptionProgress(int percentage, string stage, long bytesProcessed, long totalBytes)
    {
        Percentage = percentage;
        Stage = stage;
        BytesProcessed = bytesProcessed;
        TotalBytes = totalBytes;
    }
}