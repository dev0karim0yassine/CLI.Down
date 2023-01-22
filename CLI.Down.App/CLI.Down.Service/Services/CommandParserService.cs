using CLI.Down.Entities;
using CLI.Down.Service.Contract;
using System.Text.RegularExpressions;
using CLI.Down.Entities.CommandsArgs;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization;
using System.IO.Abstractions;

namespace CLI.Down.Service.Services
{
    public class CommandParserService : ICommandParser
    {
        private readonly IDeserializer _deserializer;
        private readonly IFileSystem _fileSystem;
        public CommandParserService(IDeserializer deserializer, IFileSystem fileSystem)
        {
            _deserializer = deserializer;
            _fileSystem = fileSystem;
        }

        public CommandParser? ParseArgs(List<string>? args)
        {
            if (args is null)
            {
                return null;
            }

            bool verbose = args.Contains(Arguments.Verbose);
            bool dryRun = args.Contains(Arguments.DryRun);
            bool isParallel = false;
            
            var regex = new Regex(Arguments.ParallelDownloadsRegex, RegexOptions.IgnoreCase);
            var parallel = args.FirstOrDefault(c => regex.IsMatch(c));
            byte parallelDownloads = 0;
            if (parallel is not null)
            {
                isParallel = true;
                parallelDownloads = byte.Parse(parallel.Split('=')[1]);
            }
            
            regex = new Regex(Commands.YamlRegex, RegexOptions.IgnoreCase);
            var yaml = args.FirstOrDefault(c => regex.IsMatch(c));

            return new CommandParser
            {
                IsVerbose = verbose,
                IsDryRun = dryRun,
                IsParallel = isParallel,
                ParallelDownloads = parallelDownloads,
                YamlPath = yaml
            };
        }

        public YamlConfig? DeserializeYaml(string Path)
        {
            YamlConfig? config;

            try
            {
                var yamlContent = _fileSystem.File.ReadAllText(Path);
                config = _deserializer.Deserialize<YamlConfig>(yamlContent);
            }
            catch
            {
                return null;
            }
                
            return config;
        }

        public bool IsValid(CommandParser arguments, string command)
        {
            return (arguments is not null && (command.Equals(Commands.Download) || command.Equals(Commands.Validate))) && _fileSystem.File.Exists(arguments.YamlPath);
        }
    }
}
