namespace CLI.Down.Entities;

public class CommandParser
{
    public bool IsVerbose { get; set; }
    public bool IsDryRun { get; set; }
    public bool IsParallel { get; set; }
    public byte ParallelDownloads { get; set; }
    public string? YamlPath { get; set; }
}
