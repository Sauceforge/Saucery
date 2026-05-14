using Saucery.NuGet.Models;
using System.Text.Json;

namespace Saucery.NuGet.Core;

public static class GlobalConfigReader {
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web) {
        PropertyNameCaseInsensitive = true,
    };

    public static GlobalConfig Read(string solutionDirectory) {
        if(string.IsNullOrEmpty(solutionDirectory))
            return new GlobalConfig();

        var configPath = Path.Combine(solutionDirectory, Constants.GlobalConfig.FileName);

        if(!File.Exists(configPath)) {
            return new GlobalConfig();
        }

        try {
            var json = File.ReadAllText(configPath);
            return JsonSerializer.Deserialize<GlobalConfig>(json, JsonOptions) ?? new GlobalConfig();

        } catch(JsonException ex) {
            Console.WriteLine($"Warning: Failed to parse {Constants.GlobalConfig.FileName}: {ex.Message}. Global exclusions will be ignored.");
            return new GlobalConfig();
        }
    }
}
