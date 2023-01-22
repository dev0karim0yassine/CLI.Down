using CLI.Down.Service.Contract;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Abstractions;
using System.IO.Pipes;
using System.Security.Cryptography;
using CLI.Down.Entities;

namespace CLI.Down.Service.Services
{
    public class UrlProcessorService : IUrlProcessor
    {
        private readonly HttpClient _client;
        private readonly IHashProcessor _hashProcessor;
        public UrlProcessorService(HttpClient client, IHashProcessor hashProcessor)
        {
            _client = client;
            _hashProcessor = hashProcessor;
        }

        public long GetAllUrlsContentLength(string[] urls)
        {
            long filesTotal = 0;

            foreach (var url in urls)
            {
                var response = IsValidUrl(url);
                if (response is not null)
                {
                    filesTotal += response.Content.Headers.ContentLength.Value;
                }
            }

            return filesTotal;
        }

        public HttpResponseMessage? IsValidUrl(string url) 
        {
            var response = _client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, CancellationToken.None).Result;
            
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return response;
        }

        public bool CheckValidFileSha(string filePath, FilesConfig filesConfig)
        {
            bool isValidKey = false;
            
            if (!string.IsNullOrWhiteSpace(filesConfig.Sha256))
            {
                isValidKey = _hashProcessor.GetSHA256File(filePath).Equals(filesConfig.Sha256);
            }
            else if (!string.IsNullOrWhiteSpace(filesConfig.Sha1))
            {
                isValidKey = _hashProcessor.GetSHA1File(filePath).Equals(filesConfig.Sha1);
            }

            return isValidKey;
        }
    }
}
