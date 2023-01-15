using CLI.Down.Entities;

namespace CLI.Down.Service.Contract
{
    public interface ICommandParser
    {
        CommandParser? ParseArgs(List<string>? args);

        YamlConfig? DeserializeYaml(string Path);

        bool IsValid(CommandParser command);
    }
}
