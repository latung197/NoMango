using Worker.Application.CustomModels.Dtos;
using Worker.Application.Interface;
using PlastMB.Utils;
using PlastMB.Utils.LogUtils;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Worker.Application.CustomModels.Others;
using Worker.Application.Enum;
using Newtonsoft.Json;
using Microsoft.Extensions.FileSystemGlobbing;
using Worker.Application.CustomModels;
using System.Globalization;
using System.IO;
using System.Reflection.PortableExecutable;
using Worker.Application.CustomModels.SearchConditions;

namespace Worker.Application.Services
{
    public class WorkerServiceImpl : IWorkerService
    {
        #region Properties
        //Get config from appsettings.json if need
        private readonly IConfiguration _configuration;
        //Mapping model to entity
        private readonly IMapper _mapper;
        //Log
        private readonly ILoggerManager _logger;
        private readonly IWorkerServiceClient _workerServiceClient;
        private readonly string _fileSetting;

        private string _folderPath1;
        
        private FileSystemWatcher _watcher;


        #endregion

        #region Constructor
        public WorkerServiceImpl(IConfiguration configuration
            , IMapper mapper
            , ILoggerManager logger
            , IWorkerServiceClient workerServiceClient)
        {
            _configuration = configuration;
            _mapper = mapper;
            _logger = logger;
            _workerServiceClient = workerServiceClient;
            _fileSetting = _configuration["FolderSetting"];
        }
        #endregion

        #region Other

        public async Task ScanFolder(string path)
        {

            if (!Directory.Exists(_folderPath1)) return;

            _logger.LogInfo($"======================================================================");
            _logger.LogInfo($"Scan line 3: {_folderPath1}");

        }

        private bool IsCsvFile(string filePath)
        {
            return Path.GetExtension(filePath).Equals(".csv", StringComparison.OrdinalIgnoreCase);
        }

        public async void ProcessDailyLogFolders(string path)
        {
            _logger.LogInfo($"=== Quét thư mục DailyLogOffice-Server ===");

            var logFolders = Directory.GetDirectories(_folderPath1, "*", SearchOption.AllDirectories)
                               .Where(path => new DirectoryInfo(path).Name.StartsWith("DailyLogOffice-Server"))
                               .ToList();

            foreach (var folder in logFolders)
            {
                _logger.LogInfo($"Đang quét thư mục: {folder}");

                var csvFiles = Directory.GetFiles(folder, "*.csv", SearchOption.AllDirectories);

                foreach (var filePath in csvFiles)
                {
                    _logger.LogInfo($"Đang quét : {filePath}");

                    FileInfo file = new FileInfo(filePath);
                    string directory = Path.GetDirectoryName(filePath);
                    string directory_out = directory.Replace("FTProot", "FTProot_Out");
                    try
                    {

                        string fileName = Path.GetFileName(filePath);
                        string[] parts = fileName.Split('_');
                        string machineId = parts[0];
                        string machineName = parts[1];
                        string fileDate = parts[2].Replace(".csv", "");
                        try
                        {
                            _logger.LogInfo("Reading file line 3 " + file.FullName);
                            var dataCsv = CsvUtils.ParseCsvCommon<TrnOperationOeeCsv>(file.FullName);
                            if (dataCsv.Any())
                            {
                                var lstData = new List<TrnOperationOeeDto>();
                                foreach (var item in dataCsv)
                                {

                                    //Folder lấy kết quả theo Csv
                                    var data = new TrnOperationOeeDto();
                                    string rawTime = string.IsNullOrWhiteSpace(item.Time) ? "00:00:00" : item.Time;
                                    DateTime dt = DateTime.ParseExact(rawTime, "H:mm:ss", CultureInfo.InvariantCulture);
                                    string formattedTime = dt.ToString("HH:mm:ss");
                                    string strDateTime = item.Date.ToString("MM/dd/yyyy") + " " + formattedTime;
                                    DateTime dtime = DateTime.ParseExact(strDateTime, "MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                                    data.MachineNo = machineId;
                                    data.AchievementRegistrationDate = item.Date;
                                    data.AchievementRegistrationTime = dtime;
                                    data.ProcessingTime = ConverTimetoInt(item.加工時間);
                                    data.ProcessingStopTime = ConverTimetoInt(item.作業停止時間);
                                    data.LossStopTime = ConverTimetoInt(item.ロス停止時間);
                                    data.ProductionCount = int.Parse(item.生産件数);
                                    data.OperationRate = decimal.Parse(item.稼働率);
                                    data.EquipmentOperationHours = ConverTimetoInt(item.設備稼働時間);
                                    data.LoadTime = ConverTimetoInt(item.負荷時間);
                                    data.TimeOperatingRate = decimal.Parse(item.時間稼働率);
                                    data.fileName = fileName;
                                    data.machineName = machineName;
                                    data.fileDate = fileDate;
                                    lstData.Add(data);
                                }
                                var response = await _workerServiceClient.ImportListTrnOperationOee(lstData);
                                _logger.LogInfo("Move File line 3 " + file.FullName);
                                _logger.LogInfo(JsonConvert.SerializeObject(lstData));
                                _logger.LogInfo(JsonConvert.SerializeObject(response));
                                MoveCsv(directory, directory_out, file.Name);
                            }
                        }
                        catch (Exception ex)
                        {
                            try
                            {
                                DateTime importTime = DateTime.Now;

                                // Tạo tham số tìm kiếm
                                var searchParam = new MstMachineSearchImpl
                                {
                                    HMI_NO = "",
                                    MACHINE_NO = machineId,
                                    MACHINE_NAME = ""
                                };

                                // Serialize tham số thành JSON
                                var jsonParam = JsonConvert.SerializeObject(searchParam);
                                // Gửi yêu cầu POST đến API
                                var responseString = await _workerServiceClient.SearchMstMachine(jsonParam);
                                // Xử lý dữ liệu phản hồi
                                var responseMachine = JsonConvert.DeserializeObject<GenericResponseResult<MstMachineDto>>(responseString);
                                var ListdataMachine = responseMachine.ListData;

                                var lstData = new List<TrnImportHistoryDto>();
                                var data = new TrnImportHistoryDto();
                                data.FileName = fileName;
                                if (ListdataMachine.Count > 0)
                                {   // khách hàng đã đảm bảo HMI_NO không trùng nên lấy giá trị đầu tiên
                                    data.MachineNo = ListdataMachine[0].MachineNo;
                                    data.FactoryCd = ListdataMachine[0].InstallationLocationCd;
                                }
                                else
                                {
                                    data.MachineNo = "-1";
                                    data.FactoryCd = -1;
                                }
                                data.Status = "失敗";
                                data.Note = "データ不正の為、データのインポートに失敗しました。";
                                data.Flag = "1";
                                data.ImportTime = importTime;
                                lstData.Add(data);
                                var response = await _workerServiceClient.ImportListTrnImportHistory(lstData);
                                _logger.LogError(ex, "Error while read file " + file.Name);
                                _logger.LogError(ex, "Error while read file " + ex.Message);

                            }
                            catch(Exception e)
                            {
                                _logger.LogError(ex, "Error Import History File Error " + ex.Message);

                            }
                        }
                        finally
                        {
                        }
                    }
                    catch { }
                }
            }
        }

        public async void ProcessProdLogFolders(string path)
        {
            _logger.LogInfo($"=== Quét thư mục ProdLog_SHUYAKU_1-Server ===");

            var logFolders = Directory.GetDirectories(_folderPath1, "*", SearchOption.AllDirectories)
                               .Where(path => new DirectoryInfo(path).Name.StartsWith("ProdLog_SHUYAKU_1"))
                               .ToList();

            foreach (var folder in logFolders)
            {
                _logger.LogInfo($"Đang quét thư mục: {folder}");

                var csvFiles = Directory.GetFiles(folder, "*.csv", SearchOption.AllDirectories);

                foreach (var filePath in csvFiles)
                {
                    _logger.LogInfo($"Đang quét : {filePath}");

                    FileInfo file = new FileInfo(filePath);
                    string fileName = Path.GetFileName(filePath);
                    string[] parts = fileName.Split('_');
                    string hmiNo = parts[1];
                    string fileDateTime = parts[2].Replace(".csv", ""); ;

                    string directory = Path.GetDirectoryName(filePath);
                    string directory_out = directory.Replace("FTProot", "FTProot_Out");
                    try
                    {
                        _logger.LogInfo("Reading file  3 " + file.FullName);
                        var dataCsv = CsvUtils.ParseCsvCommon<TrnOperationResultCsv>(file.FullName);
                        if (dataCsv.Any())
                        {
                            var lstData = new List<TrnOperationResultDto>();
                            foreach (var item in dataCsv)
                            {
                                //Folder lấy kết quả theo Csv
                                var data = new TrnOperationResultDto();

                                string rawTime = string.IsNullOrWhiteSpace(item.Time) ? "00:00:00" : item.Time;
                                DateTime dt = DateTime.ParseExact(rawTime, "H:mm:ss", CultureInfo.InvariantCulture);
                                string formattedTime = dt.ToString("HH:mm:ss");

                                string strDateTime = item.Date.ToString("MM/dd/yyyy") + " " + formattedTime;
                                DateTime dtime = DateTime.ParseExact(strDateTime, "MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                                //string isoFormat = dtime.ToString("yyyy-MM-ddTHH:mm:ss"); // ISO 8601 format

                                string rawTimeSrart = string.IsNullOrWhiteSpace(item.作業開始) ? "00:00:00" : item.作業開始;
                                DateTime dtstart = DateTime.ParseExact(rawTimeSrart, "H:mm:ss", CultureInfo.InvariantCulture);
                                string formattedTimeStart = dtstart.ToString("HH:mm:ss");

                                string strStartTime = item.Date.ToString("MM/dd/yyyy") + " " + formattedTimeStart;
                                DateTime? dtStartTime = !string.IsNullOrWhiteSpace(strStartTime) ? DateTime.ParseExact(strStartTime, "MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture) : null;

                                string rawTimeEnd = string.IsNullOrWhiteSpace(item.作業完了) ? "00:00:00" : item.作業完了;
                                DateTime dtEnd = DateTime.ParseExact(rawTimeEnd, "H:mm:ss", CultureInfo.InvariantCulture);
                                string formattedTimeEnd = dtEnd.ToString("HH:mm:ss");

                                string strEndTime = item.Date.ToString("MM/dd/yyyy") + " " + formattedTimeEnd;
                                DateTime? dtEndTime = !string.IsNullOrWhiteSpace(strEndTime) ? DateTime.ParseExact(strEndTime, "MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture) : null;

                                string strDueate = item.納期.Replace(".", "/");

                                string[] formats = { "yyyy/M/d", "yyyy/MM/dd" };
                                DateTime? dtDueate = null;

                                if (!string.IsNullOrWhiteSpace(strDueate) &&
                                    DateTime.TryParseExact(strDueate, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result))
                                {
                                    dtDueate = result;
                                }

                                data.AchievementRegistrationDate = item.Date;
                                data.AchievementRegistrationTime = dtime;
                                data.ProcessingStartTime = dtStartTime;
                                data.ProcessingEndTime = dtEndTime;
                                data.ProcessingTime = ConverTimetoInt(item.所要時間);
                                data.ProgressRate = item.進捗率;
                                data.StandardTime = ConverTimetoInt(item.標準時間);
                                data.ProcessingStopTime = ConverTimetoInt(item.作業停止時間);
                                data.LossStopTime = ConverTimetoInt(item.ロス停止時間);
                                data.SlipNo = item.伝票No;
                                data.CustomerName = item.得意先名;
                                data.DueDate = dtDueate;
                                data.ProductName1 = item.品名1;
                                data.ProductName2 = item.品名2;
                                data.Value = int.TryParse(item.個数, out var value) ? value : 0;
                                data.Operator1 = item.オペレータ1;
                                data.Operator2 = item.オペレータ2;
                                data.Operator3 = item.オペレータ3;
                                data.MeasurementInspection = int.Parse(item.測定検査);
                                data.Changeover = int.Parse(item.段取替え);
                                data.CAD = int.Parse(item.CAD);
                                data.EquipmentFailure = int.Parse(item.設備故障);
                                data.Cleaning = int.TryParse(item.清掃, out var temp) ? temp : 0;
                                data.RestTime = int.TryParse(item.休憩時間, out var tempRestTime) ? tempRestTime : 0;
                                data.fileName = fileName;
                                data.hmiNo = hmiNo;
                                data.fileDateTime = fileDateTime;
                                lstData.Add(data);
                            }
                            var response = await _workerServiceClient.ImportListTrnOperationResult(lstData);
                            _logger.LogInfo("Move File line 3 " + file.FullName);
                            _logger.LogInfo(JsonConvert.SerializeObject(lstData));
                            _logger.LogInfo(JsonConvert.SerializeObject(response));
                            MoveCsv(directory, directory_out, file.Name);

                        }
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            DateTime importTime = DateTime.Now;

                            // Tạo tham số tìm kiếm
                            var searchParam = new MstMachineSearchImpl
                            {
                                HMI_NO = hmiNo,
                                MACHINE_NO = "",
                                MACHINE_NAME = ""
                            };

                            // Serialize tham số thành JSON
                            var jsonParam = JsonConvert.SerializeObject(searchParam);

                            // Gửi yêu cầu POST đến API
                            var responseString = await _workerServiceClient.SearchMstMachine(jsonParam);

                            // Xử lý dữ liệu phản hồi
                            var responseMachine = JsonConvert.DeserializeObject<GenericResponseResult<MstMachineDto>>(responseString);

                            var ListdataMachine = responseMachine.ListData;

                            var lstData = new List<TrnImportHistoryDto>();
                            var data = new TrnImportHistoryDto();
                            data.FileName = fileName;
                            if (ListdataMachine.Count > 0)
                            {
                                data.MachineNo = ListdataMachine[0].MachineNo;
                                data.FactoryCd = ListdataMachine[0].InstallationLocationCd;
                            }
                            else
                            {
                                data.MachineNo = "-1";
                                data.FactoryCd = -1;
                            }
                            data.Status = "失敗";
                            data.Note = "データ不正の為、データのインポートに失敗しました。";
                            data.Flag = "0";
                            data.ImportTime = importTime;
                            lstData.Add(data);
                            var response = await _workerServiceClient.ImportListTrnImportHistory(lstData);
                            _logger.LogError(ex, "Error while read file " + file.Name);
                            _logger.LogError(ex, "Error while read file " + ex.Message);

                        }
                        catch
                        {
                            _logger.LogError(ex, "Error importHisory File Error " + ex.Message);
                        }

                    }
                    finally
                    {
                    }
                }
            }
        }


        public int ConverTimetoInt(string strTime)
        {
            try
            {
                TimeSpan ts = TimeSpan.Parse(strTime);
                int totalSeconds = (int)ts.TotalSeconds;
                return totalSeconds;
            }
            catch
            {
                return 0;
            }
        }

        public void MoveCsv(string fPath, string tPath, string fileName)
        {
            try
            {
                if (!Directory.Exists(tPath))
                {
                    Directory.CreateDirectory(tPath);
                }
                string _fPath = Path.Combine(fPath, fileName);
                string _tPath = Path.Combine(tPath, fileName);
                if (File.Exists(_tPath))
                {
                    string nameWithoutExt = Path.GetFileNameWithoutExtension(fileName);
                    string ext = Path.GetExtension(fileName);
                    string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                    _tPath = Path.Combine(tPath, $"{nameWithoutExt}_{timestamp}{ext}");
                }

                // Di chuyển file
                File.Move(_fPath, _tPath);
                _logger.LogInfo("Move File " + fileName);
            }
            catch (IOException ex)
            {
                _logger.LogInfo("Errol move " + ex.Message);
            }
        }

        public async Task ScanFolder(FolderPaths folderPaths)
        {
            try
            {
                _folderPath1 = folderPaths.Folder1;

                // Check if the directories exist
                if (!Directory.Exists(_folderPath1))
                {
                    _logger.LogError($"Folder đường dẫn không tồn tại: \"{_folderPath1}\"");
                    return;
                }
                ProcessDailyLogFolders(_folderPath1);
                ProcessProdLogFolders(_folderPath1);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in ScanFolder: {ex.Message}");
            }
        }

        private FileSystemWatcher WatchFolder(string folderPath, string extension = "*.csv")
        {
            try
            {
                var watcher = new FileSystemWatcher(folderPath, extension);
                watcher.IncludeSubdirectories = true;
                watcher.EnableRaisingEvents = true;
                watcher.Created += OnCreatedCsv;
                _logger.LogInfo($"Started watching folder: {folderPath}");
                return watcher;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in WatchFolder: {ex.Message}");
                return null;
            }
        }


        private void IsFileReady(string filePath)
        {
            const int maxRetries = 10;
            const int delayMs = 500;

            for (int i = 0; i < maxRetries; i++)
            {
                try
                {
                    using (FileStream fs = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        // File đã sẵn sàng, xử lý nó
                        fs.Close();
                        return;
                    }
                }
                catch (IOException)
                {
                    Thread.Sleep(delayMs); // chờ rồi thử lại
                }
            }
        }

        private async void OnCreatedCsv(object sender, FileSystemEventArgs e)
        {

            string filePath = e.FullPath;
            await Task.Delay(1000).ContinueWith(_ => IsFileReady(e.FullPath));
            try
            {
                // Kiểm tra file đã sẵn sàng
                await ScanFolder(filePath);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Lỗi khi xử lý file mới tạo: {ex.Message}");
            }
        }

        private void StopWatcher(ref FileSystemWatcher watcher)
        {
            if (watcher != null)
            {
                watcher.EnableRaisingEvents = false;
                watcher.Dispose();
                watcher = null;
                _logger.LogInfo("Stopped watching folder.");
            }
        }

        public void Stop()
        {
            StopWatcher(ref _watcher);
        }

        #endregion
    }
}
