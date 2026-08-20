using Wcs.Common.ValueObjects;

namespace Wcs.Common.Entities;

public sealed class FlowTaskHistoryEntry
{
    public Guid Id { get; init; }
    public required string FlowTaskId { get; init; }
    public string? Event { get; init; }
    public FlowStep? Step { get; init; }
    public string? LogMessage { get; init; }
    public DateTime CreatedAt { get; init; }
}
