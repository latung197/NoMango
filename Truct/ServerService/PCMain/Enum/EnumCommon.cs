using System.ComponentModel;

namespace PCMain.BaseHttp.Interface
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
