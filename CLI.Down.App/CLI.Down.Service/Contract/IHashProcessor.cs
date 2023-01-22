namespace CLI.Down.Service.Contract
{
    public interface IHashProcessor
    {
        string GetSHA256File(string filePath);
        string GetSHA1File(string filePath);
    }
}
