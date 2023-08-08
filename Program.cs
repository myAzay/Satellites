using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MyConsoleApp.Configurations;
using MyConsoleApp.Extensions;
using MyConsoleApp.Helpers;
using MyConsoleApp.Interfaces;
using MyConsoleApp.Models;

namespace MyConsoleApp;
public class Program
{
    private static string _defaultFileName = "output.txt";
    private static string _folderName = "results";
    private static string _folderPath = "";

    private static ITleService _tleService;
    private static ILogger<Program> _logger;
    public static async Task Main(string[] args)
    {
        var serviceProvider = AppStartup();
        _logger = serviceProvider.GetService<ILoggerFactory>()
           .CreateLogger<Program>();

        _logger.LogInformation("Starting application..");
        _tleService = serviceProvider.GetService<ITleService>();

        string currentDirectory = Directory.GetCurrentDirectory();
        _folderPath = Path.Combine(currentDirectory, _folderName);

        try
        {
            var fileName = _defaultFileName;
            var allSatellites = await GetAllSatellites();
            SaveSatellitesToFile(allSatellites, fileName);

            var satelliteId = allSatellites.First().SatelliteId;
            var isExist = CheckIfSatelliteExistsInFile(fileName, satelliteId);
            _logger.LogInformation($"Satellite with id: {satelliteId} is exist: {isExist}");

            var areEqual = AreTlesEqual(fileName, allSatellites);
            _logger.LogInformation($"Tles in response and tles in file are equal: {areEqual}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Exception with message: {ex.Message}");
            throw;
        }
    }

    private static async Task<List<Tle>> GetAllSatellites()
    {
        List<Tle> allTles = await _tleService.GetAllTlesAsync();
        return allTles;
    }
    private static void SaveSatellitesToFile(List<Tle> tles, string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            fileName = _defaultFileName;

        fileName = fileName.AddTxtExtension();

        var filePath = Path.Combine(_folderPath, fileName);
        _tleService.SaveTlesToFile(tles, filePath);
    }

    private static bool CheckIfSatelliteExistsInFile(string fileName, long satelliteId)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            fileName = _defaultFileName;

        fileName = fileName.AddTxtExtension();

        var filePath = Path.Combine(_folderPath, fileName);
        if (!File.Exists(filePath))
        {
            _logger.LogWarning($"File path {filePath} not found");
            return false;
        }
        var isExist = _tleService.CheckIfSatelliteExistsInFile(filePath, satelliteId);
        return isExist;
    }

    private static bool AreTlesEqual(string fileName, List<Tle> tles)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            fileName = _defaultFileName;

        fileName = fileName.AddTxtExtension();
        var filePath = Path.Combine(_folderPath, fileName);
        if (!File.Exists(filePath))
        {
            _logger.LogWarning($"File path {filePath} not found");
            return false;
        }
        var fileTles = _tleService.GetTlesFromFile(filePath);
        var areEqual = SHA256HashHelper.AreHashesEqual(tles, fileTles);
        return areEqual;
    }

    private static IServiceProvider AppStartup()
    {
        var serviceProvider = new ServiceCollection()
            .Config()
            .BuildServiceProvider();
        return serviceProvider;
    }
}

