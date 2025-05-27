using System.ComponentModel;

namespace Core.Application.Enum
{
    //Phân biệt mã  gửi lên từ handy là loại nào
    public static class EnumMasterType
    {
        public enum Type
        {
            [Description("Mã bản vẽ")]
            DrawingCode = 0,
            [Description("Mã sản phẩm")]
            ProductCode = 1,
            //[Description("Mã sản phẩm lắp cụm")]
            //ProductCode = 2,
        }
    }
}
