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

                var entities = lstData.Select(_mapper.ToExportHistoryList).ToList();

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

                _mapper.UpdateExportHistoryList(dto, entity);
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
                _mapper.UpdateExportListPlan(data, entity);
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
    }
}
