namespace Wcs.Rcs;

using Wcs.Rcs.DTOs;

public static class RcsPositions
{
    public const string BerthType = "00";

    public static PositionCodePathItem Berth(string positionCode) => new(BerthType, positionCode);
}
