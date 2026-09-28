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
    }
}
