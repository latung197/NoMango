using Astemo.Application.Constants;
using Astemo.Application.CustomModels;
using Astemo.Application.CustomModels.Dtos;
using Astemo.Application.CustomModels.Others;
using Astemo.Application.CustomModels.SearchConditions;
using Astemo.Application.Enum;
using Astemo.Application.Interface;
using Astemo.Domain.Entity;
using Astemo.Domain.Interface;
using Astemo.Infrastructure.ContextAccessors;
using Astemo.Utils;
using Astemo.Utils.LogUtils;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Astemo.Application.Services
{
    public class ExportPlanServiceImpl : IExportPlanService
    {
        //Repo
        private readonly IBaseRepositoryWrapper _repo;
        //Get config from appsettings.json if need
        private readonly IConfiguration _configuration;
        //Mapping model to entity
        private readonly IMapper _mapper;
        //Log
        private readonly ILoggerManager _logger;
        private readonly IUserPrincipalService _userPrincipalService;
        public ExportPlanServiceImpl(IBaseRepositoryWrapper repo
            , IConfiguration configuration
            , IMapper mapper
            , ILoggerManager logger
            , IUserPrincipalService userPrincipalService)
        {
            _repo = repo;
            _configuration = configuration;
            _mapper = mapper;
            _logger = logger;
            _userPrincipalService = userPrincipalService;
        }

        #region Search

        /// <summary>
        /// Search export plan
        /// Update 17/01/2024:[Số lượng trong hộp đựng ] nếu có dữ liệu trong file import thì sẽ lấy theo file import , nếu dữ liệu không có thì sẽ lấy từ bảng master trong cột [Quy cách đóng goi] dựa vào: Mã bản vẽ + Mã sản phẩm + khách hàng
        /// </summary>
        /// <param name="condition">Condition to search</param>
        /// <param name="blnExport">TRUE: Get all data. FALSE: Get pagging data</param>
        /// <returns></returns>
        public async Task<GenericResponseResult<ExportListPlanDto>> SearchExportPlan(ExportPlanSearchImpl condition, bool blnExport = false)
        {
            DateTime? fromDate = DateUtils.GetDate(condition.FromDate);
            DateTime? toDate = DateUtils.GetDate(condition.ToDate);

            var iqResult = from plan in _repo.ExportListPlan.GetAll().Where(x => x.ValidFlg == (int)EnumCommon.Status.Valid).AsNoTracking()
                           join m in _repo.MstData.GetAll().Where(x => x.ValidFlg == (int)EnumCommon.Status.Valid && x.ReferenceType != (int)EnumExportType.Type.ASSY).AsNoTracking() on
                           new { A = plan.CustomerCode, B = plan.ProductCode, C = plan.DrawingCode } equals
                           new { A = m.Customer, B = m.ProductCode, C = m.InternalDrawingCode } into leftJoin
                           from m in leftJoin.DefaultIfEmpty()
                           join mAssy in _repo.MstData.GetAll().Where(x => x.ValidFlg == (int)EnumCommon.Status.Valid && x.ReferenceType == (int)EnumExportType.Type.ASSY).AsNoTracking() on
                           new { A = plan.CustomerCode, B = plan.ProductCode, C = plan.AssemblyProductCode } equals
                           new { A = mAssy.Customer, B = mAssy.ProductCode, C = mAssy.AssemblyProductCode } into leftJoinAssy
                           from mAssy in leftJoinAssy.DefaultIfEmpty()
                           where
                           //Check OrderNumber
                           (string.IsNullOrEmpty(condition.OrderNumber) || plan.OrderNumber.ToLower().Contains(condition.OrderNumber.ToLower()))
                           && !string.IsNullOrEmpty(plan.OrderNumber)//Đã có đơn hàng mới xuất hiện
                           //Check date
                           && (!fromDate.HasValue || plan.DeliveryDate.Value.Date >= fromDate.Value.Date)
                           && (!toDate.HasValue || plan.DeliveryDate.Value.Date <= toDate.Value.Date)
                           //Check customer
                           && (string.IsNullOrEmpty(condition.CustomerCode) || plan.CustomerCode.ToLower().Contains(condition.CustomerCode.ToLower()))
                           //Check order state
                           && (condition.OrderState < 0 || condition.OrderState == plan.OrderState)
                           //Check ASSY
                           && (condition.ExportType < 0 || condition.ExportType == plan.ExportType)
                           orderby plan.ExportPlanID descending, plan.UpdateTime ?? plan.CreateTime descending
                           select new ExportListPlanDto
                           {
                               ExportPlanID = plan.ExportPlanID,
                               Staff = plan.Staff,
                               OrderNumber = plan.OrderNumber,
                               ParcelNo = plan.ParcelNo,
                               CustomerCode = plan.CustomerCode,
                               Customer = plan.Customer,
                               PlanCode = plan.PlanCode,
                               DeliveryDate = plan.DeliveryDate,
                               IndicatorQuantity = plan.IndicatorQuantity,
                               //Chỗ này đừng sửa bằng m !=null hoặc xóa đoạn m.PackageAmount !=null. Chết code đấy.
                               //To do: PackageAmount = 0
                               //Nếu loại xuất là ASSY thì lấy ở join với Mã lắp cụm
                               BoxNo = plan.BoxNo > 0 ? plan.BoxNo : ((plan.ExportType == (int)EnumExportType.Type.ASSY) ? (mAssy.PackageAmount != null ? (mAssy.PackageAmount != 0 ? ((plan.IndicatorQuantity % mAssy.PackageAmount != 0) ? plan.IndicatorQuantity / mAssy.PackageAmount + 1 : plan.IndicatorQuantity / mAssy.PackageAmount) : 0) : 0) : (m.PackageAmount != null ? (m.PackageAmount != 0 ? ((plan.IndicatorQuantity % m.PackageAmount != 0) ? plan.IndicatorQuantity / m.PackageAmount + 1 : plan.IndicatorQuantity / m.PackageAmount) : 0) : 0)),
                               //BoxNo = plan.BoxNo > 0 ? plan.BoxNo : (m.PackageAmount != null ? (m.PackageAmount != 0 ? ((int)Math.Ceiling((decimal)plan.IndicatorQuantity / m.PackageAmount)) : 0) : 0),
                               ProductID = plan.ProductID,
                               DrawingCode = plan.DrawingCode,
                               AssemblyProductCode = plan.AssemblyProductCode,
                               ProductCode = plan.ProductCode,
                               ProductName = plan.ProductName,
                               IdentityMark = m.IdentityMark,
                               ExportType = plan.ExportType,
                               Manufacturer = plan.Manufacturer,
                               OrderState = plan.OrderState,
                               ValidFlg = plan.ValidFlg,
                               BillState = plan.BillState
                           };
            //count total record
            int total = await iqResult.CountAsync();

            if (total <= 0)
            {
                return new GenericResponseResult<ExportListPlanDto>();
            }
            //count total page
            int totalPage = (int)Math.Ceiling(total / (double)condition.PageSize);
            //Index start from 0
            if (totalPage - 1 < condition.PageIndex)
            {
                condition.PageIndex = totalPage - 1;
            }

            List<ExportListPlanDto> lstData = new List<ExportListPlanDto>();

            try
            {
                //export get all data
                if (blnExport)
                    lstData = await iqResult.ToListAsync();
                else
                    lstData = await iqResult.Skip((condition.PageIndex) * condition.PageSize).Take(condition.PageSize).ToListAsync();

                return new GenericResponseResult<ExportListPlanDto>(lstData.GroupBy(x => x.ExportPlanID).Select(y => y.First()).ToList(), total, condition.PageIndex, condition.PageSize);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return new GenericResponseResult<ExportListPlanDto>(lstData.GroupBy(x => x.ExportPlanID).Select(y => y.First()).ToList(), total, condition.PageIndex, condition.PageSize, ex.Message);
            }
        }

        public async Task<GenericResponseResult<ExportListPlanDto>> SearchExportPlans(ExportPlanSearchImpl condition, bool blnExport = false)
        {
            DateTime? fromDate = DateUtils.GetDate(condition.FromDate);
            DateTime? toDate = DateUtils.GetDate(condition.ToDate);

            var iqResult = from plan in _repo.ExportListPlan.GetAll().Where(x => x.ValidFlg == (int)EnumCommon.Status.Valid).AsNoTracking()
                           join master in _repo.MstData.GetAll().Where(x => x.ValidFlg == (int)EnumCommon.Status.Valid).AsNoTracking() on
                           new { A = plan.ProductID } equals
                           new { A = master.ProductID } into leftJoin
                           from master in leftJoin.DefaultIfEmpty()
                           where
                           //Check OrderNumber
                           (string.IsNullOrEmpty(condition.OrderNumber) || plan.OrderNumber.ToLower().Contains(condition.OrderNumber.ToLower()))
                           && !string.IsNullOrEmpty(plan.OrderNumber)//Đã có đơn hàng mới xuất hiện
                           //Check date
                           && (!fromDate.HasValue || plan.DeliveryDate.Value.Date >= fromDate.Value.Date)
                           && (!toDate.HasValue || plan.DeliveryDate.Value.Date <= toDate.Value.Date)
                           //Check customer
                           && (string.IsNullOrEmpty(condition.CustomerCode) || plan.CustomerCode.ToLower().Contains(condition.CustomerCode.ToLower()))
                           //Check order state
                           && (condition.OrderState < 0 || condition.OrderState == plan.OrderState)
                           //Check ASSY
                           && (condition.ExportType < 0 || condition.ExportType == plan.ExportType)
                           orderby plan.ExportPlanID descending, plan.UpdateTime ?? plan.CreateTime descending
                           select new ExportListPlanDto
                           {
                               ExportPlanID = plan.ExportPlanID,
                               Staff = plan.Staff,
                               OrderNumber = plan.OrderNumber,
                               ParcelNo = plan.ParcelNo,
                               CustomerCode = plan.CustomerCode,
                               Customer = plan.Customer,
                               PlanCode = plan.PlanCode,
                               DeliveryDate = plan.DeliveryDate,
                               IndicatorQuantity = plan.IndicatorQuantity,
                               //Chỗ này đừng sửa bằng m !=null hoặc xóa đoạn m.PackageAmount !=null. Chết code đấy.
                               //To do: PackageAmount = 0
                               BoxNo = plan.BoxNo > 0 ? plan.BoxNo : (
                                    master.PackageAmount != null ? (
                                        master.PackageAmount != 0 ? (
                                            (plan.IndicatorQuantity % master.PackageAmount != 0) ? plan.IndicatorQuantity / master.PackageAmount + 1 : plan.IndicatorQuantity / master.PackageAmount
                                        ) : 0
                                    ) : 0
                               ),
                               //BoxNo = plan.BoxNo > 0 ? plan.BoxNo : (m.PackageAmount != null ? (m.PackageAmount != 0 ? ((int)Math.Ceiling((decimal)plan.IndicatorQuantity / m.PackageAmount)) : 0) : 0),
                               ProductID = plan.ProductID,
                               DrawingCode = plan.DrawingCode,
                               AssemblyProductCode = plan.AssemblyProductCode,
                               ProductCode = plan.ProductCode,
                               ProductName = plan.ProductName,
                               IdentityMark = master.IdentityMark,
                               ExportType = plan.ExportType,
                               Manufacturer = plan.Manufacturer,
                               OrderState = plan.OrderState,
                               ValidFlg = plan.ValidFlg,
                               BillState = plan.BillState
                           };
            //count total record
            int total = await iqResult.CountAsync();

            if (total <= 0)
            {
                return new GenericResponseResult<ExportListPlanDto>();
            }
            //count total page
            int totalPage = (int)Math.Ceiling(total / (double)condition.PageSize);
            //Index start from 0
            if (totalPage - 1 < condition.PageIndex)
            {
                condition.PageIndex = totalPage - 1;
            }

            List<ExportListPlanDto> lstData = new List<ExportListPlanDto>();

            try
            {
                //export get all data
                if (blnExport)
                    lstData = await iqResult.ToListAsync();
                else
                    lstData = await iqResult.Skip((condition.PageIndex) * condition.PageSize).Take(condition.PageSize).ToListAsync();

                return new GenericResponseResult<ExportListPlanDto>(lstData.GroupBy(x => x.ExportPlanID).Select(y => y.First()).ToList(), total, condition.PageIndex, condition.PageSize);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return new GenericResponseResult<ExportListPlanDto>(lstData.GroupBy(x => x.ExportPlanID).Select(y => y.First()).ToList(), total, condition.PageIndex, condition.PageSize, ex.Message);
            }
        }

        /// <summary>
        /// Chỉ search các đơn hàng chưa xuất PC052
        /// </summary>
        /// <param name="condition">Condition to search</param>
        /// <param name="blnExport">TRUE: Get all data. FALSE: Get pagging data</param>
        /// <remarks>29/01/2024: Bỏ điều kiện search theo trạng thái đi</remarks>
        /// <returns></returns>
        public async Task<GenericResponseResult<ExportListPlanDto>> SearchExportPlanPre(ExportPlanPreSearchImpl condition, bool blnExport = false)
        {
            DateTime? fromDate = DateUtils.GetDate(condition.FromDate);
            DateTime? toDate = DateUtils.GetDate(condition.ToDate);

            var iqResult = from plan in _repo.ExportListPlan.GetAll().Where(x => x.ValidFlg == (int)EnumCommon.Status.Valid).AsNoTracking()
                           join m in _repo.MstData.GetAll().Where(x => x.ValidFlg == (int)EnumCommon.Status.Valid && x.ReferenceType != (int)EnumExportType.Type.ASSY).AsNoTracking()
                           on new { A = plan.CustomerCode, B = plan.ProductCode, C = plan.DrawingCode } equals new { A = m.Customer, B = m.ProductCode, C = m.InternalDrawingCode } into leftJoin
                           from m in leftJoin.DefaultIfEmpty()
                           join mAssy in _repo.MstData.GetAll().Where(x => x.ValidFlg == (int)EnumCommon.Status.Valid && x.ReferenceType == (int)EnumExportType.Type.ASSY).AsNoTracking()
                           on new { A = plan.CustomerCode, B = plan.ProductCode, C = plan.AssemblyProductCode } equals new { A = mAssy.Customer, B = mAssy.ProductCode, C = mAssy.AssemblyProductCode } into leftJoinAssy
                           from mAssy in leftJoinAssy.DefaultIfEmpty()
                           where (string.IsNullOrEmpty(condition.OrderNumber) || plan.OrderNumber.ToLower().Contains(condition.OrderNumber.ToLower()))
                           && !string.IsNullOrEmpty(plan.OrderNumber)//Màn hình này là kế hoạch sản xuất nhưng đã có đơn hàng
                           //Check date
                           && (!fromDate.HasValue || plan.DeliveryDate.Value.Date >= fromDate.Value.Date)
                           && (!toDate.HasValue || plan.DeliveryDate.Value.Date <= toDate.Value.Date)
                           //Check customer
                           && (string.IsNullOrEmpty(condition.CustomerCode) || plan.CustomerCode.ToLower().Contains(condition.CustomerCode.ToLower()))
                           //Check order state
                           //Chỉ lấy đơn hàng chưa xuất
                           && plan.OrderState == (int)EnumOrderState.State.NotExport
                           //Check ASSY
                           && condition.ExportType == plan.ExportType
                           //Check billstate
                           && (condition.BillState < 0 || condition.BillState == plan.BillState)
                           orderby plan.ExportPlanID descending, plan.UpdateTime ?? plan.CreateTime descending
                           select new ExportListPlanDto
                           {
                               ExportPlanID = plan.ExportPlanID,
                               Staff = plan.Staff,
                               OrderNumber = plan.OrderNumber,
                               ParcelNo = plan.ParcelNo,
                               CustomerCode = plan.CustomerCode,
                               Customer = plan.Customer,
                               PlanCode = plan.PlanCode,
                               DeliveryDate = plan.DeliveryDate,
                               IndicatorQuantity = plan.IndicatorQuantity,
                               //Chỗ này đừng sửa bằng m !=null hoặc xóa đoạn m.PackageAmount !=null đi. Chết chương trình đấy.
                               //To do: PackageAmount = 0
                               BoxNo = plan.BoxNo > 0 ? plan.BoxNo : ((plan.ExportType == (int)EnumExportType.Type.ASSY) ? (mAssy.PackageAmount != null ? (mAssy.PackageAmount != 0 ? ((plan.IndicatorQuantity % mAssy.PackageAmount != 0) ? plan.IndicatorQuantity / mAssy.PackageAmount + 1 : plan.IndicatorQuantity / mAssy.PackageAmount) : 0) : 0) : (m.PackageAmount != null ? (m.PackageAmount != 0 ? ((plan.IndicatorQuantity % m.PackageAmount != 0) ? plan.IndicatorQuantity / m.PackageAmount + 1 : plan.IndicatorQuantity / m.PackageAmount) : 0) : 0)),
                               DrawingCode = plan.DrawingCode,
                               AssemblyProductCode = plan.AssemblyProductCode,
                               ProductCode = plan.ProductCode,
                               ProductName = plan.ProductName,
                               ExportType = plan.ExportType,
                               Manufacturer = plan.Manufacturer,
                               OrderState = plan.OrderState,
                               ReferenceType = (plan.ExportType == (int)EnumExportType.Type.ASSY) ? (mAssy.ReferenceType != null ? mAssy.ReferenceType : -1) : (m.ReferenceType != null ? m.ReferenceType : -1),
                               ValidFlg = plan.ValidFlg,
                               BillState = plan.BillState
                           };
            //count total record
            int total = await iqResult.CountAsync();

            if (total <= 0)
            {
                return new GenericResponseResult<ExportListPlanDto>();
            }
            //count total page
            int totalPage = (int)Math.Ceiling(total / (double)condition.PageSize);
            //Index start from 0
            if (totalPage - 1 < condition.PageIndex)
            {
                condition.PageIndex = totalPage - 1;
            }

            List<ExportListPlanDto> lstData = new List<ExportListPlanDto>();

            //export get all data
            if (blnExport)
                lstData = await iqResult.ToListAsync();
            else
                lstData = await iqResult.Skip((condition.PageIndex) * condition.PageSize).Take(condition.PageSize).ToListAsync();

            return new GenericResponseResult<ExportListPlanDto>(lstData, total, condition.PageIndex, condition.PageSize);
        }

        public async Task<GenericResponseResult<ExportHistoryPlanListDto>> SearchExportHistoryPlan(ExportPlanSearchImpl condition, bool blnExport = true)
        {
            DateTime? fromDate = DateUtils.GetDate(condition.FromDate);
            DateTime? toDate = DateUtils.GetDate(condition.ToDate);

            var iqResult = from plan in _repo.ExportListPlan.GetAll().Where(x => x.ValidFlg == (int)EnumCommon.Status.Valid).AsNoTracking()
                           join master in _repo.MstData.GetAll().Where(x => x.ValidFlg == (int)EnumCommon.Status.Valid).AsNoTracking() on
                           new { A = plan.ProductID } equals
                           new { A = master.ProductID } into leftJoin
                           from master in leftJoin.DefaultIfEmpty()
                           join history in _repo.ExportHistoryList.GetAll().Where(x => x.ValidFlg == (int)EnumCommon.Status.Valid).AsNoTracking() on
                           //on history.ExportPlanID equals plan.ExportPlanID
                           new { A = plan.ExportPlanID } equals
                           new { A = history.ExportPlanID } into left2Join
                           from history in left2Join.DefaultIfEmpty()
                           where
                           //Check OrderNumber
                           (string.IsNullOrEmpty(condition.OrderNumber) || plan.OrderNumber.ToLower().Contains(condition.OrderNumber.ToLower()))
                           && !string.IsNullOrEmpty(plan.OrderNumber)//Đã có đơn hàng mới xuất hiện
                           //Check date
                           && (!fromDate.HasValue || plan.DeliveryDate.Value.Date >= fromDate.Value.Date)
                           && (!toDate.HasValue || plan.DeliveryDate.Value.Date <= toDate.Value.Date)
                           //Check customer
                           && (string.IsNullOrEmpty(condition.CustomerCode) || plan.CustomerCode.ToLower().Contains(condition.CustomerCode.ToLower()))
                           //Check order state
                           && (condition.OrderState < 0 || condition.OrderState == plan.OrderState)
                           //Check ASSY
                           && (condition.ExportType < 0 || condition.ExportType == plan.ExportType)
                           orderby plan.ExportPlanID descending, history.ExportHistoryID descending, history.UpdateTime ?? history.CreateTime descending
                           select new ExportHistoryPlanListDto
                           {
                               ExportPlanID = plan.ExportPlanID,
                               ExportHistoryID = history != null ? history.ExportHistoryID : 0,
                               TimeReadQrCode = history.TimeReadQrCode,
                               Implementer = history.Implementer,
                               OrderNumber = plan.OrderNumber,
                               ParcelNo = plan.ParcelNo,
                               ReferenceType = master != null ? master.ReferenceType : 0,
                               ExportType = plan.ExportType,
                               CustomerCode = plan.CustomerCode,
                               Customer = plan.Customer,
                               PlanCode = plan.PlanCode,
                               DeliveryDate = plan.DeliveryDate,
                               IndicatorQuantity = plan.IndicatorQuantity,
                               BoxNo = plan.BoxNo,
                               DrawingCode = plan.DrawingCode,
                               AssemblyProductCode = plan.AssemblyProductCode,
                               ProductCode = plan.ProductCode,
                               ProductName = plan.ProductName,
                               Manufacturer = plan.Manufacturer,
                               IdentityMark = master != null ? master.IdentityMark : "",
                               TimeReadHU = history.TimeReadHU,
                               HUSerial = history.HUSerial,
                               LaserEngraving = history.LaserEngraving,
                               Compare = history.Compare,
                               Quantity = history != null ? history.Quantity : 0,
                               BoxSerial = history.BoxSerial,
                               ShiftWork = history != null ? history.ShiftWork : 0,
                               TimeRelease = history.TimeRelease,
                               WarehouseReleasePerson = history.WarehouseReleasePerson,
                               Exported = history != null ? history.Exported : 0,
                               OrderState = history != null ? history.OrderState : 0,
                               ValidFlg = plan.ValidFlg
                           };
            //count total record
            int total = await iqResult.CountAsync();

            if (total <= 0)
            {
                return new GenericResponseResult<ExportHistoryPlanListDto>();
            }
            //count total page
            int totalPage = (int)Math.Ceiling(total / (double)condition.PageSize);
            //Index start from 0
            if (totalPage - 1 < condition.PageIndex)
            {
                condition.PageIndex = totalPage - 1;
            }

            List<ExportHistoryPlanListDto> lstHistoryData = new List<ExportHistoryPlanListDto>();

            try
            {
                lstHistoryData = await iqResult.ToListAsync();

                return new GenericResponseResult<ExportHistoryPlanListDto>(lstHistoryData, total, condition.PageIndex, condition.PageSize);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return new GenericResponseResult<ExportHistoryPlanListDto>(lstHistoryData, total, condition.PageIndex, condition.PageSize, ex.Message);
            }
        }

        public async Task<GenericResponseResult<ExportHistoryListDto>> SearchExportHistoryPlanById(int planID)
        {
            var iqResult = from history in _repo.ExportHistoryList.GetAll().AsNoTracking()
                           join plan in _repo.ExportListPlan.GetAll().AsNoTracking() on history.ExportPlanID equals plan.ExportPlanID
                           where history.ExportPlanID == planID
                           //Check valid
                           && plan.ValidFlg == (int)EnumCommon.Status.Valid
                           && history.ValidFlg == (int)EnumCommon.Status.Valid
                           orderby history.ExportHistoryID descending, history.UpdateTime ?? history.CreateTime descending
                           select _mapper.Map<ExportHistoryListDto>(history);

            var lstData = await iqResult.ToListAsync();

#if DEBUG
            var planInfo = await _repo.ExportListPlan.GetAsync(planID);
            if (planInfo.OrderState == (int)EnumOrderState.State.Exported)
            {
                var existEcu = await _repo.EcuData.GetAllListAsync(x => x.ValidFlg == (int)EnumCommon.Status.Valid && lstData.Select(y => y.HUSerial).Contains(x.HUCode));
                if (existEcu.Any())
                {
                    var entities = existEcu.Select(x => new EcuExported
                    {
                        EcuDataID = 0,
                        DateManufacture = x.DateManufacture,
                        NgCode = x.NgCode,
                        Note = x.Note,
                        Result = x.Result,
                        HUCode = x.HUCode,
                        LaserPrinting = x.LaserPrinting,
                        PackState = x.PackState,
                        ValidFlg = x.ValidFlg,
                        CreateTime = x.CreateTime,
                        CreateId = x.CreateId,
                        UpdateTime = x.UpdateTime,
                        UpdateId = x.UpdateId
                    });
                    await _repo.EcuExported.InsertAsync(entities);
                    await _repo.SaveAync();

                    await _repo.EcuData.DeleteAsync(existEcu);
                    await _repo.SaveAync();
                }
            }
#endif

            return new GenericResponseResult<ExportHistoryListDto>(lstData);
        }

        public async Task<GenericResponseResult<ExportHistoryListDto>> SearchExportHistoryPlanByIdAndSerial(int PlanID, string boxSerial)
        {
            var iqResult = from history in _repo.ExportHistoryList.GetAll().AsNoTracking()
                           join plan in _repo.ExportListPlan.GetAll().AsNoTracking() on history.ExportPlanID equals plan.ExportPlanID
                           where history.ExportPlanID == PlanID
                           && history.BoxSerial == boxSerial
                           //Check valid
                           && plan.ValidFlg == (int)EnumCommon.Status.Valid
                           && history.ValidFlg == (int)EnumCommon.Status.Valid
                           orderby history.ExportHistoryID descending, history.UpdateTime ?? history.CreateTime descending
                           select _mapper.Map<ExportHistoryListDto>(history);

            var lstData = await iqResult.ToListAsync();

            return new GenericResponseResult<ExportHistoryListDto>(lstData);
        }

        public async Task<GenericResponseResult<ExportHistoryListDto>> SearchExportHistoryPlanBySerial(string boxSerial)
        {
            var iqResult = from history in _repo.ExportHistoryList.GetAll().AsNoTracking()
                           join box in _repo.BoxInfo.GetAll().AsNoTracking()
                           on history.BoxSerial equals box.BoxSerial
                           where history.BoxSerial == boxSerial
                           //Check valid
                           && box.ValidFlg == (int)EnumCommon.Status.Valid
                           && history.ValidFlg == (int)EnumCommon.Status.Valid
                           orderby history.ExportHistoryID descending, history.UpdateTime ?? history.CreateTime descending
                           select _mapper.Map<ExportHistoryListDto>(history);

            var lstData = await iqResult.ToListAsync();

            return new GenericResponseResult<ExportHistoryListDto>(lstData);
        }

        /// <summary>
        /// Màn hình chuẩn bị hàng - PC008
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="blnExport"></param>
        /// <returns></returns>
        /*public async Task<GenericResponseResult<PrepareProduct>> SearchPrepareProduct(PrepareProductSearchImpl condition, bool blnExport = false)
        {
            DateTime? fromDate = DateUtils.GetDate(condition.FromDate);
            DateTime? toDate = DateUtils.GetDate(condition.ToDate);
            var iqResult = from p in _repo.ExportListPlan.GetAll().AsNoTracking()
                           join h in _repo.ExportHistoryList.GetAll().AsNoTracking() on p.ExportPlanID equals h.ExportPlanID
                           where (string.IsNullOrEmpty(p.OrderNumber))//Màn hình này là kế hoạch sản xuất nhưng chưa có đơn hàng
                           && (string.IsNullOrEmpty(condition.BoxSerial) || h.BoxSerial.ToLower().Contains(condition.BoxSerial.ToLower()))//Màn hình này là kế hoạch sản xuất nhưng chưa có đơn hàng
                           //Check date TimeReadHU => TimeReadQrCode (Bug #8189)
                           && (!fromDate.HasValue || h.TimeReadQrCode.Value.Date >= fromDate.Value.Date)
                           && (!toDate.HasValue || h.TimeReadQrCode.Value.Date <= toDate.Value.Date)
                           && (condition.ExportType < 0 || condition.ExportType == p.ExportType)
                           && (p.OrderState == (int)EnumOrderState.State.Processing || p.OrderState == (int)EnumOrderState.State.Enough)
                           && (condition.OrderState < 0 || p.OrderState == condition.OrderState)
                           && p.ValidFlg == (int)EnumCommon.Status.Valid
                           && h.ValidFlg == (int)EnumCommon.Status.Valid
                           //orderby p.ExportPlanID descending, (p.UpdateTime ?? p.CreateTime) descending
                           group new { p.ExportPlanID, h.BoxSerial, p.ProductCode, p.ProductName, h.Quantity, p.ExportType, h.OrderState, h.CreateTime, h.Compare } by new { p.ExportPlanID, h.BoxSerial, p.ProductCode, p.ProductName, h.Quantity, p.ExportType, h.OrderState, h.CreateTime } into gJoin
                           orderby gJoin.Key.CreateTime descending
                           select new PrepareProduct
                           {
                               ExportPlanID = gJoin.Key.ExportPlanID,
                               BoxSerial = gJoin.Key.BoxSerial,
                               ProductName = gJoin.Key.ProductName,
                               ProductCode = gJoin.Key.ProductCode,
                               Quantity = gJoin.Count(h => h.Compare == CommonConstant.OK),
                               IndicatorQuantity = gJoin.Key.Quantity,
                               ExportType = gJoin.Key.ExportType,
                               OrderState = gJoin.Key.OrderState,
                           };
            //count total record
            int total = await iqResult.CountAsync();

            if (total <= 0)
            {
                return new GenericResponseResult<PrepareProduct>();
            }
            //count total page
            int totalPage = (int)Math.Ceiling(total / (double)condition.PageSize);
            //Index start from 0
            if (totalPage - 1 < condition.PageIndex)
            {
                condition.PageIndex = totalPage - 1;
            }

            List<PrepareProduct> lstData = new List<PrepareProduct>();

            //export get all data
            if (blnExport)
                lstData = await iqResult.ToListAsync();
            else
                lstData = await iqResult.Skip((condition.PageIndex) * condition.PageSize).Take(condition.PageSize).ToListAsync();

            return new GenericResponseResult<PrepareProduct>(lstData, total, condition.PageIndex, condition.PageSize);
        }*/

        public async Task<GenericResponseResult<PrepareProduct>> SearchPrepareProduct(PrepareProductSearchImpl condition, bool blnExport = false)
        {
            DateTime? fromDate = DateUtils.GetDate(condition.FromDate);
            DateTime? toDate = DateUtils.GetDate(condition.ToDate);
            var iqResult = from b in _repo.BoxInfo.GetAll().AsNoTracking()
                           where
                           (string.IsNullOrEmpty(condition.BoxSerial) || b.BoxSerial.ToLower().Contains(condition.BoxSerial.ToLower()))
                           //Check date TimeReadHU => TimeReadQrCode (Bug #8189)
                           && (!fromDate.HasValue || b.TimeReadQrCode.Value.Date >= fromDate.Value.Date)
                           && (!toDate.HasValue || b.TimeReadQrCode.Value.Date <= toDate.Value.Date)
                           && (condition.ExportType < 0 || condition.ExportType == b.ExportType)
                           && b.BoxState <= (int)EnumBoxState.State.Enough
                           && (b.OrderState <= (int)EnumOrderState.State.Enough)
                           //&& (p.OrderState == (int)EnumOrderState.State.Processing || p.OrderState == (int)EnumOrderState.State.Enough)
                           //&& (condition.OrderState < 0 || p.OrderState == condition.OrderState)
                           && b.ValidFlg == (int)EnumCommon.Status.Valid
                           orderby b.BoxID descending, (b.UpdateTime ?? b.CreateTime) descending
                           select new PrepareProduct
                           {
                               BoxID = b.BoxID,
                               ExportPlanID = b.ExportPlanID,
                               BoxSerial = b.BoxSerial,
                               AssemblyProductCode = b.AssemblyProductCode,
                               ProductName = b.ProductName,
                               ProductCode = b.ProductCode,
                               Quantity = b.Quantity,
                               IndicatorQuantity = b.IndicatorQuantity,
                               ExportType = b.ExportType,
                               BoxState = b.BoxState,
                               OrderState = b.OrderState,
                           };
            //count total record
            int total = await iqResult.CountAsync();

            if (total <= 0)
            {
                return new GenericResponseResult<PrepareProduct>();
            }
            //count total page
            int totalPage = (int)Math.Ceiling(total / (double)condition.PageSize);
            //Index start from 0
            if (totalPage - 1 < condition.PageIndex)
            {
                condition.PageIndex = totalPage - 1;
            }

            List<PrepareProduct> lstData = new List<PrepareProduct>();

            //export get all data
            if (blnExport)
                lstData = await iqResult.ToListAsync();
            else
                lstData = await iqResult.Skip((condition.PageIndex) * condition.PageSize).Take(condition.PageSize).ToListAsync();

            return new GenericResponseResult<PrepareProduct>(lstData, total, condition.PageIndex, condition.PageSize);
        }

        public async Task<ServiceResult> GetExportPlanById(int IDPlan)
        {
            try
            {
                var plan = await _repo.ExportListPlan.GetAsync(IDPlan);

                if (plan is null)
                    return new ServiceResultError("Kế hoạch không tồn tại!");

                if (plan.ValidFlg != (int)EnumCommon.Status.Valid)
                    return new ServiceResultError("Kế hoạch không hợp lệ!");
                var dto = _mapper.Map<ExportListPlanDto>(plan);

                return new ServiceResultSuccess("Lấy thông tin kế hoạch thành công", dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return new ServiceResultError("Lỗi khi lấy thông tin kế hoạch: " + ex.Message);
            }
        }

        /// <summary>
        /// Màn PC052
        /// </summary>
        /// <returns></returns>
        public async Task<ServiceResult> GetListExportPlanPre()
        {
            try
            {
                var iqResult = from plan in _repo.ExportListPlan.GetAll().Where(x => x.ValidFlg == (int)EnumCommon.Status.Valid).AsNoTracking()
                               join m in _repo.MstData.GetAll().Where(x => x.ValidFlg == (int)EnumCommon.Status.Valid && x.ReferenceType != (int)EnumExportType.Type.ASSY).AsNoTracking()
                           on new { A = plan.CustomerCode, B = plan.ProductCode, C = plan.DrawingCode } equals new { A = m.Customer, B = m.ProductCode, C = m.InternalDrawingCode } into leftJoin
                               from m in leftJoin.DefaultIfEmpty()
                               join mAssy in _repo.MstData.GetAll().Where(x => x.ValidFlg == (int)EnumCommon.Status.Valid && x.ReferenceType == (int)EnumExportType.Type.ASSY).AsNoTracking()
                                   on new { A = plan.CustomerCode, B = plan.ProductCode, C = plan.AssemblyProductCode } equals new { A = mAssy.Customer, B = mAssy.ProductCode, C = mAssy.AssemblyProductCode } into leftJoinAssy
                               from mAssy in leftJoinAssy.DefaultIfEmpty()
                               where !string.IsNullOrEmpty(plan.OrderNumber)//Lấy những thằng có mã orderNumber
                               //Lấy những thằng đã được lên đơn
                               && plan.BillState == (int)EnumCommon.Status.Valid
                               //Không có hộp nào
                               && plan.OrderState == (int)EnumOrderState.State.NotExport
                               // && !_repo.ExportHistoryList.GetAll().AsNoTracking().Any(x => x.ExportPlanID == p.ExportPlanID && x.ValidFlg == (int)EnumCommon.Status.Valid)
                               select new ExportPlanNonBox
                               {
                                   ExportPlanID = plan.ExportPlanID,
                                   OrderNumber = plan.OrderNumber,
                                   CustomerCode = plan.CustomerCode,
                                   DeliveryDate = plan.DeliveryDate,
                                   IndicatorQuantity = plan.IndicatorQuantity,
                                   //Chỗ này đừng sửa bằng m !=null hoặc xóa đoạn m.PackageAmount !=null đi. Chết chương trình đấy.
                                   //To do: PackageAmount = 0
                                   BoxNo = plan.BoxNo > 0 ? plan.BoxNo : ((plan.ExportType == (int)EnumExportType.Type.ASSY) ? (mAssy.PackageAmount != null ? (mAssy.PackageAmount != 0 ? ((plan.IndicatorQuantity % mAssy.PackageAmount != 0) ? plan.IndicatorQuantity / mAssy.PackageAmount + 1 : plan.IndicatorQuantity / mAssy.PackageAmount) : 0) : 0) : (m.PackageAmount != null ? (m.PackageAmount != 0 ? ((plan.IndicatorQuantity % m.PackageAmount != 0) ? plan.IndicatorQuantity / m.PackageAmount + 1 : plan.IndicatorQuantity / m.PackageAmount) : 0) : 0)),
                                   DrawingCode = plan.DrawingCode,
                                   AssemblyProductCode = plan.AssemblyProductCode,
                                   ProductCode = plan.ProductCode,
                                   ProductName = plan.ProductName,
                                   ExportType = plan.ExportType,
                                   //ReferenceType = (m.ReferenceType != null ? m.ReferenceType : 0),
                                   ReferenceType = (plan.ExportType == (int)EnumExportType.Type.ASSY ? mAssy.ReferenceType : m.ReferenceType),
                               };
                var entities = await iqResult.ToListAsync();
                return new ServiceResultSuccess("Lấy dữ liệu thành công", entities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return new ServiceResultError("Lỗi khi lấy thông tin kế hoạch: " + ex.Message);
            }
        }

        /// <summary>
        /// Lấy thông tin in phiếu temp
        /// Màn này lấy thông tin kế hoạch 
        /// => Tính ra được số thùng
        /// => Tự in ra mã BoxSerial
        /// </summary>
        /// <param name="id">id kế hoạch</param>
        /// <returns></returns>
        public async Task<GenericResponseResult<BoxStampInfo>> GetExportPlanStampInfo(int id)
        {
            var iqResult = from plan in _repo.ExportListPlan.GetAll().Where(x => x.ValidFlg == (int)EnumCommon.Status.Valid).AsNoTracking()
                           join m in _repo.MstData.GetAll().Where(x => x.ValidFlg == (int)EnumCommon.Status.Valid && x.ReferenceType != (int)EnumExportType.Type.ASSY).AsNoTracking()
                           on new { A = plan.CustomerCode, B = plan.ProductCode, C = plan.DrawingCode } equals new { A = m.Customer, B = m.ProductCode, C = m.InternalDrawingCode } into leftJoin
                           from m in leftJoin.DefaultIfEmpty()
                               //join mAssy in _repo.MstData.GetAll().Where(x => x.ValidFlg == (int)EnumCommon.Status.Valid && x.ReferenceType == (int)EnumExportType.Type.ASSY).AsNoTracking()
                           join mAssy in _repo.MstData.GetAll().Where(x => x.ValidFlg == (int)EnumCommon.Status.Valid).AsNoTracking()
                           on new { A = plan.CustomerCode, B = plan.ProductCode, C = plan.AssemblyProductCode } equals new { A = mAssy.Customer, B = mAssy.ProductCode, C = mAssy.AssemblyProductCode } into leftJoinAssy
                           from mAssy in leftJoinAssy.DefaultIfEmpty()
                           where plan.ExportPlanID == id
                           select new ExportPlanStampInfo
                           {
                               CustomerCode = plan.CustomerCode,
                               DeliveryDate = plan.DeliveryDate,
                               PackageAmount = ((plan.ExportType == (int)EnumExportType.Type.ASSY) ? (mAssy.PackageAmount != null ? mAssy.PackageAmount : 0) : (m.PackageAmount != null ? m.PackageAmount : 0)),
                               IndicatorQuantity = plan.IndicatorQuantity,
                               OrderNumber = plan.OrderNumber,
                               DrawingCode = plan.DrawingCode,
                               AssemblyProductCode = plan.AssemblyProductCode,
                               AssemblyProductCodeMaster = mAssy.AssemblyProductCode ?? m.AssemblyProductCode,
                               ProductCode = plan.ProductCode,
                               ProductName = plan.ProductName,
                               ExportType = plan.ExportType,
                               ReferenceType = ((plan.ExportType == (int)EnumExportType.Type.ASSY) ? (mAssy.ReferenceType != null ? mAssy.ReferenceType : 0) : (m.ReferenceType != null ? m.ReferenceType : 0)),
                               //Nếu số lượng chỉ thị không chia hết cho quy cách đóng gói cơ bản thì số thùng +1 (thùng lẻ cuối cùng)
                               BoxNo = plan.BoxNo > 0 ? plan.BoxNo : ((plan.ExportType == (int)EnumExportType.Type.ASSY) ? (mAssy.PackageAmount != null ? (mAssy.PackageAmount != 0 ? ((plan.IndicatorQuantity % mAssy.PackageAmount != 0) ? plan.IndicatorQuantity / mAssy.PackageAmount + 1 : plan.IndicatorQuantity / mAssy.PackageAmount) : 0) : 0) : (m.PackageAmount != null ? (m.PackageAmount != 0 ? ((plan.IndicatorQuantity % m.PackageAmount != 0) ? plan.IndicatorQuantity / m.PackageAmount + 1 : plan.IndicatorQuantity / m.PackageAmount) : 0) : 0)),
                               StampCode = mAssy.StampCode ?? m.StampCode,
                               StampReleaseDt = mAssy.StampReleaseDt ?? m.StampReleaseDt
                           };

            var planInfo = await iqResult.FirstOrDefaultAsync();

            if (planInfo is null) return new GenericResponseResult<BoxStampInfo>(CommonConstant.PLAN_NOT_MATCH_MASTER);

            List<BoxStampInfo> listData = new List<BoxStampInfo>();
            //To do: Serial thùng quy tắc cần sửa
            for (int i = 1; i <= planInfo.BoxNo; i++)
            {
                //Thùng cuối cùng là số lẻ còn lại của các thùng trc
                int quantity = (i != planInfo.BoxNo ? planInfo.PackageAmount : planInfo.IndicatorQuantity - (planInfo.BoxNo - 1) * planInfo.PackageAmount);
                //quantity = (i != planInfo.BoxNo ? planInfo.PackageAmount : planInfo.IndicatorQuantity - (i - 1) * planInfo.PackageAmount);
                /*if (i < planInfo.BoxNo) {
                    quantity = planInfo.PackageAmount;
                } else {
                    quantity = planInfo.IndicatorQuantity - (planInfo.BoxNo - 1) * planInfo.PackageAmount;
                }*/
                listData.Add(new BoxStampInfo
                {
                    //BoxNo = i,    // !? Số lượng thùng hàng
                    BoxNo = planInfo.BoxNo,
                    BoxSerial = DateTime.Now.ToString("yyyyMMddHHmmss") + i.ToString(),
                    DeliveryDate = planInfo.DeliveryDate,
                    CustomerCode = planInfo.CustomerCode,
                    IndicatorQuantity = planInfo.IndicatorQuantity,
                    OrderNumber = planInfo.OrderNumber,
                    DrawingCode = planInfo.DrawingCode,
                    AssemblyProductCode = planInfo.AssemblyProductCode,
                    ProductCode = planInfo.ProductCode,
                    ProductName = planInfo.ProductName,
                    ReferenceType = planInfo.ReferenceType,
                    //Thùng cuối cùng là số lẻ còn lại của các thùng trc
                    Quantity = quantity,
                    StampReleaseDt = planInfo.StampReleaseDt,
                    StampCode = planInfo.StampCode,
                    PackageAmount = planInfo.PackageAmount
                });
            }

            return new GenericResponseResult<BoxStampInfo>(listData);
        }

        public async Task<GenericResponseResult<BoxStampInfo>> GetExportPlansStampInfo(int id)
        {
            //var plan = await _repo.ExportListPlan.GetAsync(id);
            var planInfo = await _repo.ExportListPlan.GetAsync(id);

            //Check exist with ID
            if (planInfo is null)
                return new GenericResponseResult<BoxStampInfo>($"Kế hoạch không tồn tại!");

            if (planInfo.ValidFlg != (int)EnumCommon.Status.Valid)
                return new GenericResponseResult<BoxStampInfo>($"Kế hoạch không hợp lệ!");

            //Check xem master data có tồn tại đơn hàng không
            if (planInfo.ProductID <= 0)
            {
                //Check xem master data có tồn tại đơn hàng không
                var existMstData = new List<MstData>();

                //if (!String.IsNullOrEmpty(data.AssemblyProductCode) && data.ExportType == (int)EnumExportType.Type.ASSY)
                if (planInfo.ExportType == (int)EnumExportType.Type.ASSY)
                {
                    existMstData = await _repo.MstData.GetAllListAsync(x =>
                        x.ValidFlg == (int)EnumCommon.Status.Valid &&
                        x.Customer.ToLower() == planInfo.CustomerCode.ToLower() &&
                        x.InternalDrawingCode.ToLower() == planInfo.DrawingCode.ToLower() &&
                        x.AssemblyProductCode.ToLower() == planInfo.AssemblyProductCode.ToLower() &&
                        x.ProductCode.ToLower() == planInfo.ProductCode.ToLower()
                    );
                }
                else
                {
                    existMstData = await _repo.MstData.GetAllListAsync(x =>
                        x.ValidFlg == (int)EnumCommon.Status.Valid &&
                        x.Customer.ToLower() == planInfo.CustomerCode.ToLower() &&
                        x.InternalDrawingCode.ToLower() == planInfo.DrawingCode.ToLower() &&
                        x.ProductCode.ToLower() == planInfo.ProductCode.ToLower()
                    );
                    if (existMstData.Count > 1)
                    {   // #8834
                        var ms = new List<MstData>();
                        if (planInfo.ExportType == 0)
                        {
                            ms = existMstData.Where(e => e.ExportLooseDomestic == 1).ToList();
                        }
                        else if (planInfo.ExportType == 1)
                        {
                            ms = existMstData.Where(e => e.ExportLooseInternational == 1).ToList();
                        }
                        if (ms.Count == 1)
                        {
                            existMstData = ms;
                        }
                    }
                }

                if (existMstData.Count == 0)
                {
                    return new GenericResponseResult<BoxStampInfo>(CommonConstant.PLAN_NOT_MATCH_MASTER);
                }
                else if (existMstData.Count > 1)
                {
                    return new GenericResponseResult<BoxStampInfo>(String.Format(CommonConstant.PLAN_DUPLICATE_MASTER, existMstData.Count));
                }
                planInfo.ProductID = existMstData[0].ProductID;
                await _repo.ExportListPlan.UpdateAsync(planInfo);
                await _repo.SaveAync();
            }

            var mstData = await _repo.MstData.GetAsync(planInfo.ProductID);
            if (mstData is null)
                return new GenericResponseResult<BoxStampInfo>(CommonConstant.PLAN_NOT_MATCH_MASTER);

            /*BoxNo = plan.BoxNo > 0 ? plan.BoxNo : (
                mstData.PackageAmount != null ? (mstData.PackageAmount != 0 ? ((plan.IndicatorQuantity % mstData.PackageAmount != 0) ? plan.IndicatorQuantity / mstData.PackageAmount + 1 : plan.IndicatorQuantity / mstData.PackageAmount) : 0) : 0
            ),*/
            int boxNo = 0;
            if (planInfo.BoxNo > 0)
            {
                boxNo = planInfo.BoxNo;
            }
            else if (mstData.PackageAmount != 0)
            {
                //boxNo = (planInfo.IndicatorQuantity % mstData.PackageAmount != 0) ? planInfo.IndicatorQuantity / mstData.PackageAmount + 1 : planInfo.IndicatorQuantity / mstData.PackageAmount;
                boxNo = (int)Math.Ceiling((decimal)planInfo.IndicatorQuantity / mstData.PackageAmount);
            }
            //int packageAmount = mstData.PackageAmount != null ? mstData.PackageAmount : 0;

            List<BoxStampInfo> listData = new List<BoxStampInfo>();
            //for (int i = 1; i <= planInfo.BoxNo; i++)
            for (int i = 1; i <= boxNo; i++)
            {
                //Thùng cuối cùng là số lẻ còn lại của các thùng trc
                int quantity = (i != boxNo ? mstData.PackageAmount : planInfo.IndicatorQuantity - (boxNo - 1) * mstData.PackageAmount);
                //int quantity = (i != planInfo.BoxNo ? planInfo.PackageAmount : planInfo.IndicatorQuantity - (planInfo.BoxNo - 1) * planInfo.PackageAmount);
                //quantity = (i != planInfo.BoxNo ? planInfo.PackageAmount : planInfo.IndicatorQuantity - (i - 1) * planInfo.PackageAmount);
                /*if (i < planInfo.BoxNo) {
                    quantity = planInfo.PackageAmount;
                } else {
                    quantity = planInfo.IndicatorQuantity - (planInfo.BoxNo - 1) * planInfo.PackageAmount;
                }*/
                listData.Add(new BoxStampInfo
                {
                    //BoxNo = i,    // !? Số lượng thùng hàng
                    //BoxNo = planInfo.BoxNo,
                    BoxNo = boxNo,
                    BoxSerial = DateTime.Now.ToString("yyyyMMddHHmmss") + i.ToString(),
                    DeliveryDate = planInfo.DeliveryDate,
                    CustomerCode = planInfo.CustomerCode,
                    IndicatorQuantity = planInfo.IndicatorQuantity,
                    OrderNumber = planInfo.OrderNumber,
                    DrawingCode = planInfo.DrawingCode,
                    AssemblyProductCode = planInfo.AssemblyProductCode,
                    ProductCode = planInfo.ProductCode,
                    ProductName = planInfo.ProductName,
                    ReferenceType = mstData.ReferenceType,
                    //ReferenceType = mstData.ReferenceType != null ? mstData.ReferenceType : -1,
                    Quantity = quantity,
                    StampReleaseDt = mstData.StampReleaseDt,
                    StampCode = mstData.StampCode,
                    PackageAmount = mstData.PackageAmount
                });
            }
            return new GenericResponseResult<BoxStampInfo>(listData);
        }

        public async Task<ServiceResult> GetSpecialStampInfo(SpecialStampInfo info)
        {
            try
            {
                var lstData = new List<MstData>();

                if (string.IsNullOrEmpty(info.AssemblyProductCode))
                {
                    //Nếu là xuất rời trong nước thì mã sản phẩm là duy nhất
                    if (info.ExportType == (int)EnumExportType.Type.LooseDomestic)
                        lstData = await _repo.MstData.GetAllListAsync(x => x.ValidFlg == (int)EnumCommon.Status.Valid
                        //&& x.ReferenceType == (int)EnumExportType.Type.LooseDomestic 
                        && x.ExportLooseDomestic == 1
                        && x.ProductCode.ToLower().Equals(info.ProductCode.ToLower()));
                    else
                        lstData = await _repo.MstData.GetAllListAsync(x => x.ValidFlg == (int)EnumCommon.Status.Valid
                        //&& x.ReferenceType == (int)EnumExportType.Type.LooseInternational
                        && x.ExportLooseInternational == 1
                        && x.InternalDrawingCode.ToLower().Equals(info.DrawingCode.ToLower())
                        && x.ProductCode.ToLower().Equals(info.ProductCode.ToLower()));
                }
                else
                {
                    //Nếu có mã lắp cụm => chỉ lấy theo mã lắp cụm
                    lstData = await _repo.MstData.GetAllListAsync(x => x.ValidFlg == (int)EnumCommon.Status.Valid && x.ReferenceType == (int)EnumReferenceType.Type.Assy && x.AssemblyProductCode.ToLower().Equals(info.AssemblyProductCode.ToLower()));
                }

                if (lstData == null || lstData.Count == 0)
                    return new ServiceResultError("Dữ liệu master data không tồn tại!");

                if (lstData.Count != 1)
                {
                    var ms = lstData.Where(e => e.Customer == info.CustomerCode).ToList();
                    if (ms.Count == 1)
                    {
                        return new ServiceResultSuccess("Lấy dữ liệu thành công!", ms.FirstOrDefault());
                    }
                    else if (ms.Count == 0)
                    {
                        return new ServiceResultError("Dữ liệu master data không tồn tại!");
                    }
                    return new ServiceResultError(String.Format(CommonConstant.PLAN_DUPLICATE_MASTER, lstData.Count));
                }

                var data = _mapper.Map<MstDataDto>(lstData.FirstOrDefault());
                return new ServiceResultSuccess("Lấy dữ liệu thành công!", data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return new ServiceResultError("Lỗi khi lấy thông tin temp đặc biệt: " + ex.Message);
            }
        }

        public async Task<ServiceResult> CheckBoxInfo(string boxSerial)
        {
            try
            {
                if (string.IsNullOrEmpty(boxSerial))
                {
                    return new ServiceResultNG(CommonConstant.NOT_GOOD);
                    //return new ServiceResultError("Không có số serial của thùng hàng được gửi lên!");
                }

                BoxInfo boxInfo = await _repo.BoxInfo.FirstOrDefaultAsync(x => x.BoxSerial == boxSerial && x.ValidFlg == (int)EnumCommon.Status.Valid);

                if (boxInfo != null)
                {
                    return new ServiceResultNG("SERIALBOXEXITS");
                    //return new ServiceResultNG(CommonConstant.NOT_GOOD);
                    //return new ServiceResultNG("Thùng hàng đã được nhập kho!");
                }

                return new ServiceResultSuccess(CommonConstant.OK);
                //return new ServiceResultSuccess("Thùng hàng chưa được nhập kho!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return new ServiceResultNG(ex.Message);
                //return new ServiceResultNG(CommonConstant.NOT_GOOD);
                //return new ServiceResultError("Lỗi khi kiểm tra thông tin thùng hàng: " + ex.Message);
            }
        }

        #endregion

        #region CRUD

        public async Task<ServiceResult> UpdateListExportHistory(List<ExportHistoryListDto> lstData)
        {
            try
            {
                var IDPlans = lstData.Select(x => x.ExportPlanID).Distinct();

                if (IDPlans.Count() != 1)
                    return new ServiceResultError("Dữ liệu cập nhật không ở cùng 1 kế hoạch");

                var IDPlan = IDPlans.FirstOrDefault();

                var plan = await _repo.ExportListPlan.GetAsync(IDPlan);

                if (plan is null)
                    return new ServiceResultError("Kế hoạch không tồn tại!");

                if (plan.ValidFlg != (int)EnumCommon.Status.Valid)
                    return new ServiceResultError("Kế hoạch không hợp lệ!");

                var entities = _mapper.Map<List<ExportHistoryList>>(lstData);

                await _repo.ExportHistoryList.UpdateAsync(entities);
                await _repo.SaveAync();
                return new ServiceResultSuccess("Lưu dữ liệu thành công!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return new ServiceResultError("Lỗi khi cập nhật lịch sử xuất kho: " + ex.Message);
            }
        }

        #region Insert
        public async Task<ServiceResult> InsertHistoryPlan(ExportHistoryListDto dto)
        {
            try
            {
                var plan = await _repo.ExportListPlan.GetAsync(dto.ExportPlanID);

                if (plan is null) return new ServiceResultError("Kế hoạch xuất kho không tồn tại");

                if (plan.ValidFlg != (int)EnumCommon.Status.Valid) return new ServiceResultError("Kế hoạch xuất kho không hợp lệ");

                var entity = _mapper.Map<ExportHistoryList>(dto);
                await _repo.ExportHistoryList.InsertAsync(entity);
                await _repo.SaveAync();
                return new ServiceResultSuccess("Thêm lịch sử xuất kho thành công!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return new ServiceResultError("Lỗi khi thêm lịch sử xuất kho: " + ex.Message);
            }
        }
        /// <summary>
        /// Insert lịch sử thùng hàng
        /// Nhóm lại theo khách hàng, mã sản phẩm
        /// Insert 1 đơn hàng ảo (Chưa có mã order)
        /// Insert lịch sử với ID thằng bố
        /// </summary>
        /// <param name="lstData"></param>
        /// <returns></returns>
        public async Task<ServiceResult> InsertListExportHistory(List<ImportPlanHistory> lstData)
        {
            try
            {
                // Đánh dấu sản phẩm đã được quét và đóng gói cho vào thùng
                foreach (var item in lstData)
                {
                    var ecu = await _repo.EcuData.FirstOrDefaultAsync(x => x.HUCode == item.HUSerial && x.Result == (int)EnumCommon.Status.Valid && x.ValidFlg == (int)EnumCommon.Status.Valid);

                    if (ecu is null) return new ServiceResultError($"Ecu data không tồn tại: {item.HUSerial}");
                    if (ecu.PackState > (int)EnumPackState.State.NotPacked) return new ServiceResultError($"Sản phẩm đã được nhập kho: {item.HUSerial}");
                }

                //Nhóm lại
                /*var groupPlan = lstData.GroupBy(x => new { x.ProductCode, x.Customer });
                foreach (var item in groupPlan)
                {
                    //Thêm kế hoạch ảo => TODO: remove
                    var plan = new ExportListPlan
                    {
                        OrderNumber = string.Empty,
                        CustomerCode = item.FirstOrDefault()?.CustomerCode,
                        Customer = item.FirstOrDefault()?.Customer,
                        DeliveryDate = item.FirstOrDefault()?.DeliveryDate,
                        ProductCode = item.FirstOrDefault()?.ProductCode,
                        ProductName = item.FirstOrDefault()?.ProductName,
                        IndicatorQuantity = item.FirstOrDefault().IndicatorQuantity,
                        BoxNo = item.FirstOrDefault().BoxNo,
                        DrawingCode = item.FirstOrDefault()?.DrawingCode,
                        ExportType = item.FirstOrDefault().ExportType
                    };


                    //Set trạng thái đơn hàng. Nếu tổng khối lượng >= số lượng chỉ thị => Đã đủ
                    var totalQuantrity = item.Sum(x => x.Quantity);
                    if (totalQuantrity >= item.FirstOrDefault().IndicatorQuantity)
                        plan.OrderState = (int)EnumOrderState.State.Enough;
                    else
                        plan.OrderState = (int)EnumOrderState.State.Processing;

                    await _repo.ExportListPlan.InsertAsync(plan);
                    await _repo.SaveAync();

                    //Them lich su
                    var lstHistory = item.Select(x => new ExportHistoryList
                    {
                        ExportHistoryID = 0,
                        //ExportPlanID = plan.ExportPlanID,
                        HUSerial = x.HUSerial,
                        TimeReadHU = x.TimeReadHU,
                        TimeReadQrCode = x.TimeReadQrCode,
                        LaserEngraving = x.LaserEngraving,
                        ShiftWork = x.ShiftWork,
                        Compare = x.Compare,
                        Implementer = x.Implementer,
                        OrderState = x.OrderState,
                        BoxSerial = x.BoxSerial,
                        Quantity = x.Quantity,
                    }).ToList();

                    await _repo.ExportHistoryList.InsertAsync(lstHistory);
                    await _repo.SaveAync();
                }*/

                // Thêm thông tin thùng hàng vào lịch sử xuất nhập kho
                var groupPlan = lstData.GroupBy(x => new { x.BoxSerial });
                foreach (var item in groupPlan)
                {
                    String boxSerial = item.FirstOrDefault() != null ? item.FirstOrDefault()!.BoxSerial : "";
                    BoxInfo _boxInfo = await _repo.BoxInfo.FirstOrDefaultAsync(x => x.BoxSerial == boxSerial && x.ValidFlg == (int)EnumCommon.Status.Valid);
                    if (_boxInfo != null && _boxInfo.BoxState != (int)EnumBoxState.State.NotExport) return new ServiceResultError($": {boxSerial}"); // ($"Thùng hàng đã được nhập kho: {boxSerial}");

                    //Thông tin thùng hàng
                    var boxInfo = new BoxInfo
                    {
                        BoxID = 0,
                        BoxSerial = item.FirstOrDefault()!.BoxSerial,
                        TimeReadQrCode = item.FirstOrDefault()?.TimeReadQrCode,
                        Implementer = item.FirstOrDefault()?.Implementer,
                        IndicatorQuantity = item.FirstOrDefault() != null ? item.FirstOrDefault()!.Quantity : 0,
                        Quantity = item.Count(),
                        BoxState = (int)EnumBoxState.State.Enough,
                        Customer = item.FirstOrDefault()?.Customer,
                        //DeliveryDate = item.FirstOrDefault()?.DeliveryDate,
                        //IndicatorQuantity = item.FirstOrDefault().IndicatorQuantity,
                        //BoxNo = item.FirstOrDefault().BoxNo,       //Số lượng hộp
                        //OrderState = (int)EnumOrderState.State.Enough,
                        DrawingCode = item.FirstOrDefault()?.DrawingCode,
                        AssemblyProductCode = item.FirstOrDefault()?.AssemblyProductCode,
                        ProductCode = item.FirstOrDefault()?.ProductCode,
                        ProductName = item.FirstOrDefault()?.ProductName,
                        ExportType = item.FirstOrDefault()!.ExportType,
                        OrderState = item.FirstOrDefault().OrderState,      // (int)EnumOrderState.State.Enough | 2
                    };

                    await _repo.BoxInfo.InsertAsync(boxInfo);
                    await _repo.SaveAync();
                }


                // Thêm thông tin sản phẩm vào lịch sử xuất nhập kho
                var lstHistory = lstData.Select(x => new ExportHistoryList
                {
                    ExportHistoryID = 0,
                    //ExportPlanID = plan.ExportPlanID,
                    BoxSerial = x.BoxSerial,
                    TimeReadQrCode = x.TimeReadQrCode,  // TODO: move to boxinfo
                    HUSerial = x.HUSerial,
                    TimeReadHU = x.TimeReadHU,
                    LaserEngraving = x.LaserEngraving,
                    ShiftWork = x.ShiftWork,
                    Compare = x.Compare,
                    Implementer = x.Implementer,        // Người nhập kho
                    OrderState = x.OrderState,
                    Quantity = x.Quantity,
                }).ToList();

                await _repo.ExportHistoryList.InsertAsync(lstHistory);
                await _repo.SaveAync();

                foreach (var item in lstData)
                {
                    var ecu = await _repo.EcuData.FirstOrDefaultAsync(x => x.HUCode == item.HUSerial && x.Result == (int)EnumCommon.Status.Valid && x.ValidFlg == (int)EnumCommon.Status.Valid);

                    if (ecu is null) return new ServiceResultError($"Ecu data không tồn tại: {item.HUSerial}");
                    if (ecu.PackState > (int)EnumPackState.State.NotPacked) return new ServiceResultError($"Sản phẩm đã được nhập kho: {item.HUSerial}");

                    // TODO: test with HUCode = '5D 3N01-C-0488'
                    ecu.PackState = (int)EnumPackState.State.Packed;
                    await _repo.EcuData.UpdateAsync(ecu);
                    await _repo.SaveAync();
                }

                return new ServiceResultSuccess("Lưu dữ liệu thành công!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return new ServiceResultError("Lỗi khi cập nhật lịch sử xuất kho: " + ex.Message);
            }
        }

        public async Task<ServiceResult> InsertListExportPlan(List<ExportListPlanImportDto> lstData)
        {
            try
            {
                if (lstData is null || lstData.Count == 0) return new ServiceResultError("Không có dữ liệu được gửi lên!");

                //Tự động sinh mã đơn hàng
                if (lstData.Any(x => string.IsNullOrEmpty(x.OrderNumber) || string.IsNullOrEmpty(x.OrderNumber.Trim())))
                {
                    int index = 0;
                    foreach (var item in lstData)
                    {
                        if (string.IsNullOrEmpty(item.OrderNumber) || string.IsNullOrEmpty(item.OrderNumber.Trim()))
                        {
                            index++;
                            item.OrderNumber = DateTime.Now.ToString("yyyyMMddHHmmss") + index.ToString();
                        }
                    }
                }

                //Check có tồn tại trong master data ko
                var dictMiss = Enumerable.Range(0, 0).Select(e => new { customerCode = "", drawingCode = "", assemblyProductCode = "", productCode = "" }).ToList();
                var dictDuplicate = Enumerable.Range(0, 0).Select(e => new { customerCode = "", drawingCode = "", assemblyProductCode = "", productCode = "" }).ToList();
                var dictInvalid = Enumerable.Range(0, 0).Select(e => new { customerCode = "", drawingCode = "", assemblyProductCode = "", productCode = "" }).ToList();
                var dictError = Enumerable.Range(0, 0).Select(e => new { customerCode = "", drawingCode = "", assemblyProductCode = "", productCode = "" }).ToList();

                //Check có tồn tại trong kế hoạch xuất kho ko
                List<ExportListPlanImportDto> newPlans = new List<ExportListPlanImportDto>();
                List<ExportListPlanImportDto> existPlans = new List<ExportListPlanImportDto>();
                //List<ExportListPlan> existPlans = new List<ExportListPlan>();

                //var processingPlans = Enumerable.Range(0, 0).Select(e => new { orderNumber = "", orderState = "", customerCode = "", drawingCode = "", assemblyProductCode = "", productCode = "" }).ToList();
                //var exportedPlan = Enumerable.Range(0, 0).Select(e => new { orderNumber = "", orderState = "", customerCode = "", drawingCode = "", assemblyProductCode = "", productCode = "" }).ToList();
                int processingPlans = 0;
                int exportedPlan = 0;

                foreach (var plan in lstData)
                {
                    //bool existMstData = await _repo.MstData.AnyAsync(x => x.ValidFlg == (int)EnumCommon.Status.Valid && x.Customer.ToLower() == plan.CustomerCode.ToLower() && x.ProductCode.ToLower() == plan.ProductCode.ToLower() && x.InternalDrawingCode.ToLower() == plan.DrawingCode.ToLower());
                    //int existMstData;
                    var existMstData = new List<MstData>();
                    var existOldPlans = new List<ExportListPlan>();
                    //ExportListPlan existPlan = null;

                    if (!String.IsNullOrEmpty(plan.AssemblyProductCode) && plan.ExportType == (int)EnumExportType.Type.ASSY)
                    {
                        existMstData = await _repo.MstData.GetAllListAsync(
                            x => x.ValidFlg == (int)EnumCommon.Status.Valid &&
                            x.AssemblyProductCode.ToLower() == plan.AssemblyProductCode.ToLower()
                        );
                        if (existMstData.Count > 1)
                        {
                            existMstData = await _repo.MstData.GetAllListAsync(
                                x => x.ValidFlg == (int)EnumCommon.Status.Valid &&
                                x.Customer.ToLower() == plan.CustomerCode.ToLower() &&
                                x.InternalDrawingCode.ToLower() == plan.DrawingCode.ToLower() &&
                                x.AssemblyProductCode.ToLower() == plan.AssemblyProductCode.ToLower() &&
                                x.ProductCode.ToLower() == plan.ProductCode.ToLower()
                            );
                        }
                        if (existMstData.Count == 1)
                        {
                            plan.ProductID = existMstData[0].ProductID;
                            if (String.IsNullOrEmpty(plan.Customer))
                            {
                                plan.Customer = existMstData[0].Customer;
                                plan.CustomerCode = existMstData[0].Customer;
                            }
                            if (String.IsNullOrEmpty(plan.DrawingCode))
                            {
                                plan.DrawingCode = existMstData[0].InternalDrawingCode;
                            }
                            if (String.IsNullOrEmpty(plan.ProductCode))
                            {
                                plan.ProductCode = existMstData[0].ProductCode;
                            }
                            if (plan.BoxNo == 0 && existMstData[0].PackageAmount > 0)
                            {
                                plan.BoxNo = (int)Math.Ceiling((decimal)plan.IndicatorQuantity / existMstData[0].PackageAmount);
                            }
                        }

                        existOldPlans = await _repo.ExportListPlan.GetAllListAsync(x =>
                            x.ValidFlg == (int)EnumCommon.Status.Valid &&
                            x.ExportType == plan.ExportType &&
                            x.AssemblyProductCode.ToLower() == plan.AssemblyProductCode.ToLower() &&
                            DateTime.Compare((DateTime)x.DeliveryDate!, (DateTime)plan.DeliveryDate!) == 0
                        );
                    }
                    else
                    {
                        if (String.IsNullOrEmpty(plan.DrawingCode))
                        {
                            existMstData = await _repo.MstData.GetAllListAsync(x =>
                                x.ValidFlg == (int)EnumCommon.Status.Valid &&
                                x.Customer.ToLower() == plan.CustomerCode.ToLower() &&
                                x.ProductCode.ToLower() == plan.ProductCode.ToLower()
                            );
                            existOldPlans = await _repo.ExportListPlan.GetAllListAsync(x =>
                                x.ValidFlg == (int)EnumCommon.Status.Valid &&
                                x.ExportType == plan.ExportType &&
                                x.Customer.ToLower() == plan.Customer.ToLower() &&
                                x.ProductCode.ToLower() == plan.ProductCode.ToLower() &&
                                DateTime.Compare((DateTime)x.DeliveryDate!, (DateTime)plan.DeliveryDate!) == 0
                            );
                        }
                        else if (String.IsNullOrEmpty(plan.ProductCode))
                        {
                            existMstData = await _repo.MstData.GetAllListAsync(x =>
                                x.ValidFlg == (int)EnumCommon.Status.Valid &&
                                x.Customer.ToLower() == plan.CustomerCode.ToLower() &&
                                x.InternalDrawingCode.ToLower() == plan.DrawingCode.ToLower()
                            );
                            existOldPlans = await _repo.ExportListPlan.GetAllListAsync(x =>
                                x.ValidFlg == (int)EnumCommon.Status.Valid &&
                                x.ExportType == plan.ExportType &&
                                x.Customer.ToLower() == plan.Customer.ToLower() &&
                                x.DrawingCode.ToLower() == plan.DrawingCode.ToLower() &&
                                DateTime.Compare((DateTime)x.DeliveryDate!, (DateTime)plan.DeliveryDate!) == 0
                            );
                        }
                        else
                        {
                            existMstData = await _repo.MstData.GetAllListAsync(x =>
                                x.ValidFlg == (int)EnumCommon.Status.Valid &&
                                x.Customer.ToLower() == plan.CustomerCode.ToLower() &&
                                x.InternalDrawingCode.ToLower() == plan.DrawingCode.ToLower() &&
                                x.ProductCode.ToLower() == plan.ProductCode.ToLower()
                            );
                            existOldPlans = await _repo.ExportListPlan.GetAllListAsync(x =>
                                x.ValidFlg == (int)EnumCommon.Status.Valid &&
                                x.ExportType == plan.ExportType &&
                                x.Customer.ToLower() == plan.Customer.ToLower() &&
                                x.DrawingCode.ToLower() == plan.DrawingCode.ToLower() &&
                                x.ProductCode.ToLower() == plan.ProductCode.ToLower() &&
                                DateTime.Compare((DateTime)x.DeliveryDate!, (DateTime)plan.DeliveryDate!) == 0
                            );
                        }
                        if (existMstData.Count > 1)
                        {   // #8834
                            var ms = new List<MstData>();
                            if (plan.ExportType == 0)
                            {
                                ms = existMstData.Where(e => e.ExportLooseDomestic == 1).ToList();
                            }
                            else if (plan.ExportType == 1)
                            {
                                ms = existMstData.Where(e => e.ExportLooseInternational == 1).ToList();
                            }
                            if (ms.Count == 1)
                            {
                                existMstData = ms;
                                //plan.ProductID = existMstData[0].ProductID;
                            }
                        }
                        if (existMstData.Count == 1)
                        {
                            plan.ProductID = existMstData[0].ProductID;
                            if (String.IsNullOrEmpty(plan.Customer))
                            {
                                plan.Customer = existMstData[0].Customer;
                                plan.CustomerCode = existMstData[0].Customer;
                            }
                            if (String.IsNullOrEmpty(plan.DrawingCode))
                            {
                                plan.DrawingCode = existMstData[0].InternalDrawingCode;
                            }
                            if (String.IsNullOrEmpty(plan.ProductCode))
                            {
                                plan.ProductCode = existMstData[0].ProductCode;
                            }
                            if (plan.BoxNo == 0 && existMstData[0].PackageAmount > 0)
                            {
                                plan.BoxNo = (int)Math.Ceiling((decimal)plan.IndicatorQuantity / existMstData[0].PackageAmount);
                            }
                        }
                    }

                    //Check có tồn tại trong master data ko
                    if (existMstData.Count == 0)
                    {
                        dictMiss.Add(new { customerCode = plan.CustomerCode, drawingCode = plan.DrawingCode, assemblyProductCode = plan.AssemblyProductCode, productCode = plan.ProductCode });
                        dictError.Add(new { customerCode = plan.CustomerCode, drawingCode = plan.DrawingCode, assemblyProductCode = plan.AssemblyProductCode, productCode = plan.ProductCode });
                    }
                    else if (existMstData.Count > 1)
                    {
                        dictDuplicate.Add(new { customerCode = plan.CustomerCode, drawingCode = plan.DrawingCode, assemblyProductCode = plan.AssemblyProductCode, productCode = plan.ProductCode });
                        dictError.Add(new { customerCode = plan.CustomerCode, drawingCode = plan.DrawingCode, assemblyProductCode = plan.AssemblyProductCode, productCode = plan.ProductCode });
                    }
                    else if (existMstData[0].ReferenceType < 0)
                    {
                        dictInvalid.Add(new { customerCode = plan.CustomerCode, drawingCode = plan.DrawingCode, assemblyProductCode = plan.AssemblyProductCode, productCode = plan.ProductCode });
                        dictError.Add(new { customerCode = plan.CustomerCode, drawingCode = plan.DrawingCode, assemblyProductCode = plan.AssemblyProductCode, productCode = plan.ProductCode });
                    }

                    //Check có tồn tại trong kế hoạch xuất kho ko
                    var rejectExportedPlan = existOldPlans.Where(x => x.OrderState == (int)EnumOrderState.State.Exported);
                    var rejectProcessingPlan = existOldPlans.Where(x => x.BillState == (int)EnumCommon.Status.Valid);
                    if (existOldPlans.Count == 0)
                    {
                        newPlans.Add(plan);
                    }
                    //else if (existOldPlans.OrderState == (int)EnumOrderState.State.Exported)
                    else if (rejectExportedPlan.Any())
                    {
                        /*exportedPlan.Add(new {
                            orderNumber = rejectExportedPlan.FirstOrDefault()!.OrderNumber,
                            orderState = rejectExportedPlan.FirstOrDefault()!.OrderState.ToString(),
                            customerCode = plan.CustomerCode,
                            drawingCode = plan.DrawingCode,
                            assemblyProductCode = plan.AssemblyProductCode, 
                            productCode = plan.ProductCode
                        });*/
                        exportedPlan++;

                        // Xóa kế hoạch cũ bị trùng
                        var _existOldPlans = existOldPlans.Where(x => x.OrderState == (int)EnumOrderState.State.NotExport);
                        if (_existOldPlans.Any())
                        {
                            await _repo.ExportListPlan.DeleteAsync(_existOldPlans);
                            await _repo.SaveAync();
                            existPlans.Add(plan);
                        }
                        else
                        {
                            newPlans.Add(plan);
                        }
                    }
                    //else if (existOldPlans.BillState == (int)EnumCommon.Status.Valid)
                    else if (rejectProcessingPlan.Any())
                    {   //EnumCommon. 0 là chưa lên đơn. 1 là đã lên đơn
                        /*processingPlans.Add(new {
                            orderNumber = rejectProcessingPlan.FirstOrDefault()!.OrderNumber,
                            orderState = rejectProcessingPlan.FirstOrDefault()!.OrderState.ToString(),
                            customerCode = plan.CustomerCode,
                            drawingCode = plan.DrawingCode,
                            assemblyProductCode = plan.AssemblyProductCode,
                            productCode = plan.ProductCode
                        });*/
                        processingPlans++;

                        // Xóa kế hoạch cũ bị trùng
                        var _existOldPlans = existOldPlans.Where(x => x.OrderState == (int)EnumOrderState.State.NotExport);
                        if (_existOldPlans.Any())
                        {
                            await _repo.ExportListPlan.DeleteAsync(_existOldPlans);
                            await _repo.SaveAync();
                            existPlans.Add(plan);
                        }
                        else
                        {
                            newPlans.Add(plan);
                        }
                    }
                    else
                    {
                        // Update kế hoạch cũ
                        //existPlan.IndicatorQuantity = plan.IndicatorQuantity;
                        //existPlan.ProductName = plan.ProductName;
                        //existPlan.BoxNo = 0;
                        //existPlans.Add(existPlan);

                        // Xóa kế hoạch cũ bị trùng
                        await _repo.ExportListPlan.DeleteAsync(existOldPlans);
                        await _repo.SaveAync();
                        existPlans.Add(plan);
                    }
#if DEBUG
                    Console.WriteLine($"{plan.Customer} {plan.DeliveryDate} {plan.DrawingCode} {plan.ProductCode} {plan.AssemblyProductCode} : {newPlans.Count} {exportedPlan} {processingPlans} {existOldPlans.Count}");
#endif
                }

                if (dictMiss.Count > 0 || dictDuplicate.Count > 0)
                {
                    string message = "";
                    if (dictMiss.Count == 0)
                    {
                        message = "Sản phẩm trùng khớp với nhiều master data";
                        return new ServiceResultNG(message, dictDuplicate);
                    }
                    else if (dictDuplicate.Count == 0)
                    {
                        message = "Sản phẩm không tồn tại ở master data";
                        return new ServiceResultNG(message, dictMiss);
                    }
                    else
                    {
                        message = "Sản phẩm không tồn tại hoặc trùng khớp với nhiều master data";
                        return new ServiceResultNG(message, dictError);
                    }
                }
                else if (dictInvalid.Count > 0)
                {
                    string message = "Sản phẩm có master data thiếu thông tin tham chiếu";
                    return new ServiceResultNG(message, dictInvalid);
                }

                var entityNew = new List<ExportListPlan>();
                //var entityExist = new List<ExportListPlan>();
                if (newPlans.Count == 0 && existPlans.Count == 0)
                {
                    //if (processingPlans.Count == 0)
                    //    return new ServiceResultError($"Đơn hàng chuẩn bị xuất!", processingPlans);
                    //return new ServiceResultError($"Đơn đang ở trạng thái đã xuất!", exportedPlan);

                    var err = $"{processingPlans} đơn hàng chuẩn bị xuất, {exportedPlan} đơn đang ở trạng thái đã xuất!";
                    //processingPlans.AddRange(exportedPlan);
                    //return new ServiceResultSuccess(err, processingPlans);
                    return new ServiceResultError(err, lstData);
                }
                else
                {
                    // Insert kế hoạch mới
                    entityNew = _mapper.Map<List<ExportListPlan>>(newPlans);
                    await _repo.ExportListPlan.InsertAsync(entityNew);
                    await _repo.SaveAync();
                }

                if (existPlans.Count > 0)
                {
                    // Update kế hoạch cũ
                    //await _repo.ExportListPlan.UpdateAsync(existPlans);

                    // Xóa kế hoạch cũ, và Insert kế hoạch trùng
                    //entityExist = _mapper.Map<List<ExportListPlan>>(existPlans);
                    var entities = _mapper.Map<List<ExportListPlan>>(existPlans);
                    await _repo.ExportListPlan.InsertAsync(entities);
                    await _repo.SaveAync();
                }
                //entityNew.AddRange(existPlans);

                var mes = $"Thêm {newPlans.Count} đơn hàng và cập nhật {existPlans.Count} đơn hàng thành công!";
                mes += $"\n{processingPlans} đơn hàng chuẩn bị xuất, {exportedPlan} đơn đang ở trạng thái đã xuất!";
                return new ServiceResultSuccess(mes, entityNew);

                //Order num is unique
                //Nếu tồn tại kế hoạch trùng mã đơn hàng thì kiểm tra
                //Nếu có đơn hàng ở trạng thái chưa xuất thì hỏi ngược lại người dùng có muốn cập nhật không? => Nếu có thì cập nhật
                //Nếu tồn tại đơn hàng ở trạng thái khác thì thông báo không có cập nhật
                /*var existPlan = await _repo.ExportListPlan.GetAllListAsync(x => lstData.Select(y => y.OrderNumber.ToLower()).Contains(x.OrderNumber.ToLower()) && x.ValidFlg == (int)EnumCommon.Status.Valid);
                if (existPlan.Any())
                {
                    var dictOrderState = CommonUtils.EnumToDic<EnumOrderState.State>();
                    var rejectPlan = existPlan.Where(x => x.OrderState != (int)EnumOrderState.State.NotExport);
                    if (rejectPlan.Any())
                    {
                        var dictErr = rejectPlan.Select(x => new { x.OrderNumber, x.OrderState });
                        var dataErr = new { plan = dictErr, token = "" };
                        return new ServiceResultError($"Mã đơn hàng đã bị trùng!", dataErr);
                    }
                    var notExportPlan = existPlan.Where(x => x.OrderState == (int)EnumOrderState.State.NotExport);
                    if (notExportPlan.Any())
                    {
                        var dictWarn = notExportPlan.Select(x => new { x.OrderNumber, x.OrderState });
                        var dataWarn = new { plan = dictWarn, token = StringUtils.Encrypt(DateTime.Now.AddMinutes(10).ToString("yyyyMMddHHmmss")) };
                        return new ServiceResultWarning($"Đơn hàng đang ở trạng thái chưa xuất!", dataWarn);
                    }
                }*/

                //var entities = _mapper.Map<List<ExportListPlan>>(lstData);
                //await _repo.ExportListPlan.InsertAsync(entities);
                //await _repo.SaveAync();
                //return new ServiceResultSuccess("Thêm dữ liệu thành công!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return new ServiceResultError("Lỗi khi thêm kế hoạch: " + ex.Message);
            }
        }

        public async Task<ServiceResult> InsertListWarningPlan(ImportPlan condition)
        {
            try
            {
                //Không có token
                if (string.IsNullOrEmpty(condition.Token) || string.IsNullOrEmpty(condition.Token.Trim())) return new ServiceResultError("Token không được để trống!");
                //Token sai
                //DateTime? timeExpired = DateUtils.GetDate(StringUtils.Decrypt(condition.Token), "yyyyMMddHHmmss");
                //if (!timeExpired.HasValue) return new ServiceResultError("Token không hợp lệ!");
                //Token hết hạn
                //if (timeExpired.Value > DateTime.Now) return new ServiceResultError("Token đã hết hạn, vui lòng request lại!");

                //Tự động sinh mã
                if (condition.Data.Any(x => string.IsNullOrEmpty(x.OrderNumber) || string.IsNullOrEmpty(x.OrderNumber.Trim())))
                {
                    int index = 0;
                    foreach (var item in condition.Data)
                    {
                        if (string.IsNullOrEmpty(item.OrderNumber) || string.IsNullOrEmpty(item.OrderNumber.Trim()))
                        {
                            index++;
                            item.OrderNumber = DateTime.Now.ToString("yyyyMMddHHmmss") + index.ToString();
                        }
                    }
                }

                //Lấy danh sách các kế hoạch đã có và ở trạng thái chưa xuất
                var existPlan = await _repo.ExportListPlan.GetAllListAsync(x => condition.Data.Select(y => y.OrderNumber.ToLower()).Contains(x.OrderNumber.ToLower()) && x.ValidFlg == (int)EnumCommon.Status.Valid && x.OrderState == (int)EnumOrderState.State.NotExport);
                if (existPlan.Any())
                {
                    //Dữ liệu import đã tồn tại trong database
                    var existData = condition.Data.Where(x => existPlan.Select(y => y.OrderNumber.ToLower()).Contains(x.OrderNumber.ToLower())).ToList();
                    if (existData.Any())
                    {
                        foreach (var data in existData)
                        {
                            foreach (var plan in existPlan)
                            {
                                //Mapping các thay đổi về data
                                if (plan.OrderNumber.ToLower().Equals(data.OrderNumber.ToLower()))
                                {
                                    _mapper.Map(data, plan);
                                }
                            }
                        }

                        await _repo.ExportListPlan.UpdateAsync(existPlan);
                        await _repo.SaveAync();
                    }
                    //Dữ liệu import chưa tồn tại trong database
                    var notExistData = condition.Data.Where(x => !existPlan.Select(y => y.OrderNumber.ToLower()).Contains(x.OrderNumber)).ToList();
                    if (notExistData.Any())
                    {
                        var entities = _mapper.Map<List<ExportListPlan>>(notExistData);
                        await _repo.ExportListPlan.InsertAsync(entities);
                        await _repo.SaveAync();
                    }
                }
                else
                {
                    //Không tồn tại thì insert như bình thường
                    var entities = _mapper.Map<List<ExportListPlan>>(condition.Data);
                    await _repo.ExportListPlan.InsertAsync(entities);
                    await _repo.SaveAync();
                }
                return new ServiceResultSuccess("Thêm dữ liệu thành công!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return new ServiceResultError("Lỗi khi thêm kế hoạch: " + ex.Message);
            }
        }

        public async Task<ServiceResult> InsertExportPlan(ExportListPlanDto data)
        {
            try
            {
                //Tự động sinh mã đơn hàng
                if (string.IsNullOrEmpty(data.OrderNumber) || string.IsNullOrEmpty(data.OrderNumber.Trim()))
                    data.OrderNumber = DateTime.Now.ToString("yyyyMMddHHmmss");
                //Plan with ordernumber is unique
                var existPlan = await _repo.ExportListPlan.AnyAsync(x => x.ValidFlg == (int)EnumCommon.Status.Valid && x.OrderNumber.ToLower().Equals(data.OrderNumber.ToLower()));

                if (existPlan)
                    return new ServiceResultError($"Kế hoạch với mã đơn hàng {data.OrderNumber} đã tồn tại!", new { field = nameof(ExportListPlanDto.OrderNumber) });
                //Check xem master data có tồn tại đơn hàng không
                var existMstData = await _repo.MstData.AnyAsync(x => x.ValidFlg == (int)EnumCommon.Status.Valid && x.Customer.ToLower() == data.CustomerCode.ToLower() && x.ProductCode.ToLower() == data.ProductCode.ToLower());

                if (!existMstData)
                    return new ServiceResultError($"Sản phẩm không tồn tại ở master data", new { field = nameof(ExportListPlanDto.ProductCode) });

                //Todo: Other business
                var entity = _mapper.Map<ExportListPlan>(data);
                await _repo.ExportListPlan.InsertAsync(entity);
                await _repo.SaveAync();
                return new ServiceResultSuccess("Thêm kế hoạch thành công!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return new ServiceResultError("Lỗi khi thêm kế hoạch: " + ex.Message);
            }
        }

        /// <summary>
        /// API để cập nhập thùng hàng xuất kho vào đơn hàng (PC052)
        /// </summary>
        /// <param name="lstData"></param>
        /// <returns></returns>
        public async Task<ServiceResult> InsertScanBoxResult(List<ExportPlanScanBoxResult> lstData)
        {
            try
            {
                // Vui lòng kiểm tra lại dữ liệu
                if (lstData is null || lstData.Count == 0) return new ServiceResultError("Không có dữ liệu gửi lên!");

                // Các thùng phải thuộc cùng 1 đơn
                var orders = lstData.Select(x => x.OrderNumber).Distinct();
                if (orders.Count() != 1) return new ServiceResultError("Các thùng không cùng 1 đơn hàng");

                var orderNumber = orders.FirstOrDefault();

                // Đơn hàng phải chưa xuất
                var plan = await _repo.ExportListPlan.FirstOrDefaultAsync(x => x.OrderNumber.Equals(orderNumber) && x.ValidFlg == (int)EnumCommon.Status.Valid);

                if (plan is null) return new ServiceResultError("Đơn hàng không tồn tại!");
                if (plan.OrderState == (int)EnumOrderState.State.Exported) return new ServiceResultError("Đơn hàng đã được xuất kho!");
                if (plan.OrderState == (int)EnumOrderState.State.Cancel) return new ServiceResultError("Đơn hàng đã được hủy!");
                if (plan.OrderState != (int)EnumOrderState.State.NotExport) return new ServiceResultError("Đơn hàng không hợp lệ!");
                // TODO: check đơn hàng đã được lên đơn trong PC052 !?

                // Thùng hàng: đã đủ và chưa thuộc đơn hàng nào
                foreach (var item in lstData)
                {
                    BoxInfo boxInfo = await _repo.BoxInfo.FirstOrDefaultAsync(x => x.BoxSerial == item.BoxSerial && x.ValidFlg == (int)EnumCommon.Status.Valid);
                    if (boxInfo is null) return new ServiceResultError("BOXHASNOTWAREHOUSED"); // Thùng hàng chưa được nhập kho!
                    if (boxInfo.BoxState == (int)EnumBoxState.State.NotExport) return new ServiceResultError($"Thùng hàng chưa đủ số lượng trong hộp: {boxInfo.BoxSerial}");
                    if (boxInfo.BoxState == (int)EnumBoxState.State.Exported) return new ServiceResultError($"Thùng hàng đã được xuất kho: {boxInfo.BoxSerial}");
                    if (boxInfo.ExportPlanID != 0) return new ServiceResultError($"Thùng hàng \"{boxInfo.BoxSerial}\" đang thuộc đơn hàng: {boxInfo.ExportPlanID}");
                    if (boxInfo.ExportType == (int)EnumExportType.Type.ASSY)
                    {
                        if (!String.IsNullOrEmpty(plan.AssemblyProductCode))
                            if (!boxInfo.AssemblyProductCode!.Equals(plan.AssemblyProductCode)) return new ServiceResultError($"Đơn hàng và thùng hàng có mã số sản phẩm lắp cụm khác nhau: {plan.AssemblyProductCode} - {boxInfo.AssemblyProductCode}");
                    }
                    else
                    {
                        if (!boxInfo.DrawingCode!.Equals(plan.DrawingCode)) return new ServiceResultError($"Đơn hàng và thùng hàng có mã bản vẽ khác nhau: {plan.DrawingCode} - {boxInfo.DrawingCode}");
                        if (!boxInfo.ProductCode!.Equals(plan.ProductCode)) return new ServiceResultError($"Đơn hàng và thùng hàng có mã số sản phẩm khác nhau: {plan.ProductCode} - {boxInfo.ProductCode}");
                    }
                }

                // Kiểm tra số lượng và so với số lượng chỉ thị
                // Chỉ lấy những kết quả là OK
                var total = lstData.Where(x => x.Result.Equals(CommonConstant.OK)).Sum(x => x.Quantity);

                if (plan.IndicatorQuantity != total) return new ServiceResultError("Số lượng không khớp với đơn hàng! Số lượng chỉ thị: " + plan.IndicatorQuantity);

                // Kiểm tra các thùng đã có sẵn trong DB
                // Nếu có thì update lại ID đơn hàng
                var existHistory = await _repo.ExportHistoryList.GetAllListAsync(x => x.ValidFlg == (int)EnumCommon.Status.Valid && lstData.Select(y => y.BoxSerial).Contains(x.BoxSerial));

                if (existHistory.Any())
                {
                    string warehouseReleasePerson = lstData![0].WarehouseReleasePerson;

                    foreach (var item in existHistory)
                    {
                        item.ExportPlanID = plan.ExportPlanID;
                        item.OrderState = (int)EnumOrderState.State.Enough;
                        //Thêm thời điểm xuất kho và người xuất kho
                        item.TimeRelease = DateTime.Now;
                        //item.WarehouseReleasePerson = _userPrincipalService.Username;
                        //item.WarehouseReleasePerson = plan.WarehouseReleasePerson;
                        item.WarehouseReleasePerson = warehouseReleasePerson;
                    }

                    var notExist = lstData.Where(x => !existHistory.Select(y => y.BoxSerial).Contains(x.BoxSerial)).ToList();

                    if (notExist.Any())
                    {
                        var entities = notExist.Select(x => new ExportHistoryList
                        {
                            ExportHistoryID = 0,
                            BoxSerial = x.BoxSerial,
                            Compare = x.Result,
                            ExportPlanID = plan.ExportPlanID,
                            HUSerial = x.HUSerial,
                            Implementer = x.Implementer,
                            LaserEngraving = x.LaserEngraving,
                            OrderState = (int)EnumOrderState.State.Exported,
                            Quantity = x.Quantity,
                            TimeReadHU = x.TimeReadHU,
                            //TimeRelease = DateTime.Now,
                            TimeRelease = x.TimeRelease,
                            TimeReadQrCode = x.TimeReadQrCode,
                            ShiftWork = x.ShiftWork,
                            //WarehouseReleasePerson = _userPrincipalService.Username,
                            //WarehouseReleasePerson = x.WarehouseReleasePerson,
                            WarehouseReleasePerson = warehouseReleasePerson,
                            ValidFlg = (int)EnumCommon.Status.Valid,
                            Exported = (int)EnumCommon.Status.Valid,
                        });

                        await _repo.ExportHistoryList.InsertAsync(entities);
                    }

                    await _repo.ExportHistoryList.UpdateAsync(existHistory);

                    var existEcu = await _repo.EcuData.GetAllListAsync(x => x.ValidFlg == (int)EnumCommon.Status.Valid && existHistory.Select(y => y.HUSerial).Contains(x.HUCode));
                    if (existEcu.Any())
                    {
                        var entities = existEcu.Select(x => new EcuExported
                        {
                            EcuDataID = 0,
                            DateManufacture = x.DateManufacture,
                            NgCode = x.NgCode,
                            Note = x.Note,
                            Result = x.Result,
                            HUCode = x.HUCode,
                            LaserPrinting = x.LaserPrinting,
                            PackState = x.PackState,
                            ValidFlg = x.ValidFlg,
                            CreateTime = x.CreateTime,
                            CreateId = x.CreateId,
                            UpdateTime = x.UpdateTime,
                            UpdateId = x.UpdateId
                        });
                        await _repo.EcuExported.InsertAsync(entities);
                        await _repo.SaveAync();

                        await _repo.EcuData.DeleteAsync(existEcu);
                        await _repo.SaveAync();
                    }
                }
                else
                {
                    //Không có thì insert toàn bộ
                    /*var entities = lstData.Select(x => new ExportHistoryList
                    {
                        ExportHistoryID = 0,
                        BoxSerial = x.BoxSerial,
                        Compare = x.Result,
                        ExportPlanID = plan.ExportPlanID,
                        HUSerial = x.HUSerial,
                        Implementer = x.Implementer,
                        LaserEngraving = x.LaserEngraving,
                        OrderState = (int)EnumOrderState.State.Exported,
                        Quantity = x.Quantity,
                        TimeReadHU = x.TimeReadHU,
                        TimeRelease = DateTime.Now,
                        TimeReadQrCode = x.TimeReadQrCode,
                        ShiftWork = x.ShiftWork,
                        //WarehouseReleasePerson = _userPrincipalService.Username,
                        WarehouseReleasePerson = x.WarehouseReleasePerson,
                        ValidFlg = (int)EnumCommon.Status.Valid,
                        Exported = (int)EnumCommon.Status.Valid
                    });

                    await _repo.ExportHistoryList.InsertAsync(entities);*/

                    // Thùng chưa được quét sản phẩm và chưa nhập kho
                    return new ServiceResultError("Thông tin thùng hàng không hợp lệ!");
                }

                // Cập nhật lại trạng thái đơn
                plan.OrderState = (int)EnumOrderState.State.Exported;
                await _repo.ExportListPlan.UpdateAsync(plan);
                await _repo.SaveAync();

                // Cập nhật lại trạng thá thùng hàng
                foreach (var item in lstData)
                {
                    BoxInfo boxInfo = await _repo.BoxInfo.FirstOrDefaultAsync(x => x.BoxSerial == item.BoxSerial && x.ValidFlg == (int)EnumCommon.Status.Valid);

                    if (boxInfo is null) return new ServiceResultError("BOXHASNOTWAREHOUSED");  // Thùng hàng chưa được nhập kho!

                    boxInfo.ExportPlanID = plan.ExportPlanID;
                    //boxInfo.OrderNumber = item.OrderNumber;
                    boxInfo.OrderState = (int)EnumOrderState.State.Exported;
                    await _repo.BoxInfo.UpdateAsync(boxInfo);
                    await _repo.SaveAync();
                }

                return new ServiceResultSuccess("Thêm thông tin thành công!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return new ServiceResultError("Lỗi khi thêm thông tin thùng hàng: " + ex.Message);
            }
        }

        public async Task<ServiceResult> InsertScanBoxV2Result(List<ExportPlanScanBoxResult> lstData)
        {
            try
            {
                // Vui lòng kiểm tra lại dữ liệu
                if (lstData is null || lstData.Count == 0) return new ServiceResultError("Không có dữ liệu gửi lên!");

                // Các thùng phải thuộc cùng 1 đơn
                var orders = lstData.Select(x => x.OrderNumber).Distinct();
                if (orders.Count() != 1) return new ServiceResultError("Các thùng không cùng 1 đơn hàng");

                var orderNumber = orders.FirstOrDefault();

                // Đơn hàng phải chưa xuất
                var plan = await _repo.ExportListPlan.FirstOrDefaultAsync(x => x.OrderNumber.Equals(orderNumber) && x.ValidFlg == (int)EnumCommon.Status.Valid);

                if (plan is null) return new ServiceResultError("Đơn hàng không tồn tại!");
                if (plan.OrderState == (int)EnumOrderState.State.Exported) return new ServiceResultError("Đơn hàng đã được xuất kho!");
                if (plan.OrderState == (int)EnumOrderState.State.Cancel) return new ServiceResultError("Đơn hàng đã được hủy!");
                if (plan.OrderState != (int)EnumOrderState.State.NotExport) return new ServiceResultError("Đơn hàng không hợp lệ!");
                // TODO: check đơn hàng đã được lên đơn trong PC052 !?

                // Thùng hàng: đã đủ và chưa thuộc đơn hàng nào
                var boxs = new List<string>();
                foreach (var item in lstData)
                {
                    BoxInfo boxInfo = await _repo.BoxInfo.FirstOrDefaultAsync(x => x.BoxSerial == item.BoxSerial && x.ValidFlg == (int)EnumCommon.Status.Valid);
                    if (boxInfo is null)
                    {
                        boxs.Add(item.BoxSerial);
                        continue;
                        //return new ServiceResultError("BOXHASNOTWAREHOUSED"); // Thùng hàng chưa được nhập kho!
                    }
                    if (boxInfo.BoxState == (int)EnumBoxState.State.NotExport) return new ServiceResultError($"Thùng hàng chưa đủ số lượng trong hộp: {boxInfo.BoxSerial}");
                    if (boxInfo.BoxState == (int)EnumBoxState.State.Exported) return new ServiceResultError($"Thùng hàng đã được xuất kho: {boxInfo.BoxSerial}");
                    if (boxInfo.ExportPlanID != 0) return new ServiceResultError($"Thùng hàng \"{boxInfo.BoxSerial}\" đang thuộc đơn hàng: {boxInfo.ExportPlanID}");
                    if (boxInfo.ExportType == (int)EnumExportType.Type.ASSY)
                    {
                        if (!String.IsNullOrEmpty(plan.AssemblyProductCode))
                            if (!boxInfo.AssemblyProductCode!.Equals(plan.AssemblyProductCode)) return new ServiceResultError($"Đơn hàng và thùng hàng có mã số sản phẩm lắp cụm khác nhau: {plan.AssemblyProductCode} - {boxInfo.AssemblyProductCode}");
                    }
                    else
                    {
                        if (!boxInfo.DrawingCode!.Equals(plan.DrawingCode)) return new ServiceResultError($"Đơn hàng và thùng hàng có mã bản vẽ khác nhau: {plan.DrawingCode} - {boxInfo.DrawingCode}");
                        if (!boxInfo.ProductCode!.Equals(plan.ProductCode)) return new ServiceResultError($"Đơn hàng và thùng hàng có mã số sản phẩm khác nhau: {plan.ProductCode} - {boxInfo.ProductCode}");
                    }
                }
                if (boxs.Count > 0)
                {
                    return new ServiceResultError("BOXHASNOTWAREHOUSED", boxs);
                }

                // Kiểm tra số lượng và so với số lượng chỉ thị
                // Chỉ lấy những kết quả là OK
                var total = lstData.Where(x => x.Result.Equals(CommonConstant.OK)).Sum(x => x.Quantity);

                if (plan.IndicatorQuantity != total) return new ServiceResultError("Số lượng không khớp với đơn hàng! Số lượng chỉ thị: " + plan.IndicatorQuantity);

                // Kiểm tra các thùng đã có sẵn trong DB
                // Nếu có thì update lại ID đơn hàng
                var existHistory = await _repo.ExportHistoryList.GetAllListAsync(x => x.ValidFlg == (int)EnumCommon.Status.Valid && lstData.Select(y => y.BoxSerial).Contains(x.BoxSerial));

                if (existHistory.Any())
                {
                    string warehouseReleasePerson = lstData![0].WarehouseReleasePerson;

                    foreach (var item in existHistory)
                    {
                        item.ExportPlanID = plan.ExportPlanID;
                        item.OrderState = (int)EnumOrderState.State.Enough;
                        //Thêm thời điểm xuất kho và người xuất kho
                        item.TimeRelease = DateTime.Now;
                        //item.WarehouseReleasePerson = _userPrincipalService.Username;
                        //item.WarehouseReleasePerson = plan.WarehouseReleasePerson;
                        item.WarehouseReleasePerson = warehouseReleasePerson;
                    }

                    var notExist = lstData.Where(x => !existHistory.Select(y => y.BoxSerial).Contains(x.BoxSerial)).ToList();

                    if (notExist.Any())
                    {
                        var entities = notExist.Select(x => new ExportHistoryList
                        {
                            ExportHistoryID = 0,
                            BoxSerial = x.BoxSerial,
                            Compare = x.Result,
                            ExportPlanID = plan.ExportPlanID,
                            HUSerial = x.HUSerial,
                            Implementer = x.Implementer,
                            LaserEngraving = x.LaserEngraving,
                            OrderState = (int)EnumOrderState.State.Exported,
                            Quantity = x.Quantity,
                            TimeReadHU = x.TimeReadHU,
                            //TimeRelease = DateTime.Now,
                            TimeRelease = x.TimeRelease,
                            TimeReadQrCode = x.TimeReadQrCode,
                            ShiftWork = x.ShiftWork,
                            //WarehouseReleasePerson = _userPrincipalService.Username,
                            //WarehouseReleasePerson = x.WarehouseReleasePerson,
                            WarehouseReleasePerson = warehouseReleasePerson,
                            ValidFlg = (int)EnumCommon.Status.Valid,
                            Exported = (int)EnumCommon.Status.Valid,
                        });

                        await _repo.ExportHistoryList.InsertAsync(entities);
                    }

                    await _repo.ExportHistoryList.UpdateAsync(existHistory);

                    var existEcu = await _repo.EcuData.GetAllListAsync(x => x.ValidFlg == (int)EnumCommon.Status.Valid && existHistory.Select(y => y.HUSerial).Contains(x.HUCode));
                    if (existEcu.Any())
                    {
                        var entities = existEcu.Select(x => new EcuExported
                        {
                            EcuDataID = 0,
                            DateManufacture = x.DateManufacture,
                            NgCode = x.NgCode,
                            Note = x.Note,
                            Result = x.Result,
                            HUCode = x.HUCode,
                            LaserPrinting = x.LaserPrinting,
                            PackState = x.PackState,
                            ValidFlg = x.ValidFlg,
                            CreateTime = x.CreateTime,
                            CreateId = x.CreateId,
                            UpdateTime = x.UpdateTime,
                            UpdateId = x.UpdateId
                        });
                        await _repo.EcuExported.InsertAsync(entities);
                        await _repo.SaveAync();

                        await _repo.EcuData.DeleteAsync(existEcu);
                        await _repo.SaveAync();
                    }
                }
                else
                {
                    //Không có thì insert toàn bộ

                    // Thùng chưa được quét sản phẩm và chưa nhập kho
                    return new ServiceResultError("Thông tin thùng hàng không hợp lệ!");
                }

                // Cập nhật lại trạng thái đơn
                plan.OrderState = (int)EnumOrderState.State.Exported;
                await _repo.ExportListPlan.UpdateAsync(plan);
                await _repo.SaveAync();

                // Cập nhật lại trạng thá thùng hàng
                foreach (var item in lstData)
                {
                    BoxInfo boxInfo = await _repo.BoxInfo.FirstOrDefaultAsync(x => x.BoxSerial == item.BoxSerial && x.ValidFlg == (int)EnumCommon.Status.Valid);

                    if (boxInfo is null) return new ServiceResultError("BOXHASNOTWAREHOUSED");  // Thùng hàng chưa được nhập kho!

                    boxInfo.ExportPlanID = plan.ExportPlanID;
                    //boxInfo.OrderNumber = item.OrderNumber;
                    boxInfo.OrderState = (int)EnumOrderState.State.Exported;
                    await _repo.BoxInfo.UpdateAsync(boxInfo);
                    await _repo.SaveAync();
                }

                return new ServiceResultSuccess("Thêm thông tin thành công!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return new ServiceResultError("Lỗi khi thêm thông tin thùng hàng: " + ex.Message);
            }
        }

        public async Task<ServiceResult> AddScanBoxResult(List<ExportPlanScanBoxResult> lstData)
        {
            try
            {
                // Vui lòng kiểm tra lại dữ liệu
                if (lstData is null || lstData.Count == 0) return new ServiceResultError("Không có dữ liệu gửi lên!");

                // Các thùng phải thuộc cùng 1 đơn
                var orders = lstData.Select(x => x.OrderNumber).Distinct();
                if (orders.Count() != 1) return new ServiceResultError("Các thùng không cùng 1 đơn hàng");

                var orderNumber = orders.FirstOrDefault();

                // Đơn hàng phải chưa xuất
                var plan = await _repo.ExportListPlan.FirstOrDefaultAsync(x => x.OrderNumber.Equals(orderNumber) && x.ValidFlg == (int)EnumCommon.Status.Valid);

                if (plan is null) return new ServiceResultError("Đơn hàng không tồn tại!");
                if (plan.OrderState == (int)EnumOrderState.State.Exported) return new ServiceResultError("Đơn hàng đã được xuất kho!");
                if (plan.OrderState == (int)EnumOrderState.State.Cancel) return new ServiceResultError("Đơn hàng đã được hủy!");
                if (plan.OrderState != (int)EnumOrderState.State.NotExport) return new ServiceResultError("Đơn hàng không hợp lệ!");
                // TODO: check đơn hàng đã được lên đơn trong PC052 !?

                // Thùng hàng: đã đủ và chưa thuộc đơn hàng nào
                foreach (var item in lstData)
                {
                    BoxInfo boxInfo = await _repo.BoxInfo.FirstOrDefaultAsync(x => x.BoxSerial == item.BoxSerial && x.ValidFlg == (int)EnumCommon.Status.Valid);
                    if (boxInfo is null) return new ServiceResultError("BOXHASNOTWAREHOUSED"); // Thùng hàng chưa được nhập kho!
                    if (boxInfo.BoxState == (int)EnumBoxState.State.NotExport) return new ServiceResultError($"Thùng hàng chưa đủ số lượng trong hộp: {boxInfo.BoxSerial}");
                    if (boxInfo.BoxState == (int)EnumBoxState.State.Exported) return new ServiceResultError($"Thùng hàng đã được xuất kho: {boxInfo.BoxSerial}");
                    if (boxInfo.ExportPlanID != 0) return new ServiceResultError($"Thùng hàng \"{boxInfo.BoxSerial}\" đang thuộc đơn hàng: {boxInfo.ExportPlanID}");
                    if (boxInfo.ExportType == (int)EnumExportType.Type.ASSY)
                    {
                        if (!String.IsNullOrEmpty(plan.AssemblyProductCode))
                            if (!boxInfo.AssemblyProductCode!.Equals(plan.AssemblyProductCode)) return new ServiceResultError($"Đơn hàng và thùng hàng có mã số sản phẩm lắp cụm khác nhau: {plan.AssemblyProductCode} - {boxInfo.AssemblyProductCode}");
                    }
                    else if (!String.IsNullOrEmpty(boxInfo.DrawingCode))
                    {
                        if (!boxInfo.DrawingCode!.Equals(plan.DrawingCode)) return new ServiceResultError($"Đơn hàng và thùng hàng có mã bản vẽ khác nhau: {plan.DrawingCode} - {boxInfo.DrawingCode}");
                    }
                    else
                    {
                        if (!boxInfo.ProductCode!.Equals(plan.ProductCode)) return new ServiceResultError($"Đơn hàng và thùng hàng có mã số sản phẩm khác nhau: {plan.ProductCode} - {boxInfo.ProductCode}");
                    }
                }

                // Kiểm tra số lượng và so với số lượng chỉ thị
                // Chỉ lấy những kết quả là OK
                var total = lstData.Where(x => x.Result.Equals(CommonConstant.OK)).Sum(x => x.Quantity);

                if (plan.IndicatorQuantity != total) return new ServiceResultError("Số lượng không khớp với đơn hàng! Số lượng chỉ thị: " + plan.IndicatorQuantity);

                // Kiểm tra các thùng đã có sẵn trong DB
                // Nếu có thì update lại ID đơn hàng
                var existHistory = await _repo.ExportHistoryList.GetAllListAsync(x => x.ValidFlg == (int)EnumCommon.Status.Valid && lstData.Select(y => y.BoxSerial).Contains(x.BoxSerial));

                if (existHistory.Any())
                {
                    string warehouseReleasePerson = lstData![0].WarehouseReleasePerson;

                    foreach (var item in existHistory)
                    {
                        item.ExportPlanID = plan.ExportPlanID;
                        item.OrderState = (int)EnumOrderState.State.Enough;
                        //Thêm thời điểm xuất kho và người xuất kho
                        item.TimeRelease = DateTime.Now;
                        item.WarehouseReleasePerson = warehouseReleasePerson;
                    }

                    var notExist = lstData.Where(x => !existHistory.Select(y => y.BoxSerial).Contains(x.BoxSerial)).ToList();

                    if (notExist.Any())
                    {
                        var entities = notExist.Select(x => new ExportHistoryList
                        {
                            ExportHistoryID = 0,
                            BoxSerial = x.BoxSerial,
                            Compare = x.Result,
                            ExportPlanID = plan.ExportPlanID,
                            HUSerial = x.HUSerial,
                            Implementer = x.Implementer,
                            LaserEngraving = x.LaserEngraving,
                            OrderState = (int)EnumOrderState.State.Exported,
                            Quantity = x.Quantity,
                            TimeReadHU = x.TimeReadHU,
                            TimeRelease = x.TimeRelease,
                            TimeReadQrCode = x.TimeReadQrCode,
                            ShiftWork = x.ShiftWork,
                            WarehouseReleasePerson = warehouseReleasePerson,
                            ValidFlg = (int)EnumCommon.Status.Valid,
                            Exported = (int)EnumCommon.Status.Valid,
                        });

                        await _repo.ExportHistoryList.InsertAsync(entities);
                    }

                    await _repo.ExportHistoryList.UpdateAsync(existHistory);

                    var existEcu = await _repo.EcuData.GetAllListAsync(x => x.ValidFlg == (int)EnumCommon.Status.Valid && existHistory.Select(y => y.HUSerial).Contains(x.HUCode));
                    if (existEcu.Any())
                    {
                        var entities = existEcu.Select(x => new EcuExported
                        {
                            EcuDataID = 0,
                            DateManufacture = x.DateManufacture,
                            NgCode = x.NgCode,
                            Note = x.Note,
                            Result = x.Result,
                            HUCode = x.HUCode,
                            LaserPrinting = x.LaserPrinting,
                            PackState = x.PackState,
                            ValidFlg = x.ValidFlg,
                            CreateTime = x.CreateTime,
                            CreateId = x.CreateId,
                            UpdateTime = x.UpdateTime,
                            UpdateId = x.UpdateId
                        });
                        await _repo.EcuExported.InsertAsync(entities);
                        await _repo.SaveAync();

                        await _repo.EcuData.DeleteAsync(existEcu);
                        await _repo.SaveAync();
                    }
                }
                else
                {
                    //Không có thì insert toàn bộ

                    // Thùng chưa được quét sản phẩm và chưa nhập kho
                    return new ServiceResultError("Thông tin thùng hàng không hợp lệ!");
                }

                // Cập nhật lại trạng thái đơn
                plan.OrderState = (int)EnumOrderState.State.Exported;
                await _repo.ExportListPlan.UpdateAsync(plan);
                await _repo.SaveAync();

                // Cập nhật lại trạng thá thùng hàng
                foreach (var item in lstData)
                {
                    BoxInfo boxInfo = await _repo.BoxInfo.FirstOrDefaultAsync(x => x.BoxSerial == item.BoxSerial && x.ValidFlg == (int)EnumCommon.Status.Valid);

                    if (boxInfo is null) return new ServiceResultError("BOXHASNOTWAREHOUSED");  // Thùng hàng chưa được nhập kho!

                    boxInfo.ExportPlanID = plan.ExportPlanID;
                    //boxInfo.OrderNumber = item.OrderNumber;
                    boxInfo.OrderState = (int)EnumOrderState.State.Exported;
                    await _repo.BoxInfo.UpdateAsync(boxInfo);
                    await _repo.SaveAync();
                }

                return new ServiceResultSuccess("Thêm thông tin thành công!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return new ServiceResultError("Lỗi khi thêm thông tin thùng hàng: " + ex.Message);
            }
        }

        public async Task<ServiceResult> AddScanBoxV2Result(List<ExportPlanScanBoxResult> lstData)
        {
            try
            {
                // Vui lòng kiểm tra lại dữ liệu
                if (lstData is null || lstData.Count == 0) return new ServiceResultError("Không có dữ liệu gửi lên!");

                // Các thùng phải thuộc cùng 1 đơn
                var orders = lstData.Select(x => x.OrderNumber).Distinct();
                if (orders.Count() != 1) return new ServiceResultError("Các thùng không cùng 1 đơn hàng");

                var orderNumber = orders.FirstOrDefault();

                // Đơn hàng phải chưa xuất
                var plan = await _repo.ExportListPlan.FirstOrDefaultAsync(x => x.OrderNumber.Equals(orderNumber) && x.ValidFlg == (int)EnumCommon.Status.Valid);

                if (plan is null) return new ServiceResultError("Đơn hàng không tồn tại!");
                if (plan.OrderState == (int)EnumOrderState.State.Exported) return new ServiceResultError("Đơn hàng đã được xuất kho!");
                if (plan.OrderState == (int)EnumOrderState.State.Cancel) return new ServiceResultError("Đơn hàng đã được hủy!");
                if (plan.OrderState != (int)EnumOrderState.State.NotExport) return new ServiceResultError("Đơn hàng không hợp lệ!");
                // TODO: check đơn hàng đã được lên đơn trong PC052 !?

                // Thùng hàng: đã đủ và chưa thuộc đơn hàng nào
                var boxs = new List<string>();
                foreach (var item in lstData)
                {
                    BoxInfo boxInfo = await _repo.BoxInfo.FirstOrDefaultAsync(x => x.BoxSerial == item.BoxSerial && x.ValidFlg == (int)EnumCommon.Status.Valid);
                    if (boxInfo is null)
                    {
                        boxs.Add(item.BoxSerial);
                        continue;
                        //return new ServiceResultError("BOXHASNOTWAREHOUSED"); // Thùng hàng chưa được nhập kho!
                    }
                    if (boxInfo.BoxState == (int)EnumBoxState.State.NotExport) return new ServiceResultError($"Thùng hàng chưa đủ số lượng trong hộp: {boxInfo.BoxSerial}");
                    if (boxInfo.BoxState == (int)EnumBoxState.State.Exported) return new ServiceResultError($"Thùng hàng đã được xuất kho: {boxInfo.BoxSerial}");
                    if (boxInfo.ExportPlanID != 0) return new ServiceResultError($"Thùng hàng \"{boxInfo.BoxSerial}\" đang thuộc đơn hàng: {boxInfo.ExportPlanID}");
                    if (boxInfo.ExportType == (int)EnumExportType.Type.ASSY)
                    {
                        if (!String.IsNullOrEmpty(plan.AssemblyProductCode))
                            if (!boxInfo.AssemblyProductCode!.Equals(plan.AssemblyProductCode)) return new ServiceResultError($"Đơn hàng và thùng hàng có mã số sản phẩm lắp cụm khác nhau: {plan.AssemblyProductCode} - {boxInfo.AssemblyProductCode}");
                    }
                    else if (!String.IsNullOrEmpty(boxInfo.DrawingCode))
                    {
                        if (!boxInfo.DrawingCode!.Equals(plan.DrawingCode)) return new ServiceResultError($"Đơn hàng và thùng hàng có mã bản vẽ khác nhau: {plan.DrawingCode} - {boxInfo.DrawingCode}");
                    }
                    else
                    {
                        if (!boxInfo.ProductCode!.Equals(plan.ProductCode)) return new ServiceResultError($"Đơn hàng và thùng hàng có mã số sản phẩm khác nhau: {plan.ProductCode} - {boxInfo.ProductCode}");
                    }
                }
                if (boxs.Count > 0)
                {
                    return new ServiceResultError("BOXHASNOTWAREHOUSED", boxs);
                }

                // Kiểm tra số lượng và so với số lượng chỉ thị
                // Chỉ lấy những kết quả là OK
                var total = lstData.Where(x => x.Result.Equals(CommonConstant.OK)).Sum(x => x.Quantity);

                if (plan.IndicatorQuantity != total) return new ServiceResultError("Số lượng không khớp với đơn hàng! Số lượng chỉ thị: " + plan.IndicatorQuantity);

                // Kiểm tra các thùng đã có sẵn trong DB
                // Nếu có thì update lại ID đơn hàng
                var existHistory = await _repo.ExportHistoryList.GetAllListAsync(x => x.ValidFlg == (int)EnumCommon.Status.Valid && lstData.Select(y => y.BoxSerial).Contains(x.BoxSerial));

                if (existHistory.Any())
                {
                    string warehouseReleasePerson = lstData![0].WarehouseReleasePerson;

                    foreach (var item in existHistory)
                    {
                        item.ExportPlanID = plan.ExportPlanID;
                        item.OrderState = (int)EnumOrderState.State.Enough;
                        //Thêm thời điểm xuất kho và người xuất kho
                        item.TimeRelease = DateTime.Now;
                        item.WarehouseReleasePerson = warehouseReleasePerson;
                    }

                    var notExist = lstData.Where(x => !existHistory.Select(y => y.BoxSerial).Contains(x.BoxSerial)).ToList();

                    if (notExist.Any())
                    {
                        var entities = notExist.Select(x => new ExportHistoryList
                        {
                            ExportHistoryID = 0,
                            BoxSerial = x.BoxSerial,
                            Compare = x.Result,
                            ExportPlanID = plan.ExportPlanID,
                            HUSerial = x.HUSerial,
                            Implementer = x.Implementer,
                            LaserEngraving = x.LaserEngraving,
                            OrderState = (int)EnumOrderState.State.Exported,
                            Quantity = x.Quantity,
                            TimeReadHU = x.TimeReadHU,
                            TimeRelease = x.TimeRelease,
                            TimeReadQrCode = x.TimeReadQrCode,
                            ShiftWork = x.ShiftWork,
                            WarehouseReleasePerson = warehouseReleasePerson,
                            ValidFlg = (int)EnumCommon.Status.Valid,
                            Exported = (int)EnumCommon.Status.Valid,
                        });

                        await _repo.ExportHistoryList.InsertAsync(entities);
                    }

                    await _repo.ExportHistoryList.UpdateAsync(existHistory);

                    var existEcu = await _repo.EcuData.GetAllListAsync(x => x.ValidFlg == (int)EnumCommon.Status.Valid && existHistory.Select(y => y.HUSerial).Contains(x.HUCode));
                    if (existEcu.Any())
                    {
                        var entities = existEcu.Select(x => new EcuExported
                        {
                            EcuDataID = 0,
                            DateManufacture = x.DateManufacture,
                            NgCode = x.NgCode,
                            Note = x.Note,
                            Result = x.Result,
                            HUCode = x.HUCode,
                            LaserPrinting = x.LaserPrinting,
                            PackState = x.PackState,
                            ValidFlg = x.ValidFlg,
                            CreateTime = x.CreateTime,
                            CreateId = x.CreateId,
                            UpdateTime = x.UpdateTime,
                            UpdateId = x.UpdateId
                        });
                        await _repo.EcuExported.InsertAsync(entities);
                        await _repo.SaveAync();

                        await _repo.EcuData.DeleteAsync(existEcu);
                        await _repo.SaveAync();
                    }
                }
                else
                {
                    //Không có thì insert toàn bộ

                    // Thùng chưa được quét sản phẩm và chưa nhập kho
                    return new ServiceResultError("Thông tin thùng hàng không hợp lệ!");
                }

                // Cập nhật lại trạng thái đơn
                plan.OrderState = (int)EnumOrderState.State.Exported;
                await _repo.ExportListPlan.UpdateAsync(plan);
                await _repo.SaveAync();

                // Cập nhật lại trạng thá thùng hàng
                foreach (var item in lstData)
                {
                    BoxInfo boxInfo = await _repo.BoxInfo.FirstOrDefaultAsync(x => x.BoxSerial == item.BoxSerial && x.ValidFlg == (int)EnumCommon.Status.Valid);

                    if (boxInfo is null) return new ServiceResultError("BOXHASNOTWAREHOUSED");  // Thùng hàng chưa được nhập kho!

                    boxInfo.ExportPlanID = plan.ExportPlanID;
                    //boxInfo.OrderNumber = item.OrderNumber;
                    boxInfo.OrderState = (int)EnumOrderState.State.Exported;
                    await _repo.BoxInfo.UpdateAsync(boxInfo);
                    await _repo.SaveAync();
                }

                return new ServiceResultSuccess("Thêm thông tin thành công!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return new ServiceResultError("Lỗi khi thêm thông tin thùng hàng: " + ex.Message);
            }
        }

        #endregion

        #region Update
        public async Task<ServiceResult> UpdatetHistoryPlan(ExportHistoryListDto dto)
        {
            try
            {
                var plan = await _repo.ExportListPlan.GetAsync(dto.ExportPlanID);

                if (plan is null) return new ServiceResultError("Kế hoạch xuất kho không tồn tại");

                if (plan.ValidFlg != (int)EnumCommon.Status.Valid) return new ServiceResultError("Kế hoạch xuất kho không hợp lệ");

                var entity = await _repo.ExportHistoryList.GetAsync(dto.ExportHistoryID);

                if (entity is null) return new ServiceResultError("Lịch sử xuất kho không tồn tại");

                if (entity.ValidFlg != (int)EnumCommon.Status.Valid) return new ServiceResultError("Lịch sử xuất kho không hợp lệ");

                _mapper.Map(dto, entity);
                await _repo.ExportHistoryList.UpdateAsync(entity);
                await _repo.SaveAync();
                return new ServiceResultSuccess("Thêm lịch sử xuất kho thành công!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return new ServiceResultError("Lỗi khi thêm lịch sử xuất kho: " + ex.Message);
            }
        }
        public async Task<ServiceResult> UpdateExportPlan(ExportListPlanDto data)
        {
            try
            {
                var entity = await _repo.ExportListPlan.GetAsync(data.ExportPlanID);

                //Check exist with ID
                if (entity is null)
                    return new ServiceResultError($"Kế hoạch không tồn tại!");

                if (entity.ValidFlg != (int)EnumCommon.Status.Valid)
                    return new ServiceResultError($"Kế hoạch không hợp lệ!");
                //Tự động lấy mã cũ nếu ko có mã đơn hàng
                if (string.IsNullOrEmpty(data.OrderNumber) || string.IsNullOrEmpty(data.OrderNumber.Trim()))
                    data.OrderNumber = entity.OrderNumber;

                var exist = await _repo.ExportListPlan.AnyAsync(x => x.ValidFlg == (int)EnumCommon.Status.Valid && x.OrderNumber.ToLower().Equals(data.OrderNumber.ToLower()) && x.ExportPlanID != data.ExportPlanID);

                //Plan with ordernumber is unique
                if (exist)
                    return new ServiceResultError($"Kế hoạch với mã đơn hàng {data.OrderNumber} đã tồn tại!", new { field = nameof(ExportListPlanDto.OrderNumber) });

                //Check xem master data có tồn tại đơn hàng không
                //var existMstData = await _repo.MstData.AnyAsync(x => x.ValidFlg == (int)EnumCommon.Status.Valid && x.Customer.ToLower() == data.CustomerCode.ToLower() && x.ProductCode.ToLower() == data.ProductCode.ToLower());
                //int existMstData;
                var existMstData = new List<MstData>();
                if (!String.IsNullOrEmpty(data.AssemblyProductCode) && data.ExportType == (int)EnumExportType.Type.ASSY)
                {
                    existMstData = await _repo.MstData.GetAllListAsync(x =>
                        x.ValidFlg == (int)EnumCommon.Status.Valid &&
                        x.Customer.ToLower() == data.CustomerCode.ToLower() &&
                        x.InternalDrawingCode.ToLower() == data.DrawingCode.ToLower() &&
                        x.AssemblyProductCode.ToLower() == data.AssemblyProductCode.ToLower() &&
                        x.ProductCode.ToLower() == data.ProductCode.ToLower()
                    );
                }
                else
                {
                    existMstData = await _repo.MstData.GetAllListAsync(x =>
                        x.ValidFlg == (int)EnumCommon.Status.Valid &&
                        x.Customer.ToLower() == data.CustomerCode.ToLower() &&
                        x.InternalDrawingCode.ToLower() == data.DrawingCode.ToLower() &&
                        x.ProductCode.ToLower() == data.ProductCode.ToLower()
                    );
                    if (existMstData.Count > 1)
                    {   // #8834
                        var ms = new List<MstData>();
                        if (data.ExportType == 0)
                        {
                            ms = existMstData.Where(e => e.ExportLooseDomestic == 1).ToList();
                        }
                        else if (data.ExportType == 1)
                        {
                            ms = existMstData.Where(e => e.ExportLooseInternational == 1).ToList();
                        }
                        if (ms.Count == 1)
                        {
                            existMstData = ms;
                        }
                    }
                }

                if (existMstData.Count == 0)
                {
                    return new ServiceResultError($"Sản phẩm không tồn tại ở master data", new { field = nameof(ExportListPlanDto.ProductCode) });
                }
                else if (existMstData.Count > 1)
                {
                    return new ServiceResultError(String.Format(CommonConstant.PLAN_DUPLICATE_MASTER, existMstData.Count));
                }
                data.ProductID = existMstData[0].ProductID;

                // #8763 Cho phép thực hiện quét lại thùng hàng của đơn hàng đã hủy
                if (data.OrderState == (int)EnumOrderState.State.Cancel)
                {
                    // Cho phép thực hiện quét lại sản phẩm của đơn hàng đã hủy
                    /*var existHistory = await _repo.ExportHistoryList.GetAllListAsync(x => x.ExportPlanID == data.ExportPlanID);
                    if (existHistory != null && existHistory.Count > 0)
                    {
                        List<string> list = existHistory.Select(h => h.HUSerial).ToList();
                        if (list.Count > 0)
                        {
                            var existEcu = await _repo.EcuData.GetAllListAsync(x => list.Contains(x.HUCode));

                            foreach (var item in existEcu)
                            {
                                item.PackState = (int)EnumPackState.State.NotPacked;
                            }
                            await _repo.EcuData.UpdateAsync(existEcu);
                            await _repo.SaveAync();
                        }
                    }*/

                    // Cho phép thực hiện quét lại thùng hàng của đơn hàng đã hủy
                    var existBoxs = await _repo.BoxInfo.GetAllListAsync(x => x.ExportPlanID == data.ExportPlanID);
                    if (existBoxs != null && existBoxs.Count > 0)
                    {
                        foreach (var item in existBoxs)
                        {
                            item.ExportPlanID = 0;
                            item.OrderState = (int)EnumOrderState.State.Enough;
                        }
                        await _repo.BoxInfo.UpdateAsync(existBoxs);
                        await _repo.SaveAync();
                    }
                }

                //Mapping data
                _mapper.Map(data, entity);
                entity.ValidFlg = (int)EnumCommon.Status.Valid;
                await _repo.ExportListPlan.UpdateAsync(entity);
                await _repo.SaveAync();
                return new ServiceResultSuccess("Cập nhật kế hoạch thành công!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return new ServiceResultError("Lỗi khi cập nhật kế hoạch: " + ex.Message);
            }
        }
        public async Task<ServiceResult> UpdateListBillState(List<UpdateStatus> lstData)
        {
            try
            {
                //Chỉ lấy các đơn hàng ở trạng thái chưa xuất
                var entities = await _repo.ExportListPlan.GetAllListAsync(x => x.ValidFlg == (int)EnumCommon.Status.Valid && lstData.Select(y => y.ID).Contains(x.ExportPlanID) && (x.OrderState == (int)EnumOrderState.State.NotExport));

                if (entities is null || entities.Count == 0) return new ServiceResultError("Kế hoạch không tồn tại");

                var dictData = lstData.GroupBy(x => x.ID).ToDictionary(x => x.Key, x => x.FirstOrDefault().Status);

                //Check có tồn tại trong master data ko
                var dictMiss = Enumerable.Range(0, 0).Select(e => new { orderNumber = "", customerCode = "", drawingCode = "", assemblyProductCode = "", productCode = "" }).ToList();
                var dictDuplicate = Enumerable.Range(0, 0).Select(e => new { orderNumber = "", customerCode = "", drawingCode = "", assemblyProductCode = "", productCode = "" }).ToList();
                var dictInvalid = Enumerable.Range(0, 0).Select(e => new { orderNumber = "", customerCode = "", drawingCode = "", assemblyProductCode = "", productCode = "" }).ToList();
                var dictError = Enumerable.Range(0, 0).Select(e => new { orderNumber = "", customerCode = "", drawingCode = "", assemblyProductCode = "", productCode = "" }).ToList();

                foreach (var entity in entities)
                {
                    var existMstData = new List<MstData>();
                    if (!String.IsNullOrEmpty(entity.AssemblyProductCode) && entity.ExportType == (int)EnumExportType.Type.ASSY)
                    {   // CountAsync
                        existMstData = await _repo.MstData.GetAllListAsync(x =>
                            x.ValidFlg == (int)EnumCommon.Status.Valid &&
                            x.Customer.ToLower() == entity.CustomerCode.ToLower() &&
                            x.InternalDrawingCode.ToLower() == entity.DrawingCode.ToLower() &&
                            x.AssemblyProductCode.ToLower() == entity.AssemblyProductCode.ToLower() &&
                            x.ProductCode.ToLower() == entity.ProductCode.ToLower()
                        );
                    }
                    else if (entity.ExportType == (int)EnumExportType.Type.LooseDomestic)
                    {
                        existMstData = await _repo.MstData.GetAllListAsync(
                            x => x.ValidFlg == (int)EnumCommon.Status.Valid &&
                            x.Customer.ToLower() == entity.CustomerCode.ToLower() &&
                            x.InternalDrawingCode.ToLower() == entity.DrawingCode.ToLower() &&
                            x.ExportLooseDomestic == 1 &&
                            x.ProductCode.ToLower() == entity.ProductCode.ToLower()
                        );
                    }
                    else if (entity.ExportType == (int)EnumExportType.Type.LooseInternational)
                    {
                        existMstData = await _repo.MstData.GetAllListAsync(
                            x => x.ValidFlg == (int)EnumCommon.Status.Valid &&
                            x.Customer.ToLower() == entity.CustomerCode.ToLower() &&
                            x.InternalDrawingCode.ToLower() == entity.DrawingCode.ToLower() &&
                            x.ExportLooseInternational == 1 &&
                            x.ProductCode.ToLower() == entity.ProductCode.ToLower()
                        );
                    }
                    else
                    {
                        existMstData = await _repo.MstData.GetAllListAsync(
                            x => x.ValidFlg == (int)EnumCommon.Status.Valid &&
                            x.Customer.ToLower() == entity.CustomerCode.ToLower() &&
                            x.InternalDrawingCode.ToLower() == entity.DrawingCode.ToLower() &&
                            x.ProductCode.ToLower() == entity.ProductCode.ToLower()
                        );
                    }

                    if (existMstData.Count() == 0)
                    {
                        dictMiss.Add(new { orderNumber = entity.OrderNumber, customerCode = entity.CustomerCode, drawingCode = entity.DrawingCode, assemblyProductCode = entity.AssemblyProductCode, productCode = entity.ProductCode });
                        dictError.Add(new { orderNumber = entity.OrderNumber, customerCode = entity.CustomerCode, drawingCode = entity.DrawingCode, assemblyProductCode = entity.AssemblyProductCode, productCode = entity.ProductCode });
                        continue;
                    }
                    else if (existMstData.Count() > 1)
                    {
                        dictDuplicate.Add(new { orderNumber = entity.OrderNumber, customerCode = entity.CustomerCode, drawingCode = entity.DrawingCode, assemblyProductCode = entity.AssemblyProductCode, productCode = entity.ProductCode });
                        dictError.Add(new { orderNumber = entity.OrderNumber, customerCode = entity.CustomerCode, drawingCode = entity.DrawingCode, assemblyProductCode = entity.AssemblyProductCode, productCode = entity.ProductCode });
                        continue;
                    }
                    else if (existMstData[0].ReferenceType < 0)
                    {
                        dictInvalid.Add(new { orderNumber = entity.OrderNumber, customerCode = entity.CustomerCode, drawingCode = entity.DrawingCode, assemblyProductCode = entity.AssemblyProductCode, productCode = entity.ProductCode });
                        dictError.Add(new { orderNumber = entity.OrderNumber, customerCode = entity.CustomerCode, drawingCode = entity.DrawingCode, assemblyProductCode = entity.AssemblyProductCode, productCode = entity.ProductCode });
                        continue;
                    }

                    if (dictData.ContainsKey(entity.ExportPlanID))
                    {
                        entity.BillState = dictData.GetValueOrDefault(entity.ExportPlanID);
                    }
                }

                if (dictMiss.Count > 0 || dictDuplicate.Count > 0)
                {
                    string message = "";
                    if (dictMiss.Count == 0)
                    {
                        message = "Đơn hàng có thông tin sản phẩm trùng khớp với nhiều master data";
                        return new ServiceResultNG(message, dictDuplicate);
                    }
                    else if (dictDuplicate.Count == 0)
                    {
                        message = "Đơn hàng có thông tin sản phẩm không tồn tại ở master data";
                        return new ServiceResultNG(message, dictMiss);
                    }
                    else
                    {
                        message = "Đơn hàng có thông tin sản phẩm không tồn tại hoặc trùng khớp với nhiều master data";
                        return new ServiceResultNG(message, dictError);
                    }
                }
                else if (dictInvalid.Count > 0)
                {
                    string message = "Đơn hàng có thông tin sản phẩm ở master data thiếu thông tin tham chiếu";
                    return new ServiceResultNG(message, dictInvalid);
                }

                await _repo.ExportListPlan.UpdateAsync(entities);
                await _repo.SaveAync();

                return new ServiceResultSuccess("Cập nhật danh sách thành công!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return new ServiceResultError("Lỗi khi cập nhật trạng thái lên đơn: " + ex.Message);
            }
        }

        /// <summary>
        /// Update trạng thái cho màn hình PC0081
        /// </summary>
        /// <param name="lstData"></param>
        /// <returns></returns>
        public async Task<ServiceResult> UpdatePrepareProductOrderState(List<UpdateStatus> lstData)
        {
            try
            {
                //Chỉ lấy các đơn hàng ở trạng thái Đã đủ hoặc Đang chuẩn bị
                //var entities = await _repo.ExportListPlan.GetAllListAsync(x => x.ValidFlg == (int)EnumCommon.Status.Valid && lstData.Select(y => y.ID).Contains(x.ExportPlanID) && (x.OrderState == (int)EnumOrderState.State.Processing || x.OrderState == (int)EnumOrderState.State.Enough));

                //if (entities is null || entities.Count == 0) return new ServiceResultError("Đơn hàng không tồn tại");

                if (lstData.Count() == 0)
                    return new ServiceResultError("Không có thông tin của thùng hàng");

                var entities = await _repo.ExportHistoryList.GetAllListAsync(x =>
                    x.ValidFlg == (int)EnumCommon.Status.Valid &&
                    x.BoxSerial == lstData[0].BoxSerial
                );

                if (entities is null || entities.Count == 0) return new ServiceResultError("Thùng hàng không tồn tại");

                var dictData = lstData.GroupBy(x => x.ID).ToDictionary(x => x.Key, x => x.FirstOrDefault().Status);

                foreach (var entity in entities)
                {
                    if (dictData.ContainsKey(entity.ExportPlanID))
                    {   // Fixbug #8195
                        //entity.BillState = dictData.GetValueOrDefault(entity.ExportPlanID);
                        entity.OrderState = dictData.GetValueOrDefault(entity.ExportPlanID);
                    }
                }

                //await _repo.ExportListPlan.UpdateAsync(entities);
                await _repo.ExportHistoryList.UpdateAsync(entities);
                await _repo.SaveAync();

                //return new ServiceResultSuccess("Cập nhật danh sách thành công!");
                return new ServiceResultSuccess("Cập nhật trạng thái thùng hàng thành công!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                //return new ServiceResultError("Lỗi khi cập nhật trạng thái lên đơn: " + ex.Message);
                return new ServiceResultError("Lỗi khi cập nhật trạng thái thùng hàng: " + ex.Message);
            }
        }

        /// <summary>
        /// Update trạng thái cho màn hình PC0081
        /// </summary>
        /// <param name="lstData"></param>
        /// <returns></returns>
        public async Task<ServiceResult> UpdatePrepareBoxInfoOrderState(List<UpdateStatus> lstData)
        {
            try
            {
                if (lstData.Count() == 0)
                    return new ServiceResultError("Không có thông tin của thùng hàng");

                var box = await _repo.BoxInfo.GetAsync(lstData[0].BoxID);
                if (box is null) return new ServiceResultError("Thùng hàng không tồn tại");

                // #8743 - 1: Đang chuẩn bị, 2: Đã đủ
                box.BoxState = lstData[0].Status;

                await _repo.BoxInfo.UpdateAsync(box);
                await _repo.SaveAync();

                return new ServiceResultSuccess("Cập nhật trạng thái thùng hàng thành công!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                //return new ServiceResultError("Lỗi khi cập nhật trạng thái lên đơn: " + ex.Message);
                return new ServiceResultError("Lỗi khi cập nhật trạng thái thùng hàng: " + ex.Message);
            }
        }

        #endregion

        #region Delete

        public async Task<ServiceResult> DeleteExportPlan(int ExportPlanID)
        {
            try
            {
                var data = await _repo.ExportListPlan.GetAsync(ExportPlanID);
                if (data is null) return new ServiceResultError("Kế hoạch không tồn tại!");

                if (data.ValidFlg == (int)EnumCommon.Status.Invalid)
                    if (data is null) return new ServiceResultError("Kế hoạch đã không hợp lệ!");

                //Todo: Other business

                data.ValidFlg = (int)EnumCommon.Status.Invalid;
                await _repo.ExportListPlan.UpdateAsync(data);
                await _repo.SaveAync();
                return new ServiceResultSuccess("Xóa kế hoạch thành công!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return new ServiceResultError("Lỗi khi xóa kế hoạch: " + ex.Message);
            }
        }

        /// <summary>
        /// Xóa vật lý thùng hàng trong bảng box_info và export_history_list
        /// Thông tin PackState trong ecu_data sẽ là 0 (uncheck)
        /// </summary>
        /// <param name="ExportPlanID"></param>
        /// <returns></returns>
        public async Task<ServiceResult> DeleteBoxInfo(int BoxID)
        {
            try
            {
                var box = await _repo.BoxInfo.GetAsync(BoxID);

                var existHistory = await _repo.ExportHistoryList.GetAllListAsync(x => x.BoxSerial == box.BoxSerial);

                if (existHistory != null && existHistory.Count > 0)
                {
                    List<string> list = existHistory.Select(h => h.HUSerial).ToList();
                    if (list.Count > 0)
                    {
                        var existEcu = await _repo.EcuData.GetAllListAsync(x => list.Contains(x.HUCode));

                        foreach (var item in existEcu)
                        {
                            item.PackState = (int)EnumPackState.State.NotPacked;
                        }
                        await _repo.EcuData.UpdateAsync(existEcu);
                        await _repo.SaveAync();
                    }
                }

                await _repo.ExportHistoryList.DeleteAsync(existHistory);
                await _repo.SaveAync();

                await _repo.BoxInfo.DeleteAsync(box);
                await _repo.SaveAync();

                return new ServiceResultSuccess("Xóa thùng hàng thành công!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return new ServiceResultError("Lỗi khi xóa thùng hàng: " + ex.Message);
            }
        }

        #endregion

        #endregion

        #region Others
        #endregion
    }
}
