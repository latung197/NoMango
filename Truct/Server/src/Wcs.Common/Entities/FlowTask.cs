using Wcs.Common.ValueObjects;

namespace Wcs.Common.Entities;

public sealed class FlowTask
{
    public string Id { get; set; } = GenerateId();
    public Station FromStation { get; set; } = null!;
    public Station ToStation { get; set; } = null!;
    public Robot? Robot { get; set; } = null;
    public FlowStep CurrentStep { get; set; } = FlowStep.Initial;
    public WaitingSet? WaitingFor { get; set; }
    public string? RcsTaskId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
    public DateTime? CurrentStepStartedAt { get; set; }
    public string? LogMessage { get; set; } = string.Empty;
    // TODO: Check WMS error reporting logic when crane returns a D3054 special error.
    public List<CranePosition>? CranePositions { get; set; }
    public string? CassetteCode { get; set; }
    /// <summary>Công đoạn lưu kho gửi từ client; dùng cho WMS confirm thay cho FromStation.StageCode nếu có.</summary>
    public string? StorageStageCode { get; set; }
    /// <summary>Số lượng gửi WMS confirm inbound. Mặc định 1 nếu không truyền.</summary>
    public int? Quantity { get; set; }
    /// <summary>Mã sản phẩm gửi WMS confirm inbound.</summary>
    public string? Product { get; set; }
    /// <summary>Số task crane (D3016/D3056), phạm vi 1..60000.</summary>
    public int? CraneTaskNo { get; set; }

    /// <summary>Yêu cầu điều hướng lại đang chờ (đổi đích giữa chừng). null = không có.</summary>
    public RerouteRequest? Reroute { get; set; }

    /// <summary>Số lần task đã bị điều hướng lại — dùng để giới hạn số lần reroute.</summary>
    public int RerouteCount { get; set; }

    /// <summary>Số "filler leg" HIK (leg đệm tại drop waiting) đã tiêu thụ. Dùng để đồng bộ số leg với template HIK.</summary>
    public int ConsumedDropFillers { get; set; }

    /// <summary>Size thực tế của task (từ client/cassette).</summary>
    public Size TaskSize { get; set; } = Size.S;

    // Status computed property
    public FlowStatus Status { get; set; } = FlowStatus.Active;

    // Default constructor
    public FlowTask() { }

    // Main constructor - original signature preserved
    public FlowTask(Station fromStation, Station toStation, Robot robot, FlowStep currentStep)
    {
        Id = GenerateId();
        FromStation = fromStation ?? throw new ArgumentNullException(nameof(fromStation));
        ToStation = toStation ?? throw new ArgumentNullException(nameof(toStation));
        Robot = robot ?? throw new ArgumentNullException(nameof(robot));
        CurrentStep = currentStep;
    }

    // Business methods
    public void UpdateStep(FlowStep newStep)
    {
        CurrentStep = newStep;
        UpdatedAt = DateTime.UtcNow;
        CurrentStepStartedAt = DateTime.UtcNow;

        if (newStep == FlowStep.Completed)
        {
            CompletedAt = DateTime.UtcNow;
        }
    }

    public void Cancel()
    {
        Status = FlowStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetWaitingConditions(WaitingSet? waitingSet = null)
    {
        WaitingFor = waitingSet;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkWaitingConditionMet(string conditionType)
    {
        WaitingFor?.Mark(conditionType);
        UpdatedAt = DateTime.UtcNow;
    }

    public void Complete()
    {
        CurrentStep = FlowStep.Completed;
        CompletedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetRcsTaskId(string rcsTaskId)
    {
        RcsTaskId = rcsTaskId;
        UpdatedAt = DateTime.UtcNow;
    }

    public bool IsCompleted => CurrentStep == FlowStep.Completed || CompletedAt.HasValue;
    public bool IsActive => !IsCompleted;
    public TimeSpan? ExecutionTime => CompletedAt.HasValue ? CompletedAt.Value - CreatedAt : null;

    private static string GenerateId()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        return new string(Enumerable.Repeat(chars, 10)
            .Select(s => s[Random.Shared.Next(s.Length)]).ToArray());
    }
}
