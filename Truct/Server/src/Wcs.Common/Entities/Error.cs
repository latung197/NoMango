using Wcs.Common.ValueObjects;

namespace Wcs.Common.Entities;

public sealed class Error
{
    public Guid Id { get; private set; }
    public string Code { get; set; }
    public string? Message { get; set; }
    public ErrorType ErrorType { get; set; }
    //public string? StackTrace { get; set; }
    //public string? ActionRequired { get; set; }
    //public DateTime ErrorDate { get; set; }
    //public DateTime? ResolvedDate { get; set; }
    //public bool IsResolved { get; set; } = false;
    //public StageArea? Area { get; set; }
    public string? Area { get; set; }
    //public string? Stage { get; set; }
    //public string? Station { get; set; }
    public string? FlowTaskId { get; set; }
    public string? FromStation { get; set; }
    public string? ToStation { get; set; }
    public ErrorStatus Status { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Business methods
    //public void Activate() => IsResolved = false;
    //public void Deactivate() => IsResolved = true;
    public void Activate() => Status = ErrorStatus.InProcessing;
    public void Deactivate() => Status = ErrorStatus.Resolved;

    // Default constructor
    //public Error() { }

    // Main constructor - original signature preserved
    public Error(Guid id, string code)
    {
        Id = id;
        Code = code;
    }

    public Error(Guid id, string code, string? area)
    {
        Id = id;
        Code = code;
        Area = area;
    }

    public Error(Guid id, string code, string? area, string? stage, string? station)
    {
        Id = id;
        Code = code;
        //Area = string.IsNullOrWhiteSpace(area) ? null : StageArea.FromString(area);
        Area = area;
        //Stage = stage;
        //Station = station;
    }

    public Error(Guid id, string code, ErrorType type, string? area, string? stage, string? station)
    {
        Id = id;
        Code = code;
        ErrorType = type;
        //Area = string.IsNullOrWhiteSpace(area) ? null : StageArea.FromString(area);
        Area = area;
        //Stage = stage;
        //Station = station;
    }
}