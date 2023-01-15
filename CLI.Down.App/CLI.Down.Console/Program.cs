using CLI.Down.Entities.CommandsArgs;
using CLI.Down.Service.Contract;
using CLI.Down.Service.Services;

ICommandParser _commandParser = new CommandParserService();

//For test only
List<string> testArgs = new List<string>
{
    "download",
    "--verbose",
    "--dry-run",
    "parallel-downloads=3",
    "config.yml"
};

args = testArgs.ToArray();
var commandsArgs = args.ToList();
var command = commandsArgs.FirstOrDefault();

if (string.IsNullOrWhiteSpace(command) || !command.Equals(Commands.Download) || !command.Equals(Commands.Validate))
{
    Console.WriteLine($"Invalid command please provide a valid command");
}

var arguments = _commandParser.ParseArgs(commandsArgs);

if (!_commandParser.IsValid(arguments))
{
    Console.WriteLine($"Invalid command please provide a valid command");
}
else
{
    var config = _commandParser.DeserializeYaml(arguments?.YamlPath);

    switch (command)
    {
        //download [--verbose] [--dry-run] [parallel-downloads=N] config.yml
        case Commands.Download:
            Console.WriteLine($"{commandsArgs.First()} verbose:{arguments?.IsVerbose} dryRun:{arguments?.IsDryRun} parallelDownloads:{arguments?.IsParallel}({arguments?.ParallelDownloads}) yaml:{arguments?.YamlPath}");
            break;

        //validate [--verbose] config.yml
        case Commands.Validate:
            Console.WriteLine($"{commandsArgs.First()} verbose:{arguments?.IsVerbose} yaml:{arguments?.YamlPath}");
            break;
        default:
            break;
    }
}
