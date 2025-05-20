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

namespace PlastMB.Application.Services
{
    public class TrnImportHistoryServiceImpl : ITrnImportHistoryService
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
        public TrnImportHistoryServiceImpl(IBaseRepositoryWrapper repo
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


        public async Task<GenericResponseResult<TrnImportHistoryDto>> SearchImportHistory(TrnImportHistorySearchImpl condition, bool blnExport = false)
        {
            DateTime? fromDate = DateUtils.GetDate(condition.FromDate);
            DateTime? toDate = DateUtils.GetDate(condition.ToDate);

            var iqResult = from m in _repo.TrnImportHistory.GetAll().AsNoTracking()
                           join e in _repo.MstMachine.GetAll().AsNoTracking()
                                on m.MachineNo equals e.MachineNo into joinMachine
                           from e in joinMachine.DefaultIfEmpty() // Left join
                           where (string.IsNullOrEmpty(condition.FileName) || m.FileName.ToLower().Contains(condition.FileName.ToLower()))
                           && (string.IsNullOrEmpty(condition.MachineNo) || (e.MachineName != null && e.MachineName.ToLower().Contains(condition.MachineNo.ToLower())))
                           && (!fromDate.HasValue || m.ImportTime.Value.Date >= fromDate.Value.Date)
                           && (!toDate.HasValue || m.ImportTime.Value.Date <= toDate.Value.Date)
                           orderby m.ImportTime descending
                           select _mapper.Map<TrnImportHistoryDto>(m);

            int total = await iqResult.CountAsync();

            if (total <= 0)
            {
                return new GenericResponseResult<TrnImportHistoryDto>();
            }

            int totalPage = (int)Math.Ceiling(total / (double)condition.PageSize);
            //Index start from 0
            if (totalPage - 1 < condition.PageIndex)
            {
                condition.PageIndex = totalPage - 1;
            }

            List<TrnImportHistoryDto> lstData = new List<TrnImportHistoryDto>();

            //export get all data
            if (blnExport)
                lstData = await iqResult.ToListAsync();
            else
                lstData = await iqResult.Skip((condition.PageIndex) * condition.PageSize).Take(condition.PageSize).ToListAsync();

            return new GenericResponseResult<TrnImportHistoryDto>(lstData, total, condition.PageIndex, condition.PageSize);
        }
        #endregion
        #region CRUD

        public async Task<ServiceResult> ImportListImportHistory(List<TrnImportHistoryDto> data)
        {
            try
            {
                // Vui lòng kiểm tra lại dữ liệu
                if (data is null || data.Count == 0)
                {
                    _logger.LogError($" không có dữ liệu gửi lên.");
                    return new ServiceResultError("Không có dữ liệu gửi lên!");
                }

                //var entities = _mapper.Map<List<EcuData>>(data);
                var entities = _mapper.Map<List<TrnImportHistory>>(data);
                await _repo.TrnImportHistory.BulkInsertAsync(entities);
                await _repo.SaveAync();
                return new ServiceResultSuccess($"Thêm dữ liệu thành công!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return new ServiceResultError("Lỗi khi thêm list dữ liệu: " + ex.Message);
            }
        }
        #endregion
    }
}
