namespace Wcs.Okamura.Enums;

public enum CurtainState
{
    Unknown = 0,
    Closed = 1,   // vùng an toàn đang đóng, không cho xâm nhập
    Open = 2      // vùng an toàn đã mở/cho phép (ví dụ SafetySensorOff = 1)
}

public enum CurtainCommandType
{
    Open = 1,
    Close = 2
}