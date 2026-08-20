using Wcs.Common.ValueObjects;

namespace Wcs.Common.Entities;

public sealed class Amr
{
    public Guid Id { get; private set; }
    public string Name { get; set; }
    public string Code { get; set; }
    public StageArea Area { get; set; }
    public string MapCode { get; set; } = string.Empty;
    public string MapName { get; set; } = string.Empty;
    public string RobotStatus { get; set; } = string.Empty;
    public string TypeCode { get; set; } = string.Empty;
    public string Battery { get; set; } = string.Empty;
    public string Direction { get; set; } = string.Empty;
    public string Exclude { get; set; } = string.Empty;
    public string ExcludeStr { get; set; } = string.Empty;
    public string OnLine { get; set; } = string.Empty;
    public string PodCode { get; set; } = string.Empty;
    public string PodDir { get; set; } = string.Empty;
    public string PosX { get; set; } = string.Empty;
    public string PosY { get; set; } = string.Empty;
    public string Ip { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string StatusStr { get; set; } = string.Empty;
    public string Stop { get; set; } = string.Empty;
    public string StopStr { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // ... other property
    public IReadOnlyList<Station> Stations => _stations.AsReadOnly();
    private readonly List<Station> _stations = [];

    // Business methods
    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;

    // Default constructor
    //public Amr() { }

    // Main constructor - original signature preserved
    public Amr(Guid id, string name, string code, StageArea area)
    {
        Id = id;
        Code = code;
        Name = name;
        Area = area;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public Amr(Guid id, string name, string code, string? mapCode, string? status, string? typeCode)
    {
        Id = id;
        Code = code;
        Name = name;
        //Area = area;
        MapCode = mapCode ?? string.Empty;
        RobotStatus = status ?? string.Empty;
        TypeCode = typeCode ?? string.Empty;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
}