using System.ComponentModel;

namespace Core.Application.Enum
{
    public static class EnumPackState
    {
        /// <summary>
        /// Trạng thái sản phẩm
        /// </summary>
        public enum State
        {
            [Description("Chưa đóng gói")]
            NotPacked = 0,
            [Description("Đã đóng gói")]
            Packed = 1,
            //[Description("Đang đóng gói")]
            //Processing = 2,
            //[Description("Đã xuất")]
            //Exported = 3,
            //[Description("Hủy")]
            //Cancel = 4
        }
    }
}
