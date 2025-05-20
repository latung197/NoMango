using System.ComponentModel;

namespace PlastMB.Application.Enum
{
    public static class EnumOrderState
    {
        /// <summary>
        /// Trạng thái đơn hàng
        /// </summary>
        public enum State
        {
            [Description("Chưa xuất")]
            NotExport = 0,
            [Description("Đang xuất")]
            Processing = 1,
            [Description("Đã đủ")]
            Enough = 2,
            [Description("Đã xuất")]
            Exported = 3,
            [Description("Hủy")]
            Cancel = 4
        }
    }
}
