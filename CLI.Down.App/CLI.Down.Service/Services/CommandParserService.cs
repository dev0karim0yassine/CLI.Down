using CLI.Down.Entities;
using CLI.Down.Service.Contract;
using System.Text.RegularExpressions;
using CLI.Down.Entities.CommandsArgs;
using YamlDotNet.Serialization.NamingConventions;

namespace CLI.Down.Service.Services
{
    public class CommandParserService : ICommandParser
    {
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
            var deserializer = new YamlDotNet.Serialization.DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

            YamlConfig? config;

            try
            {
                config = deserializer.Deserialize<YamlConfig>(File.ReadAllText(Path));
            }
            catch
            {
                return null;
            }
                
            return config;
        }

        public bool IsValid(CommandParser command)
        {
            if(command is null || string.IsNullOrWhiteSpace(command.YamlPath))
                return false;

            return true;
        }
    }
}
