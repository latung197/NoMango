using Wcs.Common.ValueObjects;

namespace Wcs.Api.Configs;

public class HikConfig
{
    /// <summary>TaskTyp mặc định khi không tìm thấy mapping theo <see cref="Size"/>.</summary>
    public string MainFlowTaskTyp { get; set; } = string.Empty;

    /// <summary>TaskTyp theo kích thước cassette (key = tên enum <see cref="Size"/>).</summary>
    public Dictionary<string, string> MainFlowTaskTypBySize { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    public string FallbackTaskTyp { get; set; } = string.Empty;

    public string GetMainFlowTaskTyp(Size size)
    {
        var key = size.ToString();
        if (MainFlowTaskTypBySize.TryGetValue(key, out var taskTyp) && !string.IsNullOrWhiteSpace(taskTyp))
        {
            return taskTyp;
        }

        if (!string.IsNullOrWhiteSpace(MainFlowTaskTyp))
        {
            return MainFlowTaskTyp;
        }

        throw new InvalidOperationException($"TaskTyp chưa được cấu hình cho size {size}.");
    }

    /// <summary>
    /// Số "filler leg" (leg đệm) HIK giữa BeforeDrop và EnterDropPoint trong template.
    /// Mỗi filler leg là một <c>continueTask</c> tới drop waiting point (case thường = giữ chỗ,
    /// case redirect = di chuyển sang trạm mới). Đặt 0 nếu template không có leg đệm (không hỗ trợ redirect).
    /// </summary>
    public int DropFillerCount { get; set; } = 1;
}
