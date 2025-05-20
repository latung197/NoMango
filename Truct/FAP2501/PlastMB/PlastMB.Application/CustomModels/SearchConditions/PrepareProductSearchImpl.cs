using PlastMB.Application.CustomModels.Pagging;

namespace PlastMB.Application.CustomModels.SearchConditions
{
    /// <summary>
    /// POC008 - Màn hình chuẩn bị hàng
    /// </summary>
    public class PrepareProductSearchImpl : PaggingImpl
    {
        public string BoxSerial { get; set; } = string.Empty;// OrderNumber
        public string FromDate { get; set; } = string.Empty;//Delivery allocate from date
        public string ToDate { get; set; } = string.Empty;//Delivery allocate tp date
        public int ExportType { get; set; }//EnumExportType.Type
        public int OrderState {  get; set; }//EnumCommon. 0 là chưa lên đơn. 1 là đã lên đơn
    }
}