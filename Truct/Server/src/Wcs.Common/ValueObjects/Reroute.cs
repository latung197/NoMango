namespace Wcs.Common.ValueObjects;

/// <summary>
/// Mô tả một yêu cầu điều hướng lại task giữa chừng (đổi đích tại runtime),
/// ví dụ khi trạm hạ hàng không sẵn sàng và cần chuyển AMR sang trạm khác.
///
/// Đây là khái niệm chung, không gắn riêng với "drop": bất kỳ luồng nào cần
/// đưa AMR tới một điểm mới rồi chạy lại một nhánh step đều có thể dùng.
/// </summary>
/// <param name="ExpectedHikLeg">
/// Số leg HIK (Method prefix, ví dụ 71) báo hiệu AMR đã tới điểm reroute.
/// </param>
/// <param name="ResumeStep">
/// Step sẽ chạy lại sau khi AMR tới điểm reroute (ví dụ <see cref="FlowStep.BeforeDrop"/>
/// để chờ rèm tại trạm mới).
/// </param>
public record RerouteRequest(
    int ExpectedHikLeg,
    FlowStep ResumeStep
);
