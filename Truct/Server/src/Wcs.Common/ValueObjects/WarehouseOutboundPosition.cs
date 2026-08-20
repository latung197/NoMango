namespace Wcs.Common.ValueObjects;

public sealed class WarehouseOutboundPosition
{
    public int Row { get; init; }
    public int Floor { get; init; }
    public int PositionNumber { get; init; }
    public string? CassetteId { get; init; }

    public bool IsValid => Row > 0 && Floor > 0 && PositionNumber > 0;
}
