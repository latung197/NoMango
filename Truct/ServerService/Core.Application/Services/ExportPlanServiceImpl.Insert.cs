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
        #region Insert
        public async Task<ServiceResult> InsertHistoryPlan(ExportHistoryListDto dto)
        {
            try
            {
                var plan = await _repo.ExportListPlan.GetAsync(dto.ExportPlanID);

                if (plan is null) return new ServiceResultError("Kế hoạch xuất kho không tồn tại");

                if (plan.ValidFlg != (int)EnumCommon.Status.Valid) return new ServiceResultError("Kế hoạch xuất kho không hợp lệ");

                var entity = _mapper.ToExportHistoryList(dto);
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
                    entityNew = newPlans.Select(_mapper.ToExportListPlan).ToList();
                    await _repo.ExportListPlan.InsertAsync(entityNew);
                    await _repo.SaveAync();
                }

                if (existPlans.Count > 0)
                {
                    // Update kế hoạch cũ
                    //await _repo.ExportListPlan.UpdateAsync(existPlans);

                    // Xóa kế hoạch cũ, và Insert kế hoạch trùng
                    //entityExist = existPlans.Select(_mapper.ToExportListPlan).ToList();
                    var entities = existPlans.Select(_mapper.ToExportListPlan).ToList();
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
                                    _mapper.UpdateExportListPlan(data, plan);
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
                        var entities = notExistData.Select(_mapper.ToExportListPlan).ToList();
                        await _repo.ExportListPlan.InsertAsync(entities);
                        await _repo.SaveAync();
                    }
                }
                else
                {
                    //Không tồn tại thì insert như bình thường
                    var entities = condition.Data.Select(_mapper.ToExportListPlan).ToList();
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
                var entity = _mapper.ToExportListPlan(data);
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
    }
}
