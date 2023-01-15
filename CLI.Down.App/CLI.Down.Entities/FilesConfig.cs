namespace CLI.Down.Entities
{
    public class FilesConfig
    {
        public string? Url { get; set; }
        public string? File { get; set; }
        public string? Sha256 { get; set; }
        public string? Sha1 { get; set; }
        public bool Overwrite { get; set; }
    }
}