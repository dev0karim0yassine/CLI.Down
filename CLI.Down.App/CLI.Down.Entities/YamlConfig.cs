using YamlDotNet.Serialization;

namespace CLI.Down.Entities
{
    public class YamlConfig
    {
        [YamlMember(Alias = "config", ApplyNamingConventions = false)]
        public ParallelConfig? Config { get; set; }

        [YamlMember(Alias = "downloads", ApplyNamingConventions = false)]
        public List<FilesConfig>? Downloads { get; set; }
    }
}