using System.ComponentModel;

namespace Core.Application.Enum
{
    public static class EnumCommon
    {
        public enum Status
        {
            [Description("Invalid flg")]
            Invalid = 0,
            [Description("Valid flg")]
            Valid = 1,
        }
    }
}
