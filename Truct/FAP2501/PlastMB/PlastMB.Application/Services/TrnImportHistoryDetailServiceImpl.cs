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
    public class TrnImportHistoryDetailServiceImpl : ITrnImportHistoryDetailService
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
        public TrnImportHistoryDetailServiceImpl(IBaseRepositoryWrapper repo
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


        public async Task<GenericResponseResult<TrnImportHistoryDetailDto>> SearchImportHistoryDetail(TrnImportHistoryDetailSearchImpl condition, bool blnExport = false)
        {
            DateTime? fromDate = DateUtils.GetDate(condition.FromDate);
            DateTime? toDate = DateUtils.GetDate(condition.ToDate);

            var iqResult = from m in _repo.TrnImportHistoryDetail.GetAll().AsNoTracking()
                           where (string.IsNullOrEmpty(condition.FileName) || m.FileName.ToLower().Contains(condition.FileName.ToLower()))
                           && (string.IsNullOrEmpty(condition.MachineNo) || m.MachineNo.ToLower().Contains(condition.MachineNo.ToLower()))
                           && (string.IsNullOrEmpty(condition.FactoryCd) || m.FactoryCd.ToString().ToLower().Contains(condition.FactoryCd.ToLower()))

                           //orderby m.CreateTime ascending, (m.UpdateTime ?? m.CreateTime) ascending
                           select _mapper.Map<TrnImportHistoryDetailDto>(m);

            int total = await iqResult.CountAsync();

            if (total <= 0)
            {
                return new GenericResponseResult<TrnImportHistoryDetailDto>();
            }

            int totalPage = (int)Math.Ceiling(total / (double)condition.PageSize);
            //Index start from 0
            if (totalPage - 1 < condition.PageIndex)
            {
                condition.PageIndex = totalPage - 1;
            }

            List<TrnImportHistoryDetailDto> lstData = new List<TrnImportHistoryDetailDto>();

            //export get all data
            if (blnExport)
                lstData = await iqResult.ToListAsync();
            else
                lstData = await iqResult.Skip((condition.PageIndex) * condition.PageSize).Take(condition.PageSize).ToListAsync();

            return new GenericResponseResult<TrnImportHistoryDetailDto>(lstData, total, condition.PageIndex, condition.PageSize);
        }
        #endregion
        #region CRUD

        public async Task<ServiceResult> ImportListImportHistoryDetail(List<TrnImportHistoryDetailDto> data)
        {
            try
            {
                // Vui lòng kiểm tra lại dữ liệu
                if (data is null || data.Count == 0)
                {
                    _logger.LogError($"ImportEcuData: không có dữ liệu gửi lên.");
                    return new ServiceResultError("Không có dữ liệu gửi lên!");
                }

                //var entities = _mapper.Map<List<EcuData>>(data);
                var entities = _mapper.Map<List<TrnImportHistoryDetail>>(data);
                await _repo.TrnImportHistoryDetail.BulkInsertAsync(entities);
                await _repo.SaveAync();
                return new ServiceResultSuccess($"Thêm dữ liệu thành công!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return new ServiceResultError("Lỗi khi thêm list dữ liệu Ecu: " + ex.Message);
            }
        }

        public async Task<ServiceResult> DeteteImportHistory(int id)
        {
            try
            {

                var entity = await _repo.MstUser.GetAsync(id);
                if (entity is null) return new ServiceResultError("Nhân viên không tồn tại!");
                if (entity.ValidFlg != (int)EnumCommon.Status.Valid) return new ServiceResultError("Nhân viên không hợp lệ!");

                entity.ValidFlg = (int)EnumCommon.Status.Invalid;
                entity.upd_dt = DateTime.Now;
                await _repo.MstUser.UpdateAsync(entity);
                await _repo.SaveAync();
                return new ServiceResultSuccess("Xóa nhân viên thành công!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return new ServiceResultError("Lỗi khi thêm user: " + ex.Message);
            }
        }






        #endregion
    }
}
