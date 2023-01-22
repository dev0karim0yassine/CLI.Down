using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLI.Down.Service.Contract
{
    public interface IHashProcessor
    {
        string GetSHA256File(string filePath);
        string GetSHA1File(string filePath);
    }
}
