
namespace CLI.Down.Service.Tests
{
    public class CommandParserServiceTest
    {
        private readonly Mock<IDeserializer> _mockDeserializer;
        private readonly IDeserializer _realDeserializer;
        private readonly Mock<IDeserializer> _deserializer;
        private readonly Mock<IFileSystem> _fileSystem;
        private readonly Fixture _fixture;
        private CommandParserService _sut;

        private readonly static string yamlContent = "config:\r\n  parallel_downloads: 3\r\n  download_dir: D:\\Backup\r\ndownloads:\r\n- url: https://www.7-zip.org/a/7z2201.exe\r\n  file: 7z2201.exe\r\n  sha256: 8c8fbcf80f0484b48a07bd20e512b103969992dbf81b6588832b08205e3a1b43";
        public CommandParserServiceTest()
        {
            _deserializer = new Mock<IDeserializer>();
            _fileSystem = new Mock<IFileSystem>();
            _mockDeserializer= new Mock<IDeserializer>();
            _realDeserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();
            _sut = new CommandParserService(_mockDeserializer.Object, _fileSystem.Object);
            _fixture = new Fixture();
        }

        [Fact]
        public void ParseArgs_WhenInvalidArguments_ReturnsNullableCommandParser()
        {
            //Arrange
            List<string>? args = null;

            //Act
            var result = _sut.ParseArgs(args);

            //Assert
            result.Should().BeNull();
        }

        [Fact]
        public void ParseArgs_WhenValidArguments_ReturnsValidCommandParser()
        {
            //Arrange
            List<string> args = new()
            {
                "download",
                "--verbose",
                "--dry-run",
                "parallel-downloads=3",
                "config.yml"
            };
            var expected = new CommandParser
            {
                IsVerbose = true,
                IsDryRun = true,
                IsParallel = true,
                ParallelDownloads = 3,
                YamlPath = "config.yml"
            };

            //Act
            var result = _sut.ParseArgs(args);

            //Assert
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void DeserializeYaml_WhenInvalidYaml_ReturnsNullableYamlConfig()
        {
            //Arrange
            string yamlPath = _fixture.Create<string>();

            _fileSystem.Setup(f => f.File.ReadAllText(It.IsAny<string>()))
                .Returns(string.Empty).Verifiable();

            _mockDeserializer.Setup(d => d.Deserialize<It.IsAnyType>(It.IsAny<string>()))
                .Throws(new Exception()).Verifiable();

            //Act
            var result = _sut.DeserializeYaml(yamlPath);

            //Assert
            result.Should().BeNull();
            _fileSystem.Verify();
            _deserializer.Verify();
        }

        [Fact]
        public void DeserializeYaml_WhenValidYaml_ReturnsValidYamlConfig()
        {
            //Arrange
            string yamlPath = "config.yml";
            var expected = new YamlConfig
            {
                Config = new ParallelConfig { DownloadDir = "D:\\Backup", ParallelDownloads = 3 },
                Downloads = new List<FilesConfig>()
                {
                    new FilesConfig
                    {
                        File = "7z2201.exe",
                        Sha256= "8c8fbcf80f0484b48a07bd20e512b103969992dbf81b6588832b08205e3a1b43",
                        Url = "https://www.7-zip.org/a/7z2201.exe",
                        Overwrite= true, Sha1 = null
                    }
                }
            };

            _fileSystem.Setup(f => f.File.ReadAllText(It.IsAny<string>()))
                .Returns(yamlContent).Verifiable();

            //Act
            _sut = new CommandParserService(_realDeserializer, _fileSystem.Object);
            var result = _sut.DeserializeYaml(yamlPath);
            
            //Assert
            result.Should().BeEquivalentTo(expected);
            _fileSystem.Verify();
        }

        [Fact]
        public void IsValid_WhenInvalidCommandOrYamlPath_ReturnsFalse()
        {
            //Arrange
            var args = _fixture.Create<CommandParser>();

            _fileSystem.Setup(f => f.File.Exists(It.IsAny<string>()))
                .Returns(false).Verifiable();

            //Act
            var result = _sut.IsValid(args, Commands.Download);

            //Assert
            result.Should().Be(false);
            _fileSystem.Verify();
        }
        
        [Fact]
        public void IsValid_WhenValidCommandAndYamlPath_ReturnsTrue()
        {
            //Arrange
            var command = new CommandParser { YamlPath = "config?yml" };

            _fileSystem.Setup(f => f.File.Exists(It.IsAny<string>()))
                .Returns(true).Verifiable();

            //Act
            var result = _sut.IsValid(command, Commands.Validate);

            //Assert
            result.Should().Be(true);
            _fileSystem.Verify();
        }

    }
}