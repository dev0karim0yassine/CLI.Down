namespace CLI.Down.Service.Tests
{
    public class HashProcessorServiceTest
    {
        private readonly IFileSystem _fileSystem;
        private readonly HashProcessorService _sut;

        public HashProcessorServiceTest()
        {
            _fileSystem= new FileSystem();
            _sut = new HashProcessorService(_fileSystem);
        }

        [Fact]
        public void GetSHA1File_ForPridedFile_ReturnsSHA1()
        {
            //Arrange
            var filePath = "C:\\Windows\\explorer.exe";
            var sha1 = "fc924e1bbec021cb5685b05728618eb421ad3fbe";

            //Act
            var result = _sut.GetSHA1File(filePath);

            //Assert
            result.Should().Be(sha1);
        }

        [Fact]
        public void GetSHA256File_ForPridedFile_ReturnsSHA256()
        {
            //Arrange
            var filePath = "C:\\Windows\\explorer.exe";
            var sha256 = "0472c590414103f5f8fb9fb3d710adc5dfd13539e48b4aaa55cc954203202c13";

            //Act
            var result = _sut.GetSHA256File(filePath);

            //Assert
            result.Should().Be(sha256);
        }
    }
}
