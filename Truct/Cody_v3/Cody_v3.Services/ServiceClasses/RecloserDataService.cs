using Cody_v3.Repositories.Entities;
using Cody_v3.Repositories.Interfaces;
using Cody_v3.Repositories.Paging;
using Cody_v3.Services.DTOs;
using Cody_v3.Services.Generics;
using Cody_v3.Services.Helpers;
using Cody_v3.Services.Hubs;
using Cody_v3.Services.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Build.Tasks;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cody_v3.Services.ServiceClasses
{
    public class RecloserDataService : GenericService<RecloserData>, IRecloserDataService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IDapperGenericService dapper;
        private readonly IHubContext<ExcelHandlingHub, IExcelHandlingClient>_hubcontext;
        public RecloserDataService(IGenericRepository<RecloserData> repository, IWebHostEnvironment environment, IDapperGenericService dapperGenericService, IHubContext<ExcelHandlingHub, IExcelHandlingClient> hubcontext) : base(repository)
        {
            _environment = environment;
            this.dapper = dapperGenericService;
            _hubcontext=hubcontext;
        }

        public async Task<List<RecloserData>> GetAllCurrent()
        {
            return (await repository.GetByCondition(c => c.IsDeleted != true)).ToList();
        }

        public async Task<List<OverloadReportResponseDTO>> GetOverloadReportResponseDTOs(ReportRequestDTO dto)
        {
            var Department = dto.DepartmentId == Guid.Empty ? null : dto.DepartmentId;
            return (await dapper.ExecProcedureDataAsync<OverloadReportResponseDTO>("USP_RecloserData_GetReport", new { from = dto.From, to = dto.To, DepartmentId=Department })).ToList();
        }

        public async Task<List<RecloserDataResponseDTO>> GetRecloserDataResponseDTOs(ReportRequestDTO dto)
        {
            var Department = dto.DepartmentId == Guid.Empty ? null : dto.DepartmentId;
           return (await dapper.ExecProcedureDataAsync<RecloserDataResponseDTO>("USP_RecloserData_GetForTraceBack", new { from = dto.From, to = dto.To, DepartmentId = Department })).ToList();
        }

        public async Task<PageResult<RecloserDataResponseDTO>> GetWithPaging(int currentPage, int pageSize, string actionName)
        {
            var pageResultRecloseDatas= await GetWithPaging(currentPage, pageSize, w=>w.IsDeleted!=true, o=>o.OrderByDescending(o=>o.CreatedDate), "Recloser,Recloser.Department");
            var recloserDataDTOs = pageResultRecloseDatas.Results.Select(s=> new RecloserDataResponseDTO()
            {
                DepartmentName= s.Recloser.Department.DepartmentName,
                ExcelFileName= s.ExcelFileName,
                SheetName = s.SheetName,
                Date =s.Date,
                Time=s.Time ,
                RecloserQualify = s.Recloser.RecloserQualify,
                U_kV = s.U_kV,
                I_A = s.I_A,
                P_MW =s.P_MW ,
                Q_MVar =s.Q_MVar ,
                COS_φ =s.COS_φ ,
            }).ToList();
            PageResult<RecloserDataResponseDTO> pageResult = new PageResult<RecloserDataResponseDTO>();
            pageResultRecloseDatas.CopyPropertiesTo(pageResult);
            pageResult.Results = recloserDataDTOs;
            pageResult.ActionName = actionName;

            return pageResult;
        }

        public async Task<int> ImportData(IFormFileCollection excelFiles, string folderSave)
        {
            await _hubcontext.Clients.All.ReceiveMessage($"Bắt đầu xử lý {excelFiles.Count} file(s) (0%)",0);
            var listResult = new List<RecloserDataImportDTO>();
            int z = 0;
            //await Clients.Caller.SendAsync("ReceiveMessage", $"Starting handle {excelFiles.Count} file(s) ({z}%)"); 
            foreach (IFormFile excelFile in excelFiles)
            {
                var percent = Math.Round(z * 100.0 / excelFiles.Count, 2);
                z++; await _hubcontext.Clients.All.ReceiveMessage($"File {z}/{excelFiles.Count} ({percent}%)", percent);
                //await Clients.Caller.SendAsync("ReceiveMessage", $"Starting handle {z}/{excelFiles.Count} file(s) ({Math.Round(z*100.0/excelFiles.Count,2)}%)");
                var dateStr = Path.GetFileNameWithoutExtension(excelFile.FileName).Replace("Recloser", "");
                var date = dateStr.ToExacDateTime();

                var data = await FileHelper.GetDataExcel(Path.Combine(folderSave, excelFile.FileName));

                foreach (DataTable dt in data.Tables)
                {
                    string[] columnNames = dt.Columns.Cast<DataColumn>().Where(w => w.ColumnName.Contains("Recloser")).Select(x => x.ColumnName.Trim()).ToArray();
                    int countCol = 0;
                    int maxCount = 5;
                    string depertmentName = dt?.TableName?.Trim();
                    foreach (var col in columnNames)
                    {
                        for (int i = 1; i < dt.Rows.Count; i++)
                        {
                            DataRow dr = dt.Rows[i];
                            listResult.Add(new RecloserDataImportDTO()
                            {
                                SheetName = depertmentName,
                                ExcelFileName= Path.GetFileName(excelFile.FileName),
                                Recloser = col,
                                Date = date,
                                Time = dr[0]?.ToString(),
                                U_kV = dr[maxCount * countCol + 1].ToDecimal(),
                                I_A = dr[maxCount * countCol + 2].ToDecimal(),
                                P_MW = dr[maxCount * countCol + 3].ToDecimal(),
                                Q_MVar = dr[maxCount * countCol + 4].ToDecimal(),
                                COS_φ = dr[maxCount * countCol + 5].ToDecimal()
                            });
                        }
                        countCol = 1;
                    }
                }
            }

            var json = listResult.ToJSON();
            await _hubcontext.Clients.All.ReceiveMessage($"Đã xử lý được {listResult.Count.ToString("#,#")} dòng dữ liệu trong số {excelFiles.Count} file. Đang chuẩn bị lưu...", 100);
            return await ExecProcNonqueryAsync("dbo.USP_RecloserData_Import", new{ json });
        }

        public async Task<string> UploadImage(IFormFile image)
        {
            var filePath = Path.Combine(_environment.WebRootPath, FileHelper.SUMMERNOTE_UPLOAD_FILE_PATH);
            try
            {
                var fileName=await FileHelper.UploadFile(image, filePath);
                return Path.Combine(FileHelper.SUMMERNOTE_UPLOAD_FILE_PATH,fileName);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public async Task<bool> Uploads(IFormFileCollection excelFiles, string folderSave, bool generateFileName = false)
        {

            var filePath = Path.Combine(_environment.WebRootPath, FileHelper.EXCEL_UPLOAD_FILE_PATH);
            try
            {
                return await FileHelper.Upload(excelFiles, filePath);
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
