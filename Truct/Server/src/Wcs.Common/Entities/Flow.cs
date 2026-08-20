using Wcs.Common.ValueObjects;

namespace Wcs.Common.Entities;

public sealed class Flow
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public StageArea Area { get; set; }
    public FlowPriority Priority { get; set; }
    public bool ReturnEmpty { get; set; } = false;

    // Status computed property
    public FlowStatus Status { get; set; } = FlowStatus.Active;
    
    // Soft delete flag
    public bool IsActive { get; set; } = true;

    // Timestamps
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // ... other property
    public List<Step> Steps { get; set; } = [];

    // Default constructor
    public Flow() { }

    // Main constructor - original signature preserved
    public Flow(Guid id, string name, StageArea area, FlowPriority priority, bool returnEmpty, FlowStatus status)
    {
        Id = id;
        Name = name;
        Area = area;
        Priority = priority;
        ReturnEmpty = returnEmpty;
        Status = status;
        IsActive = true;
        Steps = [];
    }

    // Business methods
    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
}
