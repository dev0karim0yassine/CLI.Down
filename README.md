# CLI.Down

this project is a small CLI tool that downloads multiple files from various sources in parallel. 

all necessary data are extracted from a configuration file (.yaml).

Beside downloading files, the tool should verify the existence of the downloaded files.

Task 1:
Now the CLI supports parsing the following commands with their arguments (arguments order dosen't matter) :
-	**download [--verbose] [--dry-run] [parallel-downloads=N] config.yml**
-	**validate [--verbose] config.yml**

Also parsing the provider yaml file name into an object

Task 2:
- Implementing the download/validation process with all optional arguments
- Unit test with (xUnit, Moq and AutoFixture)

> How to:
- Go to the folder **CLI.Down.Console**
- Run the cmd **[dotnet run](https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-run)**

this project uses the packages: 
- *[FluentAssertions](https://www.nuget.org/packages/FluentAssertions)*
- *[Moq](https://www.nuget.org/packages/Moq)*
- *[AutoFixture](https://www.nuget.org/packages/AutoFixture)*
- *[YamlDotNet](https://www.nuget.org/packages/YamlDotNet)*
- *[System.IO.Abstractions](https://www.nuget.org/packages/System.IO.Abstractions)*
