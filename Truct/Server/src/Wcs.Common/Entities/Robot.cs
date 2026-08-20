namespace Wcs.Common.Entities;

public sealed class Robot
{
    public Guid Id { get; set; }
    public string Code { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Parameterless constructor required for configuration binding/deserialization
    public Robot()
    {
        Id = Guid.NewGuid();
        Code = string.Empty;
        Name = string.Empty;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    // Constructor for creating new robot
    public Robot(string code) {
        Id = Guid.NewGuid();
        Code = code;
        Name = string.Empty;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public Robot(string code, string name, bool isActive = true)
    {
        Id = Guid.NewGuid();
        Code = code;
        Name = name;
        IsActive = isActive;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    // Business methods
    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
    public void UpdateName(string newName)
    {
        Name = newName;
        UpdatedAt = DateTime.UtcNow;
    }
}