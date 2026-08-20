using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Wcs.Common.Abstractions;

namespace Wcs.Common.Services;

public sealed class ConfigService : IConfigService, IDisposable
{
    private readonly ILogger<ConfigService> _logger;
    private readonly string _configDirectory;
    private readonly Dictionary<string, IConfigurationRoot> _fileConfigs = new();
    private readonly object _lockObject = new();

    public ConfigService(string configDir, ILogger<ConfigService>? logger = null)
    {
        _logger = logger ?? Microsoft.Extensions.Logging.Abstractions.NullLogger<ConfigService>.Instance;
        _configDirectory = configDir;
        
        _logger.LogInformation("ConfigService khởi tạo với config directory: {ConfigDirectory}", configDir);
        
        // Tạo directory nếu chưa tồn tại
        if (!Directory.Exists(configDir))
        {
            Directory.CreateDirectory(configDir);
            _logger.LogInformation("Đã tạo config directory: {ConfigDirectory}", configDir);
        }

        _logger.LogInformation("ConfigService đã khởi tạo thành công");
    }

    public T GetFromFile<T>(string fileName) where T : class, new()
    {
        try
        {
            lock (_lockObject)
            {
                // Kiểm tra cache trước
                if (_fileConfigs.TryGetValue(fileName, out var cachedConfig))
                {
                    var cachedObj = new T();
                    cachedConfig.Bind(cachedObj);
                    _logger.LogDebug("Đã load config từ cache: {FileName} -> {TypeName}", fileName, typeof(T).Name);
                    return cachedObj;
                }
            }

            // Load file mới
            var filePath = Path.Combine(_configDirectory, fileName);
            if (!File.Exists(filePath))
            {
                _logger.LogWarning("Config file không tồn tại: {FilePath}", filePath);
                return new T();
            }

            // Tạo ConfigurationBuilder để đọc file
            var configBuilder = new ConfigurationBuilder()
                .SetBasePath(_configDirectory)
                .AddJsonFile(fileName, optional: false, reloadOnChange: true);
            
            var fileConfig = configBuilder.Build();
            
            // Cache config
            lock (_lockObject)
            {
                _fileConfigs[fileName] = fileConfig;
            }
            
            // Bind toàn bộ file content vào object T
            var newObj = new T();
            fileConfig.Bind(newObj);
            
            _logger.LogDebug("Đã load config từ file: {FileName} -> {TypeName}", fileName, typeof(T).Name);
            return newObj;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi load config từ file: {FileName}", fileName);
            return new T();
        }
    }

    public void Dispose()
    {
        lock (_lockObject)
        {
            _fileConfigs.Clear();
        }
        _logger.LogInformation("ConfigService đã được dispose");
    }
}