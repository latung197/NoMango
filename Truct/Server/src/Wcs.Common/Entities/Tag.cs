namespace Wcs.Common.Entities;

public sealed class Tag
{
    public required string TagOPC { get; set; }
    public required string TagName { get; set; }
    public string? Description { get; set; }
    public required string DeviceType { get; set; }
    public required string Address { get; set; }
    public required string DataType { get; set; }
    public required string Location { get; set; }

    // Parameterless constructor required for configuration binding/deserialization
    /*public Tag()
    {
        TagOPC = string.Empty;
        TagName = string.Empty;
        Description = string.Empty;
        DeviceType = string.Empty;
        Address = 0;
        DataType = string.Empty;
        Location = string.Empty;
    }*/

    // Constructor for creating new robot

    // Business methods
    //public void Activate() => IsActive = true;
    //public void Deactivate() => IsActive = false;
    //public void UpdateName(string newName)
    //{
    //    Name = newName;
    //    UpdatedAt = DateTime.UtcNow;
    //}
}