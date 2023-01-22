using CLI.Down.Service.Contract;
using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CLI.Down.Service.Services
{
    public class HashProcessorService : IHashProcessor
    {
        private readonly IFileSystem _fileSystem;
        public HashProcessorService(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }
        public string GetSHA1File(string filePath)
        {
            using FileSystemStream fileStream = _fileSystem.File.OpenRead(filePath);
            using SHA1 sha1 = SHA1.Create();
            byte[] hash = sha1.ComputeHash(fileStream);
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }

        public string GetSHA256File(string filePath)
        {
            using FileSystemStream fileStream = _fileSystem.File.OpenRead(filePath);
            using SHA256 sha256 = SHA256.Create();
            byte[] hash = sha256.ComputeHash(fileStream);
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }
    }
}
