using Astemo.Application.CustomModels;
using Astemo.Application.CustomModels.Others;
using Astemo.Application.CustomModels.SearchConditions;
using Astemo.Application.Interface;
using Astemo.Domain.Interface;
using Astemo.Utils;
using Astemo.Utils.LogUtils;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Diagnostics;

namespace Astemo.Application.Services
{
    public class BackupServiceImpl : IBackupService
    {
        #region Properties
        //Repo
        private readonly IBaseRepositoryWrapper _repo;
        //Get config from appsettings.json if need
        private readonly IConfiguration _configuration;
        //Mapping model to entity
        private readonly IMapper _mapper;
        //Log
        private readonly ILoggerManager _logger;
        #endregion
        #region Constructor
        public BackupServiceImpl(IBaseRepositoryWrapper repo
            , IConfiguration configuration
            , IMapper mapper
            , ILoggerManager logger)
        {
            _repo = repo;
            _configuration = configuration;
            _mapper = mapper;
            _logger = logger;
        }
        #endregion
        #region Search
        public async Task<GenericResponseResult<FileBackupInfo>> SearchFileBackup(FileBackupSearchImpl condition)
        {
            var backupFolder = _configuration["BackupFolder"];
            var fromDate = DateUtils.GetDate(condition.FromDate) ?? DateTime.MinValue;
            var toDate = DateUtils.GetDate(condition.ToDate) ?? DateTime.MaxValue;
            if (Directory.Exists(backupFolder))
            {
                var dInfo = new DirectoryInfo(backupFolder);
                var files = dInfo.GetFiles("*.*").Where(x => string.IsNullOrEmpty(condition.FileName) || x.Name.ToLower().Contains(condition.FileName.ToLower()) && (x.LastWriteTime.Date >= fromDate && x.LastWriteTime.Date <= toDate));
                var total = files.Count();
                if (total == 0)
                {
                    return new GenericResponseResult<FileBackupInfo>();
                }
                else
                {
                    int totalPage = (int)Math.Ceiling(total / (double)condition.PageSize);
                    if (totalPage - 1 < condition.PageIndex)
                    {
                        condition.PageIndex = totalPage - 1;
                    }
                    var lstData = files.Skip((condition.PageIndex) * condition.PageSize).Take(condition.PageSize).Select(x => new FileBackupInfo
                    {
                        FileName = x.Name,
                        CreatedDate = x.LastWriteTime,
                        FileSize = x.Length
                    }).ToList();

                    return new GenericResponseResult<FileBackupInfo>(lstData, total, condition.PageIndex, condition.PageSize);
                }
            }
            else
            {
                Directory.CreateDirectory(backupFolder);
                return new GenericResponseResult<FileBackupInfo>();
            }
        }
        #endregion
        #region Others
        public async Task<ServiceResult> Database()
        {
            try
            {
                var strFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Scripts", "backup.bat");
                Process p = new Process();
                // Redirect the output stream of the child process.
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.FileName = strFile;
                p.Start();
                // Do not wait for the child process to exit before
                // Read the output stream first and then wait.
                string output = await p.StandardOutput.ReadToEndAsync();
                await p.WaitForExitAsync();
                return new ServiceResultSuccess(output);
            }
            catch (Exception ex)
            {
                _logger.LogError("Backup: " + ex.Message);
                return new ServiceResultError("Lỗi xảy ra khi backup: " + ex.Message);
            }
        }
        #endregion
    }
}
