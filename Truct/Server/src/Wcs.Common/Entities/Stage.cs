using Wcs.Common.ValueObjects;

namespace Wcs.Common.Entities;

public sealed class Stage
{
    public Guid Id { get; private set; }
    public string Name { get; set; }
    public string Code { get; set; }
    public StageArea Area { get; set; }
    public float Distance { get; set; } = 1;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool ReturnEmpty { get; set; }

    // ... other property
    public IReadOnlyList<Station> Stations => _stations.AsReadOnly();
    private readonly List<Station> _stations = [];

    // Business methods
    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;

    // Default constructor
    //public Stage() { }

    // Main constructor - original signature preserved
    public Stage(Guid id, string name, string code, StageArea area)
    {
        Id = id;
        Code = code;
        Name = name;
        Area = area;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public Stage(Guid id, string name, string code, StageArea area, float distance)
    {
        Id = id;
        Code = code;
        Name = name;
        Area = area;
        Distance = distance;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    // Quan hệ stage > station (đơn giản)
    public void AddStation(Station station)
    {
        if (station.StageCode != Code) throw new ArgumentException("Station.StageCode mismatch");
        _stations.Add(station);
    }
}