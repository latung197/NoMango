namespace Wcs.Common.Entities;

public sealed class Signal
{
    public string Area { get; set; }
    public string Device { get; set; } = string.Empty;
    public List<Tag> Tags { get; set; } = [];

    // Parameterless constructor required for configuration binding/deserialization
    public Signal()
    {
        Area = string.Empty;
        Device = string.Empty;
        Tags = [];
    }

    // Constructor for creating new robot
    public Signal(string area, string device)
    {
        Area = area;
        Device = device;
        Tags = [];
    }

    // Business methods
    //public void Activate() => IsActive = true;
    //public void Deactivate() => IsActive = false;
    //public void UpdateName(string newName)
    //{
    //    Name = newName;
    //    UpdatedAt = DateTime.UtcNow;
    //}
}