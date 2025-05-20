using PlastMB.Application.CustomModels;
using PlastMB.Application.CustomModels.Dtos;
using PlastMB.Application.CustomModels.SearchConditions;
using PlastMB.Application.Enum;
using PlastMB.Application.Interface;
using PlastMB.Domain.Entity;
using PlastMB.Domain.Interface;
using PlastMB.Utils;
using PlastMB.Utils.LogUtils;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Reflection.PortableExecutable;

namespace PlastMB.Application.Services
{
    public class TrnOperationResultServiceImpl : ITrnOperationResultService
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
        public TrnOperationResultServiceImpl(IBaseRepositoryWrapper repo
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
        public async Task<GenericResponseResult<TrnOperationResultDto>> SearchTrnOperationResult(TrnOperationResultSearchImpl condition, bool blnExport = false)
        {

            var iqResult = from e in _repo.TrnOperationResult.GetAll().AsNoTracking()
                           join m in _repo.TrnImportHistoryDetail.GetAll().AsNoTracking()
                               on new { e.MachineNo, e.FactoryCd, ID = e.Id.ToString() }
                               equals new { m.MachineNo, m.FactoryCd, ID = m.IDData.ToString() }
                           where (string.IsNullOrEmpty(condition.FILENAME)) || m.FileName.ToLower().Contains(condition.FILENAME.ToLower())
                                 && m.ImportTime == condition.IMPORTTIME
                           select _mapper.Map<TrnOperationResultDto>(e);

            //count total record
            int total1 = await iqResult.CountAsync();
            // int total2 = await iqResult2.CountAsync();
            int total = total1;// + total2;

            if (total <= 0)
            {
                return new GenericResponseResult<TrnOperationResultDto>();
            }
            //count total page
            int totalPage1 = (int)Math.Ceiling(total1 / (double)condition.PageSize);
            int totalPage = (int)Math.Ceiling(total / (double)condition.PageSize);

            //Index start from 0
            if (totalPage - 1 < condition.PageIndex)
            {
                condition.PageIndex = totalPage - 1;
            }

            List<TrnOperationResultDto> lstData = new List<TrnOperationResultDto>();
            List<TrnOperationResultDto> lstData2 = new List<TrnOperationResultDto>();
            //export get all data
            lstData = await iqResult.ToListAsync();

            return new GenericResponseResult<TrnOperationResultDto>(lstData, total, condition.PageIndex, condition.PageSize);
        }
        #endregion
        #region CRUD

        public async Task<ServiceResult> ImportListTrnOperationResult(List<TrnOperationResultDto> data)
        {
            DateTime importTime = DateTime.Now;
            var fileName = "";

            try
            {
                // Vui lòng kiểm tra lại dữ liệu
                if (data is null || data.Count == 0)
                {
                    _logger.LogError($" không có dữ liệu gửi lên.");
                    return new ServiceResultError("Không có dữ liệu gửi lên!");
                }

                var entities = _mapper.Map<List<TrnOperationResult>>(data);
                var entitiesDto = _mapper.Map<List<TrnOperationResultDto>>(data);
                fileName = entitiesDto.FirstOrDefault()?.fileName;
                var machineNo = entities.FirstOrDefault()?.MachineNo;
                var hmiNo = entitiesDto.FirstOrDefault()?.hmiNo;
                var importFileResult = (from e in _repo.TrnImportHistory.GetAll().AsNoTracking()
                                        where e.FileName.Equals(fileName)
                                        orderby e.ImportTime descending
                                        select e).FirstOrDefault();

                if (importFileResult != null)
                {
                    var iqResult1 = from e in _repo.TrnOperationResult.GetAll().AsNoTracking()
                                    join m in _repo.TrnImportHistoryDetail.GetAll().AsNoTracking()
                                        on new { e.MachineNo, e.FactoryCd, ID = e.Id.ToString() }
                                        equals new { m.MachineNo, m.FactoryCd, ID = m.IDData.ToString() }
                                    where m.FileName.ToLower().Contains(importFileResult.FileName.ToLower())
                                    //&& m.ImportTime.ToString().ToLower().Contains(importFileResult.ImportTime.ToString().ToLower())
                                    select _mapper.Map<TrnOperationResult>(e);

                    List<TrnOperationResult> TrnOperationResult = await iqResult1.ToListAsync();
                    importFileResult.Note = "ファイルのデータは上書きされました。";
                    await _repo.TrnImportHistory.UpdateAsync(importFileResult);
                    await _repo.TrnOperationResult.DeleteAsync(TrnOperationResult);
                    await _repo.SaveAync();
                }
                // kiểm tra mã máy có tồn tại không
                var iqResult = from e in _repo.MstMachine.GetAll().AsNoTracking()
                               where e.HmiNo.Equals(hmiNo)
                               select e;

                List<MstMachine> lstData = await iqResult.ToListAsync();

                if (lstData.Count > 0)
                {
                    var matchedMachine = lstData.FirstOrDefault(m => m.HmiNo == hmiNo);
                    //if (matchedMachine != null)
                    foreach (var entity in entities)
                    {
                        entity.MachineNo = matchedMachine?.MachineNo ?? "";
                        entity.FactoryCd = matchedMachine?.InstallationLocationCd ?? 0;
                        if (null == machineNo)
                        {
                            machineNo = matchedMachine?.MachineNo;
                        }
                    }

                    await _repo.TrnOperationResult.InsertAsync(entities);
                    await _repo.SaveAync();
                    List<TrnImportHistoryDetail> listTrnImportHistoryDetails = new List<TrnImportHistoryDetail>();

                    foreach (var entity in entities)
                    {
                        TrnImportHistoryDetail trnImportHistoryDetail = new TrnImportHistoryDetail();
                        trnImportHistoryDetail.IDData = entity.Id;
                        trnImportHistoryDetail.MachineNo = entity.MachineNo;
                        trnImportHistoryDetail.FactoryCd = entity.FactoryCd;
                        trnImportHistoryDetail.FileName = fileName;
                        trnImportHistoryDetail.ImportTime = importTime;
                        listTrnImportHistoryDetails.Add(trnImportHistoryDetail);
                    }
                    await _repo.TrnImportHistoryDetail.InsertAsync(listTrnImportHistoryDetails);
                    await _repo.SaveAync();
                    var history = new TrnImportHistory();
                    history.FileName = fileName;
                    history.MachineNo = machineNo;
                    history.FactoryCd = matchedMachine?.InstallationLocationCd ?? 0;
                    history.Status = "成功";
                    history.Note = "データのインポートに成功しました。";
                    history.Flag = "0";
                    history.RecordCount = entities.Count;
                    history.ImportTime = importTime;
                    await _repo.TrnImportHistory.InsertAsync(history);
                    await _repo.SaveAync();
                    return new ServiceResultSuccess($"Thêm dữ liệu thành công !");

                }
                else
                {
                    var history = new TrnImportHistory();
                    history.FileName = fileName;
                    history.MachineNo = "";
                    history.FactoryCd = -1;
                    history.Status = "失敗";
                    history.Note = "設備Noが存在しない為、データのインポートに失敗しました。";
                    history.Flag = "0";
                    history.ImportTime = importTime;
                    await _repo.TrnImportHistory.InsertAsync(history);
                    await _repo.SaveAync();
                    // Import Lỗi 
                    return new ServiceResultSuccess($"Thêm dữ liệu thành công do Không có mã máy trong Database !");

                }

            }
            catch (Exception ex)
            {
                var history = new TrnImportHistory();
                history.FileName = fileName;
                history.MachineNo = "";
                history.FactoryCd = -1;
                history.Status = "失敗";
                history.Note = "設備Noが存在しない為、データのインポートに失敗しました。";
                history.Flag = "0";
                history.ImportTime = importTime;
                await _repo.TrnImportHistory.InsertAsync(history);
                await _repo.SaveAync();
                _logger.LogError(ex);
                return new ServiceResultError("Lỗi khi thêm list dữ liệu: " + ex.Message);
            }
        }
        #endregion
    }
}
