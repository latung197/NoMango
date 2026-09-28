using Core.Application.Constants;
using Core.Application.CustomModels;
using Core.Application.CustomModels.Dtos;
using Core.Application.CustomModels.Others;
using Core.Application.CustomModels.SearchConditions;
using Core.Application.Enum;
using Core.Application.Interface;
using Core.Domain.Entity;
using Core.Domain.Interface;
using Core.Application.Security;
using Core.Utils;
using Core.Utils.LogUtils;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;


namespace Core.Application.Services
{
    public partial class ExportPlanServiceImpl
    {
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
                           select _mapper.ToExportHistoryListDto(history);

            var lstData = await iqResult.ToListAsync();


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
                           select _mapper.ToExportHistoryListDto(history);

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
                           select _mapper.ToExportHistoryListDto(history);

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
                var dto = _mapper.ToExportListPlanDto(plan);

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

                var data = lstData.Count == 0 ? null : _mapper.ToMstDataDto(lstData[0]);
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
    }
}
