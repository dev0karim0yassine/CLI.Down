using CLI.Down.Entities;
using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLI.Down.Service.Contract
{
    public interface IUrlProcessor
    {
        long GetAllUrlsContentLength(string[] urls);
        HttpResponseMessage? IsValidUrl(string url);
        bool CheckValidFileSha(string filePath, FilesConfig filesConfig);
    }
}
