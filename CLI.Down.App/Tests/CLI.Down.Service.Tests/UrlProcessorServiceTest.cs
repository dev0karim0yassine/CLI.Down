using CLI.Down.Service.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLI.Down.Service.Tests
{
    public class UrlProcessorServiceTest
    {
        private readonly HttpClient _client;
        private readonly Mock<IHashProcessor> _hashProcessor;
        private readonly Fixture _fixture;
        private readonly UrlProcessorService _sut;

        public UrlProcessorServiceTest()
        {
            _client = new HttpClient();
            _hashProcessor = new Mock<IHashProcessor>();
            _fixture = new Fixture();
            _sut = new UrlProcessorService(_client, _hashProcessor.Object);
        }

        [Fact]
        public void GetAllUrlsContentLength_CheckFilesUrlProvided_ReturnsContentLength()
        {
            //Arrange
            string[] url =
            {
                "https://www.7-zip.org/a/7z2201.exe" ,
                "https://www.7-zip.org/a/7z2201-x64.exeeee" //not found = 0
            };
            long expected = 1290308;

            //Act
            var result = _sut.GetAllUrlsContentLength(url);

            //Assert
            result.Should().Be(expected);
        }

        [Fact]
        public void IsValidUrl_CheckFileUrlProvided_ReturnsNullableHttpResponseMessage()
        {
            //Act
            var result = _sut.IsValidUrl("https://www.7-zip.org/a/7z2201-x64.exeeee");

            //Assert
            result.Should().BeNull();
        }

        [Fact]
        public void IsValidUrl_CheckFileUrlProvided_ReturnsValidHttpResponseMessage()
        {
            //Act
            var result = _sut.IsValidUrl("https://www.7-zip.org/a/7z2201.exe");

            //Assert
            result.Should().NotBeNull();
        }

        [Fact]
        public void CheckValidFileSha_WhenValidFileSha256_ReturnsTrue()
        {
            //Arrange
            var hash = _fixture.Create<string>();
            var config = new FilesConfig { Sha256 = hash };
            string filePath = "config.yml";

            _hashProcessor.Setup(h => h.GetSHA256File(It.IsAny<string>()))
                .Returns(hash).Verifiable();

            //Act
            var result = _sut.CheckValidFileSha(filePath, config);

            //Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void CheckValidFileSha_WhenInvalidFileSha256_ReturnsFalse()
        {
            //Arrange
            var hash = _fixture.Create<string>();
            var config = new FilesConfig { Sha256 = hash };
            string filePath = "config.yml";

            _hashProcessor.Setup(h => h.GetSHA256File(It.IsAny<string>()))
                .Returns($"{hash}0").Verifiable();

            //Act
            var result = _sut.CheckValidFileSha(filePath, config);

            //Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void CheckValidFileSha_WhenValidFileSha1_ReturnsTrue()
        {
            //Arrange
            var hash = _fixture.Create<string>();
            var config = new FilesConfig { Sha1 = hash };
            string filePath = "config.yml";

            _hashProcessor.Setup(h => h.GetSHA1File(It.IsAny<string>()))
                .Returns(hash).Verifiable();

            //Act
            var result = _sut.CheckValidFileSha(filePath, config);

            //Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void CheckValidFileSha_WhenInvalidFileSha1_ReturnsFalse()
        {
            //Arrange
            var hash = _fixture.Create<string>();
            var config = new FilesConfig { Sha1 = hash };
            string filePath = "config.yml";

            _hashProcessor.Setup(h => h.GetSHA1File(It.IsAny<string>()))
                .Returns($"{hash}0").Verifiable();

            //Act
            var result = _sut.CheckValidFileSha(filePath, config);

            //Assert
            result.Should().BeFalse();
        }
    }
}
