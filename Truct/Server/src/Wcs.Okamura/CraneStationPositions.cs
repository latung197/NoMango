using Wcs.Okamura.Models;

namespace Wcs.Okamura;

/// <summary>
/// Vị trí conveyor/station cố định trên crane map (PLC coordinates).
/// </summary>
public static class CraneStationPositions
{
    /// <summary>Vị trí lấy hàng tại station (start position).</summary>
    public static readonly CranePosition Pick = new(Row: 1, Floor: 1, Position: 1);

    /// <summary>Vị trí trả hàng tại station (end position).</summary>
    public static readonly CranePosition Put = new(Row: 1, Floor: 1, Position: 18);
}
