using Worker.Application.CustomModels.Dtos;
using Worker.Application.Interface;
using Astemo.Utils;
using Astemo.Utils.LogUtils;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Worker.Application.CustomModels.Others;
using Worker.Application.Enum;
using Newtonsoft.Json;

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
        private readonly string _fileNameLine3;
        private readonly string _fileNameLine4;
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
            _fileNameLine3 = _configuration["FileNameLine3"];
            _fileNameLine4 = _configuration["FileNameLine4"];
        }
        #endregion

        #region Other

        public async Task ScanFolderLine4()
        {
            var folder = _configuration["FolderScanLine4"];

            if (!Directory.Exists(folder)) return;

            _logger.LogInfo($"Scan line 4: {folder}");
            DirectoryInfo df = new DirectoryInfo(folder);
            IEnumerable<FileInfo> files = df.GetFiles().Where(x => x.FullName.ToLower().EndsWith(".csv"));

            foreach (FileInfo file in files)
            {
                if (!IsValidFileNameLine4(file.Name))
                    continue;

                try
                {
                    _logger.LogInfo("Reading file line 4 " + file.FullName);
                    var dataCsv = CsvUtils.ParseCsvCommon<EcuCsvDataLine4>(file.FullName);
                    if (dataCsv.Any())
                    {
                        var lstData = new List<EcuDataDto>();
                        foreach (var item in dataCsv)
                        {
                            //Không lấy các bản ghi ko có Laser
                            if (string.IsNullOrEmpty(item.LaserPrinting) || string.IsNullOrEmpty(item.LaserPrinting.Trim()))
                                continue;
                            //Folder Line4 lấy kết quả default là Ok
                            var data = new EcuDataDto();
                            data.DateManufacture = item.DateManufacture;
                            data.NgCode = "0000";
                            data.Result = (int)EnumCommon.Status.Valid;
                            data.HUCode = item.HUCode;
                            data.LaserPrinting = item.LaserPrinting;
                            lstData.Add(data);
                        }
                        var response = await _workerServiceClient.ImportListEcuData(lstData);
                        _logger.LogInfo(JsonConvert.SerializeObject(lstData));
                        _logger.LogInfo(JsonConvert.SerializeObject(response));
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while read file line 4 " + file.Name);
                }
                finally
                {
                    _logger.LogInfo("Delete file line 4 " + file.FullName);
                    File.Delete(file.FullName);
                }
            }
        }

        public async Task ScanFolderLine3()
        {
            var folder = _configuration["FolderScanLine3"];

            if (!Directory.Exists(folder)) return;

            _logger.LogInfo($"======================================================================");
            _logger.LogInfo($"Scan line 3: {folder}");
            DirectoryInfo df = new DirectoryInfo(folder);
            IEnumerable<FileInfo> files = df.GetFiles().Where(x => x.FullName.ToLower().EndsWith(".csv"));

            foreach (FileInfo file in files)
            {
                if (!IsValidFileNameLine3(file.Name))
                    continue;

                try
                {
                    _logger.LogInfo("Reading file line 3 " + file.FullName);
                    var dataCsv = CsvUtils.ParseCsvCommon<EcuCsvDataLine3>(file.FullName);
                    if (dataCsv.Any())
                    {
                        var lstData = new List<EcuDataDto>();
                        foreach (var item in dataCsv)
                        {
                            //Không lấy các bản ghi ko có Laser
                            if (string.IsNullOrEmpty(item.LaserPrinting) || string.IsNullOrEmpty(item.LaserPrinting.Trim()))
                                continue;

                            //Folder Line3 lấy kết quả theo Csv
                            var data = new EcuDataDto();
                            data.DateManufacture = item.DateManufacture;
                            data.NgCode = item.NgCode;
                            data.Result = item.Result == "OK" ? (int)EnumCommon.Status.Valid : (int)EnumCommon.Status.Invalid;
                            data.HUCode = item.HUCode;
                            data.LaserPrinting = item.LaserPrinting;
                            lstData.Add(data);
                        }
                        var response = await _workerServiceClient.ImportListEcuData(lstData);
                        _logger.LogInfo(JsonConvert.SerializeObject(lstData));
                        _logger.LogInfo(JsonConvert.SerializeObject(response));
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while read file line 3 " + file.Name);
                }
                finally
                {
                    _logger.LogInfo("Delete file line 3 " + file.FullName);
                    File.Delete(file.FullName);
                }
            }
        }

        /// <summary>
        /// Kiểm tra tên file có định dạng hợp lệ không
        /// </summary>
        /// <param name="fileName">Tên file</param>
        /// <remarks>Tên file có dạng: xxxxxxxx_yyyyMMdd</remarks>
        /// <returns></returns>
        private bool IsValidFileNameLine3(string fileName)
        {
            //Kiểm tra tên hợp lệ
            if (!fileName.StartsWith(_fileNameLine3)) return false;
            //Lấy tên ko có .csv
            var arrName = fileName.Substring(0, fileName.Length - 4).Split("_");
            var dateStr = arrName.LastOrDefault();
            var dateCsv = DateUtils.GetDate(dateStr, "yyyyMMdd");
            if (!dateCsv.HasValue) return false;
            //To do: other logic
            return true;
        }

        /// <summary>
        /// Kiểm tra tên file có định dạng hợp lệ không
        /// </summary>
        /// <param name="fileName">Tên file</param>
        /// <remarks>Tên file có dạng: xxxxxxxx_yyyyMMdd</remarks>
        /// <returns></returns>
        private bool IsValidFileNameLine4(string fileName)
        {
            //Kiểm tra tên hợp lệ
            if (!fileName.StartsWith(_fileNameLine4)) return false;
            //Lấy tên ko có .csv
            var arrName = fileName.Substring(0, fileName.Length - 4).Split("_");
            var dateStr = arrName.LastOrDefault();
            var dateCsv = DateUtils.GetDate(dateStr, "yyyyMMdd");
            if (!dateCsv.HasValue) return false;
            //To do: other logic
            return true;
        }

        #endregion
    }
}
