using Core.Application.CustomModels;
using Core.Application.CustomModels.Dtos;
using Core.Application.CustomModels.SearchConditions;
using Core.Application.Enum;
using Core.Application.Interface;
using Core.Domain.Entity;
using Core.Domain.Interface;
using Core.Utils;
using Core.Utils.LogUtils;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Core.Application.Services
{
    public class EcuDataServiceImpl : IEcuDataService
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
        public EcuDataServiceImpl(IBaseRepositoryWrapper repo
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
        public async Task<GenericResponseResult<EcuDataDto>> SearchEcuData(EcuDataSearchImpl condition, bool blnExport = false)
        {
            DateTime? fromDate = DateUtils.GetDate(condition.FromDate);
            DateTime? toDate = DateUtils.GetDate(condition.ToDate);

            var iqResult = from e in _repo.EcuData.GetAll().AsNoTracking()
                           where (string.IsNullOrEmpty(condition.HUCode) || e.HUCode.ToLower().Contains(condition.HUCode.ToLower()))
                           && (string.IsNullOrEmpty(condition.LaserPrinting) || e.LaserPrinting.ToLower().Contains(condition.LaserPrinting.ToLower()))
                           && (string.IsNullOrEmpty(condition.NgCode) || e.NgCode.ToLower().Contains(condition.NgCode.ToLower()))
                           && (condition.Result == -1 || condition.Result == e.Result)
                           //Check date
                           && (!fromDate.HasValue || e.DateManufacture.Value.Date >= fromDate.Value.Date)
                           && (!toDate.HasValue || e.DateManufacture.Value.Date <= toDate.Value.Date)
                           //Check valid
                           && e.ValidFlg == (int)EnumCommon.Status.Valid
                           orderby e.EcuDataID descending, (e.UpdateTime ?? e.CreateTime) descending
                           select _mapper.Map<EcuDataDto>(e);
            var iqResult2 = from e in _repo.EcuExported.GetAll().AsNoTracking()
                            where (string.IsNullOrEmpty(condition.HUCode) || e.HUCode.ToLower().Contains(condition.HUCode.ToLower()))
                            && (string.IsNullOrEmpty(condition.LaserPrinting) || e.LaserPrinting.ToLower().Contains(condition.LaserPrinting.ToLower()))
                            && (string.IsNullOrEmpty(condition.NgCode) || e.NgCode.ToLower().Contains(condition.NgCode.ToLower()))
                            && (condition.Result == -1 || condition.Result == e.Result)
                            //Check date
                            && (!fromDate.HasValue || e.DateManufacture.Value.Date >= fromDate.Value.Date)
                            && (!toDate.HasValue || e.DateManufacture.Value.Date <= toDate.Value.Date)
                            //Check valid
                            && e.ValidFlg == (int)EnumCommon.Status.Valid
                            orderby e.EcuDataID descending, (e.UpdateTime ?? e.CreateTime) descending
                            select _mapper.Map<EcuDataDto>(e);
            //count total record
            int total1 = await iqResult.CountAsync();
            int total2 = await iqResult2.CountAsync();
            int total = total1 + total2;

            if (total <= 0)
            {
                return new GenericResponseResult<EcuDataDto>();
            }
            //count total page
            int totalPage1 = (int)Math.Ceiling(total1 / (double)condition.PageSize);
            int totalPage = (int)Math.Ceiling(total / (double)condition.PageSize);

            //Index start from 0
            if (totalPage - 1 < condition.PageIndex)
            {
                condition.PageIndex = totalPage - 1;
            }

            List<EcuDataDto> lstData = new List<EcuDataDto>();
            List<EcuDataDto> lstData2 = new List<EcuDataDto>();

            //export get all data
            if (blnExport)
            {
                lstData = await iqResult.ToListAsync();
            }
            else
            {
                int startIndex = condition.PageIndex * condition.PageSize;
                lstData = await iqResult.Skip(startIndex).Take(condition.PageSize).ToListAsync();
                if (lstData.Count < condition.PageSize)
                {
                    int index = 0;
                    int pageSize = condition.PageSize;
                    if (condition.PageIndex == totalPage1 - 1)
                    {
                        pageSize =  condition.PageSize - lstData.Count;
                    }
                    else if (condition.PageIndex >= totalPage1)
                    {
                        int page = condition.PageIndex - totalPage1;
                        index = page * condition.PageSize;
                    }
                    lstData2 = await iqResult2.Skip(index).Take(pageSize).ToListAsync();
                    foreach (var item in lstData2)
                    {
                        item.Exported = (int)EnumCommon.Status.Valid;
                    }
                    lstData.AddRange(lstData2);
                }
            }

            return new GenericResponseResult<EcuDataDto>(lstData, total, condition.PageIndex, condition.PageSize);
        }
        #endregion
        #region CRUD

        public async Task<ServiceResult> ImportListEcuData(List<EcuDataDto> data)
        {
            try
            {
                // Vui lòng kiểm tra lại dữ liệu
                if (data is null || data.Count == 0)
                {
                    _logger.LogError($"ImportEcuData: không có dữ liệu gửi lên.");
                    return new ServiceResultError("Không có dữ liệu gửi lên!");
                }

                // Define your startDate. In our case it would be 05/12/2023 12:00:00 AM
                /*DateTime startDate;
                if (data[0].DateManufacture != null)
                {
                    startDate = ((DateTime)data[0]!.DateManufacture!).Date;
                }
                else
                {
                    startDate = DateTime.Today.Date;
                }
                // Define your endDate. In our case it would be 05/12/2023 11:59:59 PM
                DateTime endDate = startDate.AddDays(1).AddMilliseconds(-1);*/

                // Tìm tất cả dữ liệu ECU trong ngày để kiểm tra trùng lặp
                var iqResult = from e in _repo.EcuData.GetAll().AsNoTracking()
                               where e.ValidFlg == (int)EnumCommon.Status.Valid
                               //&& e.DateManufacture >= startDate && e.DateManufacture <= endDate
                               select e;

                List<EcuData> lstData = await iqResult.ToListAsync();

                var notExist = data.Where(x =>
                {
                    //int exist = lstData.Count(y => y.HUCode == x.HUCode && y.LaserPrinting == x.LaserPrinting);
                    int exist = lstData.Count(y =>
                    {
                        if (y.HUCode != x.HUCode) return false;
                        if (y.LaserPrinting != x.LaserPrinting) return false;
                        if (y.Result != x.Result) return false;
                        //int result = DateTime.Compare((DateTime)y.DateManufacture!, (DateTime)x.DateManufacture!);
                        //if (result != 0) return false;

                        //_logger.LogInfo($"{y.DateManufacture} {x.DateManufacture} : {result}");
                        //_logger.LogInfo($"{y.DateManufacture?.ToString("yyyy-MM-dd HH:mm:ss \"GMT\"zzz")} < {x.DateManufacture?.ToString("yyyy-MM-dd HH:mm:ss \"GMT\"zzz")}");
                        // Đã tìm thấy dữ liệu trùng lặp
                        return true;
                    });
                    if (exist > 0)
                    {   // Nếu có dữ liệu trùng lặp thì bỏ
                        return false;
                    }
                    return true;
                }
                ).ToList();

                // Tìm tất cả dữ liệu ECU trong ngày để kiểm tra trùng lặp
                var iqResult2 = from e in _repo.EcuExported.GetAll().AsNoTracking()
                               where e.ValidFlg == (int)EnumCommon.Status.Valid
                               //&& e.DateManufacture >= startDate && e.DateManufacture <= endDate
                               select e;
                List<EcuExported> lstData2 = await iqResult2.ToListAsync();

                var notExist2 = data.Where(x =>
                {
                    //int exist = lstData.Count(y => y.HUCode == x.HUCode && y.LaserPrinting == x.LaserPrinting);
                    int exist = lstData2.Count(y =>
                    {
                        if (y.HUCode != x.HUCode) return false;
                        if (y.LaserPrinting != x.LaserPrinting) return false;
                        //int result = DateTime.Compare((DateTime)y.DateManufacture!, (DateTime)x.DateManufacture!);
                        //if (result != 0) return false;

                        //_logger.LogInfo($"{y.DateManufacture} {x.DateManufacture} : {result}");
                        //_logger.LogInfo($"{y.DateManufacture?.ToString("yyyy-MM-dd HH:mm:ss \"GMT\"zzz")} < {x.DateManufacture?.ToString("yyyy-MM-dd HH:mm:ss \"GMT\"zzz")}");
                        // Đã tìm thấy dữ liệu trùng lặp
                        return true;
                    });
                    if (exist > 0)
                    {   // Nếu có dữ liệu trùng lặp thì bỏ
                        return false;
                    }
                    return true;
                }
                ).ToList();

                if (notExist.Count == 0 || notExist2.Count ==0)
                {   // Nếu có dữ liệu trùng lặp thì bỏ qua import
                    var mes = $"ImportEcuData: tất cả {data.Count} dữ liệu đều đã tồn tại {notExist.Count} {notExist2.Count}!";
                    //var mes = $"ImportEcuData: tất cả {data.Count} dữ liệu ngày {startDate.Date.ToString("dd MMM yyyy")} đều đã tồn tại {notExist.Count} {notExist2.Count}!";
                    _logger.LogWarning(mes);
                    return new ServiceResultError(mes);
                }

                //var entities = _mapper.Map<List<EcuData>>(data);
                var entities = _mapper.Map<List<EcuData>>(notExist);
                await _repo.EcuData.BulkInsertAsync(entities);
                await _repo.SaveAync();
                return new ServiceResultSuccess($"Thêm dữ liệu thành công {notExist.Count} {notExist2.Count}!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return new ServiceResultError("Lỗi khi thêm list dữ liệu Ecu: " + ex.Message);
            }
        }

        public async Task<ServiceResult> UpdateEcuData(EcuDataDto dto)
        {
            try
            {
                var entity = await _repo.EcuData.GetAsync(dto.EcuDataID);
                if (entity is null)
                {
                    var entityExported = await _repo.EcuExported.GetAsync(dto.EcuDataID);
                    if (entityExported is null)
                        return new ServiceResultError("Dữ liệu Ecu không tồn tại!");

                    if (entityExported.ValidFlg != (int)EnumCommon.Status.Valid) return new ServiceResultError("Dữ liệu Ecu không hợp lệ!");

                    entityExported.Note = dto.Note;
                    entityExported.Result = dto.Result;
                    entityExported.NgCode = dto.NgCode;
                    await _repo.EcuExported.UpdateAsync(entityExported);
                    await _repo.SaveAync();
                    return new ServiceResultSuccess("Cập nhật dữ liệu Ecu đã xuất kho thành công!");
                }

                if (entity.ValidFlg != (int)EnumCommon.Status.Valid) return new ServiceResultError("Dữ liệu Ecu không hợp lệ!");

                entity.Note = dto.Note;
                entity.Result = dto.Result;
                entity.NgCode = dto.NgCode;
                await _repo.EcuData.UpdateAsync(entity);
                await _repo.SaveAync();
                return new ServiceResultSuccess("Cập nhật dữ liệu Ecu thành công!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return new ServiceResultError("Lỗi khi cập nhật dữ liệu Ecu: " + ex.Message);
            }
        }

        public async Task<ServiceResult> DeleteEcuData(int id)
        {
            try
            {
                var entity = await _repo.EcuData.GetAsync(id);
                if (entity is null)
                {
                    var entityExported = await _repo.EcuExported.GetAsync(id);
                    if (entityExported is null)
                        return new ServiceResultError("Dữ liệu Ecu không tồn tại!");

                    await _repo.EcuExported.DeleteAsync(entityExported);
                    await _repo.SaveAync();
                    return new ServiceResultSuccess("Xóa dữ liệu Ecu đã xuất kho thành công!");
                }

                //if (entity.ValidFlg != (int)EnumCommon.Status.Valid) return new ServiceResultError("Dữ liệu Ecu không hợp lệ!");

                await _repo.EcuData.DeleteAsync(entity);
                await _repo.SaveAync();
                return new ServiceResultSuccess("Xóa dữ liệu Ecu thành công!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return new ServiceResultError("Lỗi khi xóa dữ liệu Ecu: " + ex.Message);
            }
        }
        #endregion
    }
}
