namespace Wcs.Common.ValueObjects;

/// <summary>
/// Value object đại diện cho vị trí Crane
/// </summary>
public record CranePosition
{
    public int Row { get; init; }
    public int Floor { get; init; }
    public int Position { get; init; }

    public CranePosition() { }

    public CranePosition(int row, int floor, int position)
    {
        Row = row;
        Floor = floor;
        Position = position;
    }
}

