using Newtonsoft.Json;
using NLog;
using PlastMB.Utils.LogUtils;
using Worker.Application.CustomModels;
using Worker.Application.Interface;

namespace PlastMB.WorkerService
{
    /// <summary>
    /// Scan folder line 3
    /// </summary>
    public class WorkerForder : BackgroundService
    {
        private readonly IWorkerService _service;
        private string _jsonFilePath;
        private FileSystemWatcher _fileWatcher;
        private readonly ILoggerManager _logger;
        private FileSystemWatcher _watcher;
        private readonly IConfiguration _configuration;
        private readonly string _forderFileSetting;
        private FolderPaths initialFolderPaths;
        public WorkerForder(IWorkerService service)
        {
            _service = service;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var logConfigPath = Path.Combine(AppContext.BaseDirectory, "nlog.config");
            var logger = NLog.LogManager.Setup().LoadConfigurationFromFile(logConfigPath).GetCurrentClassLogger();
            try
            {
                logger.Info("========================================================================================================");
                logger.Info("Service worker on Start");
                //_logger.LogInfo($"Version: {Constant.VERSION_CODE} - Release Date: {Constant.VERSION_DATE}");
                // Khởi tạo lần đầu tiên
                initialFolderPaths = ReadFolderPathsFromJson(_jsonFilePath);

                while (!stoppingToken.IsCancellationRequested)
                {
                    await _service.ScanFolder(initialFolderPaths);
                    await Task.Delay(int.Parse(initialFolderPaths.TimeReadCsv)*60000, stoppingToken);
                }

                // Dừng cho đến khi token hủy được kích hoạt
                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error executing background service: {ex.Message}");
            }
        }

        public WorkerForder(IWorkerService service, ILoggerManager logger, IConfiguration configuration)
        {
            _forderFileSetting = configuration["FolderSetting"];
            _service = service;
            _logger = logger; // Khởi tạo ILoggerManager
            _logger.LogError($"Error executing background service: {_forderFileSetting + "appsetting.json 01"}");
            _jsonFilePath = Path.Combine(_forderFileSetting, "appsetting.json");
            //_jsonFilePath = Path.Combine(Properties.Resources.fileName , "appsetting.json");

            try
            {
                // Tạo một FileSystemWatcher để theo dõi sự thay đổi của tệp JSON
                _fileWatcher = new FileSystemWatcher(Path.GetDirectoryName(_jsonFilePath), Path.GetFileName(_jsonFilePath));
                _fileWatcher.NotifyFilter = NotifyFilters.LastWrite;
                _fileWatcher.Changed += OnJsonFileChanged;
                _fileWatcher.EnableRaisingEvents = true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error initializing FileSystemWatcher: {ex.Message}");
            }
        }




        private async void OnJsonFileChanged(object sender, FileSystemEventArgs e)
        {
            try
            {
                _fileWatcher.EnableRaisingEvents = false;

                // Fix Error reading JSON file: The process cannot access the file 'C:\Public\Denka\appsetting.json' because it is being used by another process.
                Thread.Sleep(300);     // 210 300

                // Nếu có sự thay đổi trong tệp JSON, tải lại cấu hình và thực hiện công việc
                if (e.ChangeType == WatcherChangeTypes.Changed)
                {
                    //_logger.LogWarn($"Json setting is changed: {_jsonFilePath}");

                    initialFolderPaths = ReadFolderPathsFromJson(_jsonFilePath);

                    // Dừng các trình theo dõi cũ trước khi bắt đầu các trình theo dõi mới
                    _service.Stop();

                    //await _service.ScanFolder(folderPaths);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error handling JSON file change: {ex.Message}");
            }
            finally
            {
                _fileWatcher.EnableRaisingEvents = true;
            }
        }

        private FolderPaths ReadFolderPathsFromJson(string jsonFilePath)
        {
            try
            {
                if (File.Exists(jsonFilePath))
                {
                    var json = File.ReadAllText(jsonFilePath);
                    return JsonConvert.DeserializeObject<FolderPaths>(json);
                }
                else
                {
                    _logger.LogError($"File setting JSON không tồn tại: {jsonFilePath}");
                    throw new FileNotFoundException("File JSON không tồn tại.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error reading JSON file: {ex.Message}");
                throw;
            }
        }
    }
}
