using System.ComponentModel;

namespace Astemo.Application.Enum
{
    /// <summary>
    /// Xuất rời - Xuất ASSY
    /// </summary>
    public static class EnumExportType
    {
        public enum Type
        {
            [Description("Xuất rời trong nước")]
            LooseDomestic = 0,
            [Description("Xuất rời ra nước ngoài")]
            LooseInternational = 1,
            [Description("Xuất ASSY (Xuất cụm)")]
            ASSY = 2,
        }
    }
}
