namespace Wcs.Okamura.Models;

using Wcs.Okamura.Enums;

/// <summary>
/// Confirm data từ PLC (D3042–D3049, D3056) dùng xác định task khi có lỗi D3054.
/// </summary>
public record CraneConfirmData(
    int Tunnel,
    int StartPosX,
    int StartPosY,
    int StartPosZ,
    int EndPosX,
    int EndPosY,
    int EndPosZ,
    CraneTaskType TaskType,
    int TaskNo)
{
    public bool IsCleared =>
        Tunnel == 0
        && StartPosX == 0 && StartPosY == 0 && StartPosZ == 0
        && EndPosX == 0 && EndPosY == 0 && EndPosZ == 0
        && TaskType == CraneTaskType.None
        && TaskNo == 0;
}
