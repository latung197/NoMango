using System.ComponentModel;

namespace PlastMB.Application.Enum
{
    public static class EnumReferenceType
    {
        public enum Type
        {
            [Description("Mã nội bộ")]
            InternalCode = 0,
            [Description("Mã sản phẩm")]
            ProductCode = 1,
            [Description("Mã lắp cụm")]
            Assy = 2,
        }
    }
}
