using CLI.Down.Entities;

namespace CLI.Down.Service.Contract
{
    public interface IUrlProcessor
    {
        long GetAllUrlsContentLength(string[] urls);
        HttpResponseMessage? IsValidUrl(string url);
        bool CheckValidFileSha(string filePath, FilesConfig filesConfig);
    }
}
