using Core.Application.CustomModels;
using Core.Application.CustomModels.Dtos;
using Core.Application.CustomModels.SearchConditions;
using Core.Application.Enum;
using Core.Application.Interface;
using Core.Domain.Entity;
using Core.Domain.Interface;
using Core.Utils.LogUtils;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Core.Application.Services
{
    public class MstDataServiceImpl : IMstDataService
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
        public MstDataServiceImpl(IBaseRepositoryWrapper repo
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
        public async Task<GenericResponseResult<MstDataDto>> SearchMasterData(MstDataSearchImpl condition, bool blnExport = false)
        {
            var iqResult = from m in _repo.MstData.GetAll().AsNoTracking()
                           where (string.IsNullOrEmpty(condition.Customer) || m.Customer.ToLower().Contains(condition.Customer.ToLower()))
                           && (string.IsNullOrEmpty(condition.AssemblyProductCode) || m.AssemblyProductCode.ToLower().Contains(condition.AssemblyProductCode.ToLower()))
                           && (string.IsNullOrEmpty(condition.Type) || m.Type.ToLower().ToLower().Contains(condition.Type.ToLower()))
                           && (string.IsNullOrEmpty(condition.HuSerial) || m.HuSerial.ToLower().Contains(condition.HuSerial.ToLower()))
                           && (string.IsNullOrEmpty(condition.InternalDrawingCode) || m.InternalDrawingCode.ToLower().Contains(condition.InternalDrawingCode.ToLower()))
                           && (string.IsNullOrEmpty(condition.ProductCode) || m.ProductCode.ToLower().Contains(condition.ProductCode.ToLower()))
                           && m.ValidFlg == (int)EnumCommon.Status.Valid
                           orderby m.ProductID ascending, (m.UpdateTime ?? m.CreateTime) ascending
                           select _mapper.Map<MstDataDto>(m);

            int total = await iqResult.CountAsync();

            if (total <= 0)
            {
                return new GenericResponseResult<MstDataDto>();
            }

            int totalPage = (int)Math.Ceiling(total / (double)condition.PageSize);
            //Index start from 0
            if (totalPage - 1 < condition.PageIndex)
            {
                condition.PageIndex = totalPage - 1;
            }

            List<MstDataDto> lstData = new List<MstDataDto>();

            //export get all data
            if (blnExport)
                lstData = await iqResult.ToListAsync();
            else
                lstData = await iqResult.Skip((condition.PageIndex) * condition.PageSize).Take(condition.PageSize).ToListAsync();

            return new GenericResponseResult<MstDataDto>(lstData, total, condition.PageIndex, condition.PageSize);
        }

        public async Task<ServiceResult> GetMasterDataById(int id)
        {
            try
            {
                var entity = await _repo.MstData.GetAsync(id);

                if (entity is null) return new ServiceResultError("Dữ liệu không tồn tại");

                if (entity.ValidFlg != (int)EnumCommon.Status.Valid) return new ServiceResultError("Dữ liệu không hợp lệ");

                var data = _mapper.Map<MstDataDto>(entity);
                return new ServiceResultSuccess("Lấy dữ liệu thành công!", data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return new ServiceResultError("Error while get data: " + ex.Message);
            }
        }
        #endregion
        #region CRUD
        public async Task<ServiceResult> InsertListData(List<MstDataDto> lstData)
        {
            //To do: Valid data
            try
            {
                //Update 2024-02-01: Check trùng tất cả các trường (Tạm thời bên khách hàng cũng không biết trùng cái gì)
                var checkDuplicateData = lstData.Select(x => new { x.InternalDrawingCode, x.ProductCode });
                //var exist = await _repo.MstData.AnyAsync(x => lstData.Any(y => y.InternalDrawingCode.Equals(x.InternalDrawingCode) && y.ProductCode.Equals(x.ProductCode)) && x.ValidFlg == (int)EnumCommon.Status.Valid);

                //if (exist) return new ServiceResultError("Đã tồn tại mã bản vẽ và mã sản phẩm!");

                var entities = _mapper.Map<List<MstData>>(lstData);
                await _repo.MstData.InsertAsync(entities);
                await _repo.SaveAync();
                return new ServiceResultSuccess("Thêm dữ liệu thành công!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return new ServiceResultError("Lỗi khi thêm dữ liệu: " + ex.Message);
            }
        }
        public async Task<ServiceResult> UpdateListData(List<MstDataDto> lstData)
        {
            //To do: Valid data
            try
            {
                var entities = await _repo.MstData.GetAllListAsync(x => lstData.Select(y => y.ProductID).Contains(x.ProductID));
                //No record
                if (entities is null || entities.Count == 0)
                    return new ServiceResultError("Dữ liệu không tồn tại");
                //Data update more than data exist
                if (entities.Count() != lstData.Count)
                    return new ServiceResultError("Dữ liệu cập nhật nhiều hơn dữ liệu đang tồn tại!");

                foreach (var entity in entities)
                {
                    foreach (var item in lstData)
                    {
                        if (item.ProductID == entity.ProductID)
                        {
                            _mapper.Map(item, entity);
                        }
                    }
                }
                await _repo.MstData.UpdateAsync(entities);
                await _repo.SaveAync();
                return new ServiceResultSuccess("Cập nhật dữ liệu thành công!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return new ServiceResultError("Lỗi khi thêm data: " + ex.Message);
            }
        }
        public async Task<ServiceResult> DeleteMasterData(int id)
        {
            try
            {
                var entity = await _repo.MstData.GetAsync(id);

                if (entity is null) return new ServiceResultError("Dữ liệu không tồn tại");
                if (entity.ValidFlg != (int)EnumCommon.Status.Valid) return new ServiceResultError("Dữ liệu không hợp lệ");

                entity.ValidFlg = (int)EnumCommon.Status.Invalid;
                await _repo.MstData.UpdateAsync(entity);
                await _repo.SaveAync();
                return new ServiceResultSuccess("Xóa thành công!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return new ServiceResultError("Lỗi khi xóa data: " + ex.Message);
            }
        }
        #endregion
    }
}
