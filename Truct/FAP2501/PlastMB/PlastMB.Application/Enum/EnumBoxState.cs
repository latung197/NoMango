using System.ComponentModel;

namespace PlastMB.Application.Enum
{
    public static class EnumBoxState
    {
        /// <summary>
        /// Trạng thái thùng hàng
        /// </summary>
        public enum State
        {
            [Description("Đang chuẩn bị")]  // Chưa xuất
            NotExport = 0,
            [Description("Đã đủ")]
            Enough = 1,
            [Description("Đã xuất")]        // Đã nằm trong đơn hàng đã được xuất đi
            Exported = 2,
            [Description("Hủy")]
            Cancel = 3
        }
    }
}
