using CLI.Down.Entities;
using CLI.Down.Entities.CommandsArgs;
using CLI.Down.Service.Contract;
using CLI.Down.Service.Services;
using Microsoft.Extensions.DependencyInjection;
using System.IO.Abstractions;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

internal class Program
{
    private static readonly object _sync = new object();

    //For test only
    private static List<string> testArgs = new List<string>
    {
        "download",
        "--verbose",
        "--dry-run",
        "parallel-downloads=3",
        "config.yml"
    };

    private readonly static IProgress<double> progress = new Progress<double>(ReportProgress);

    private static async Task Main(string[] args)
    {
        #region Services DI Configurations
        // Create the service collection
        ServiceProvider serviceProvider = ConfigureDIServices();

        //Get the services
        var _urlProcessorService = serviceProvider.GetService<IUrlProcessor>();
        var _commandParserService = serviceProvider.GetService<ICommandParser>();
        var _fileSystem = serviceProvider.GetService<IFileSystem>();
        #endregion

        if (args.Length == 0)
        {
            Console.WriteLine($"No arguments provided TEST ARGS will be used");
            args = testArgs.ToArray();
        }

        var commandsArgs = args.ToList();
        var command = commandsArgs.FirstOrDefault();
        var arguments = _commandParserService.ParseArgs(commandsArgs);

        if (!_commandParserService.IsValid(arguments, command))
        {
            Console.WriteLine($"Invalid command please provide a valid command");
            Console.WriteLine($"Valid commands are {Commands.Download} or {Commands.Validate}");
            Console.WriteLine($"download [--verbose] [--dry-run] [parallel-downloads=N] pathToConfig.yml");
            Console.WriteLine($"validate [--verbose] pathToConfig.yml");
            Console.WriteLine($"Also check the pathToConfig.yml");
        }
        else
        {
            var yamlConfig = _commandParserService.DeserializeYaml(arguments?.YamlPath);

            switch (command)
            {
                //download [--verbose] [--dry-run] [parallel-downloads=N] config.yml
                //download --verbose --dry-run parallel-downloads=3 config.yml
                case Commands.Download:

                    if (yamlConfig is null)
                    {
                        Console.WriteLine("Invalid YML configuration\n");
                        return;
                    }
                    if (arguments.IsVerbose)
                    {
                        Console.Write($"processing command: {commandsArgs.First()} verbose:{arguments?.IsVerbose} dryRun:{arguments?.IsDryRun} parallelDownloads:{arguments?.IsParallel}({arguments?.ParallelDownloads}) yaml:{arguments?.YamlPath}\n");
                    }

                    var urls = yamlConfig.Downloads.Select(d => d.Url).ToArray();
                    long filesTotal = _urlProcessorService.GetAllUrlsContentLength(urls);
                    double filesDonwloadedSize = 0;
                    int maxParallel = yamlConfig.Downloads.Count;

                    if (arguments.IsParallel)
                    {
                        maxParallel = arguments.ParallelDownloads;
                    }
                    else if (yamlConfig.Config.ParallelDownloads > 0)
                    {
                        maxParallel = yamlConfig.Config.ParallelDownloads;
                    }
                    ParallelOptions _parallelOptions = new ParallelOptions
                    {
                        CancellationToken = CancellationToken.None,
                        MaxDegreeOfParallelism = maxParallel
                    };

                    await Task.Run(() =>
                    {
                        filesDonwloadedSize = ProcessParallelDownload(_urlProcessorService, _fileSystem, arguments, yamlConfig, filesTotal, filesDonwloadedSize, _parallelOptions);
                        CheckDownloadedFilesCheckSum(_urlProcessorService, _fileSystem, yamlConfig);
                    });
                    break;

                //validate [--verbose] config.yml
                case Commands.Validate:
                    Console.WriteLine($"{commandsArgs.First()} verbose:{arguments?.IsVerbose} yaml:{arguments?.YamlPath}\n");

                    if (yamlConfig is null)
                    {
                        Console.WriteLine("Invalid YML configuration\n");
                    }
                    break;
                default:
                    break;
            }
        }
    }

    private static ServiceProvider ConfigureDIServices()
    {
        var services = new ServiceCollection();

        //Add Services
        services.AddSingleton<IFileSystem, FileSystem>();
        services.AddSingleton<ICommandParser, CommandParserService>();
        services.AddSingleton<HttpClient, HttpClient>();
        services.AddSingleton<IUrlProcessor, UrlProcessorService>();
        services.AddSingleton<IHashProcessor, HashProcessorService>();
        services.AddSingleton(new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build());

        //Get the service provider
        var serviceProvider = services.BuildServiceProvider();
        return serviceProvider;
    }

    private static void ReportProgress(double value)
    {
        lock (_sync)
        {
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write($"Download Progress : {value:0.00}%");
        }
    }

    private static void CheckDownloadedFilesCheckSum(IUrlProcessor _urlProcessorService, IFileSystem? _fileSystem, YamlConfig config)
    {
        config.Downloads.ForEach(fileConfig =>
        {
            var filePath = Path.Combine(config.Config.DownloadDir, fileConfig.File);
            if (_fileSystem.File.Exists(filePath))
            {
                bool isValidFileSha = _urlProcessorService.CheckValidFileSha(filePath, fileConfig);

                if (!isValidFileSha)
                {
                    Console.WriteLine($"\n{fileConfig.File}: invalid hash({fileConfig.Sha256}{fileConfig.Sha1})");
                    _fileSystem.File.Delete(filePath);
                    Console.WriteLine($"\n{fileConfig.File}: deleted");
                }
            }
        });
    }

    private static double ProcessParallelDownload(IUrlProcessor _urlProcessorService, IFileSystem? _fileSystem, CommandParser? cmdArguments, YamlConfig yamlConfig, long filesTotal, double filesDonwloadedSize, ParallelOptions _parallelOptions)
    {
        Parallel.ForEach(yamlConfig.Downloads, _parallelOptions, fileConfig =>
        {
            using HttpClient client = new HttpClient();
            var response = _urlProcessorService.IsValidUrl(fileConfig.Url);
            if (response is null)
            {
                Console.WriteLine($"Invalid Url: {fileConfig.Url}\n");
                return;
            }

            var filePath = Path.Combine(yamlConfig.Config.DownloadDir, fileConfig.File);
            if (cmdArguments.IsVerbose)
            {
                Console.Write($"Downloading url: {fileConfig.Url}\n");
                if (_fileSystem.File.Exists(filePath))
                {
                    Console.Write($"File: {fileConfig.File} already exist\n");
                    if (fileConfig.Overwrite)
                    {
                        Console.Write($"Overwriting file: {fileConfig.File}\n");
                    }
                    else
                    {
                        Console.Write($"File: {fileConfig.File} will not be Overwrited\n");
                    }
                }
            }

            if (fileConfig.Overwrite || !_fileSystem.File.Exists(filePath))
            {
                FileSystemStream? _fileStream;
                if (cmdArguments.IsDryRun)
                {
                    _fileStream = _fileSystem?.FileStream.New(filePath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, FileOptions.DeleteOnClose);
                }
                else
                {
                    _fileStream = _fileSystem?.FileStream.New(filePath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true);

                }
                using var stream = response.Content.ReadAsStreamAsync().Result;
                using FileSystemStream? fileStream = _fileStream;
                var buffer = new byte[8192];
                var size = stream.Read(buffer, 0, buffer.Length);
                double lastReportedProgress = 0;
                while (size > 0)
                {
                    fileStream?.Write(buffer, 0, size);
                    size = stream.Read(buffer, 0, buffer.Length);
                    filesDonwloadedSize += size;
                    var currentProgress = filesDonwloadedSize / filesTotal * 100;
                    if (currentProgress - lastReportedProgress >= 0.2)
                    {
                        lastReportedProgress = currentProgress;
                        progress.Report(filesDonwloadedSize / filesTotal * 100);
                    }
                }
            }
        });
        progress.Report(100);
        return filesDonwloadedSize;
    }
}