using Core.Application.CustomModels.Pagging;

namespace Core.Application.CustomModels.SearchConditions
{
    /// <summary>
    /// Tìm đơn hàng đã đủ
    /// </summary>
    public class ExportPlanPreSearchImpl : PaggingImpl
    {
        public string OrderNumber { get; set; } = string.Empty;// OrderNumber
        public string FromDate { get; set; } = string.Empty;//Delivery allocate from date
        public string ToDate { get; set; } = string.Empty;//Delivery allocate tp date
        public string CustomerCode { get; set; } = string.Empty;//Customer code
        public int ExportType { get; set; }//EnumExportType.Type
        public int BillState {  get; set; }//EnumCommon. 0 là chưa lên đơn. 1 là đã lên đơn
    }
}