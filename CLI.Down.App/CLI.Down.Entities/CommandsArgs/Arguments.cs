namespace CLI.Down.Entities.CommandsArgs;

public static class Arguments
{
    //Arguments
    public const string Verbose = "--verbose";
    public const string DryRun = "--dry-run";
    public const string ParallelDownloadsRegex = "parallel-downloads=[0-9]";
    public const string Yaml = "[A-Za-z0-9]+\\.yml";
}
