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
using System.Net.WebSockets;
using System.Linq;

namespace PlastMB.Application.Services
{
    public class TrnOperationOeeServiceImpl : ITrnOperationOeeService
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
        public TrnOperationOeeServiceImpl(IBaseRepositoryWrapper repo
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
        public async Task<GenericResponseResult<TrnOperationOeeDto>> SearchTrnOperationOee(TrnOperationOeeSearchImpl condition, bool blnExport = false)
        {

            var iqResult = from e in _repo.TrnOperationOee.GetAll().AsNoTracking()
                           join m in _repo.TrnImportHistoryDetail.GetAll().AsNoTracking()
                               on  new { e.MachineNo, e.FactoryCd, ID = e.Id.ToString()} 
                               equals  new { m.MachineNo , m.FactoryCd, ID = m.IDData.ToString()} 
                           where m.FileName.ToLower().Contains(condition.FILENAME.ToLower())
                                 && m.ImportTime == condition.IMPORTTIME
                           select _mapper.Map<TrnOperationOeeDto>(e);

            //count total record
            int total1 = await iqResult.CountAsync();
            // int total2 = await iqResult2.CountAsync();
            int total = total1;// + total2;

            if (total <= 0)
            {
                return new GenericResponseResult<TrnOperationOeeDto>();
            }
            //count total page
            int totalPage1 = (int)Math.Ceiling(total1 / (double)condition.PageSize);
            int totalPage = (int)Math.Ceiling(total / (double)condition.PageSize);
            //Index start from 0
            if (totalPage - 1 < condition.PageIndex)
            {
                condition.PageIndex = totalPage - 1;
            }
            List<TrnOperationOeeDto> lstData = new List<TrnOperationOeeDto>();
            List<TrnOperationOeeDto> lstData2 = new List<TrnOperationOeeDto>();
            //export get all data
            lstData = await iqResult.ToListAsync();

            return new GenericResponseResult<TrnOperationOeeDto>(lstData, total, condition.PageIndex, condition.PageSize);
        }
        #endregion
        #region CRUD

        public async Task<ServiceResult> ImportListTrnOperationOee(List<TrnOperationOeeDto> data)
        {
            var fileName = "";
            DateTime importTime = DateTime.Now;
            try
            {
                // Vui lòng kiểm tra lại dữ liệu
                if (data is null || data.Count == 0)
                {
                    _logger.LogError($"Import OPERATION_OEE: không có dữ liệu gửi lên.");
                    return new ServiceResultError("Không có dữ liệu gửi lên!");
                }

                var entities = _mapper.Map<List<TrnOperationOee>>(data);
                var entitiesDto = _mapper.Map<List<TrnOperationOeeDto>>(data);
                var machineNo = entities.FirstOrDefault()?.MachineNo;
                fileName = entitiesDto.FirstOrDefault()?.fileName;
                var iqResult = from e in _repo.MstMachine.GetAll().AsNoTracking()
                               where e.MachineNo.Equals(machineNo)
                               select e;

                var importFileResult = (from e in _repo.TrnImportHistory.GetAll().AsNoTracking()
                                      where e.FileName.Equals(fileName)
                                      orderby e.ImportTime descending
                                      select e).FirstOrDefault();

                if(importFileResult!= null)
                {
                    var iqResult1 = from e in _repo.TrnOperationOee.GetAll().AsNoTracking()
                                   join m in _repo.TrnImportHistoryDetail.GetAll().AsNoTracking()
                                       on new { e.MachineNo, e.FactoryCd, ID = e.Id.ToString() }
                                       equals new { m.MachineNo, m.FactoryCd, ID = m.IDData.ToString() }
                                   where m.FileName.ToLower().Contains(importFileResult.FileName.ToLower())
                                         //&& m.ImportTime.ToString().ToLower().Contains(importFileResult.ImportTime.ToString().ToLower())
                                   select _mapper.Map<TrnOperationOee>(e);

                    List<TrnOperationOee> trnOperationOees = await iqResult1.ToListAsync();

                    importFileResult.Note = "ファイルのデータは上書きされました。";
                    await _repo.TrnImportHistory.UpdateAsync(importFileResult);
                    await _repo.TrnOperationOee.DeleteAsync(trnOperationOees);
                    await _repo.SaveAync();
                }

                List<MstMachine> lstData = await iqResult.ToListAsync();

                if (lstData.Count > 0)
                {
                    var matchedMachine = lstData.FirstOrDefault(m => m.MachineNo == machineNo);
                    foreach (var entity in entities)
                    {
                        entity.FactoryCd = matchedMachine?.InstallationLocationCd ?? 0;
                    }
                    await _repo.TrnOperationOee.InsertAsync(entities);
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
                    history.Flag = "1";
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
                    history.FactoryCd = 0;
                    history.Status = "失敗";
                    history.Note = "設備Noが存在しない為、データのインポートに失敗しました。";
                    history.Flag="1";
                    history.ImportTime=importTime;
                    await _repo.TrnImportHistory.InsertAsync(history);
                    await _repo.SaveAync();
                    // Import Lỗi 
                    return new ServiceResultSuccess($"Thêm dữ liệu không thành công do Không có mã máy trong Database !");

                }

            }
            catch (Exception ex)
            {

                var history = new TrnImportHistory();
                history.FileName = fileName;
                history.MachineNo = "";
                history.FactoryCd = 0;
                history.Status = "失敗";
                history.Note = "設備Noが存在しない為、データのインポートに失敗しました。";
                history.Flag = "1";
                history.ImportTime = importTime;
                await _repo.TrnImportHistory.InsertAsync(history);
                await _repo.SaveAync();
                _logger.LogError(ex);
                return new ServiceResultError("Lỗi khi thêm list dữ liệu OPERATION_OEE: " + ex.Message);
            }
        }
        #endregion
    }
}
