using Core.Application.CustomModels.Pagging;

namespace Core.Application.CustomModels.SearchConditions
{
    /// <summary>
    /// Tìm hiếm tất cả đơn hàng
    /// </summary>
    public class ExportPlanSearchImpl : PaggingImpl
    {
        public string OrderNumber { get; set; } = string.Empty;// OrderNumber
        public string FromDate { get; set; } = string.Empty;//Delivery allocate from date
        public string ToDate { get; set; } = string.Empty;//Delivery allocate tp date
        public string CustomerCode { get; set; } = string.Empty;//Customer code
        public int OrderState { get; set; }//EnumOrderState.State
        public int ExportType { get; set; }//EnumExportType.Type
    }
}