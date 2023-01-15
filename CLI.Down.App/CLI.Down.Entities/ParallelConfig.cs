using YamlDotNet.Serialization;

namespace CLI.Down.Entities
{
    public class ParallelConfig
    {
        [YamlMember(Alias = "parallel_downloads", ApplyNamingConventions = false)]
        public byte ParallelDownloads { get; set; }

        [YamlMember(Alias = "download_dir", ApplyNamingConventions = false)]
        public string? DownloadDir { get; set; }
    }
}