namespace Wcs.Okamura;

public static class OpcTags
{
    // ================================
    // PC → PLC (Instruction Data)
    // ================================

    public const string D3000 = "DeviceM.Stacker.PLC.wcs_heartBeat";
    public const string D3001 = "DeviceM.Stacker.PLC.wcs_deviceCode";
    public const string D3002 = "DeviceM.Stacker.PLC.wcs_taryType";
    public const string D3003 = "DeviceM.Stacker.PLC.wcs_startPosX";
    public const string D3004 = "DeviceM.Stacker.PLC.wcs_startPosY";
    public const string D3005 = "DeviceM.Stacker.PLC.wcs_startPosZ";
    public const string D3006 = "DeviceM.Stacker.PLC.wcs_endPosX";
    public const string D3007 = "DeviceM.Stacker.PLC.wcs_endPosY";
    public const string D3008 = "DeviceM.Stacker.PLC.wcs_endPosZ";
    public const string D3009 = "DeviceM.Stacker.PLC.wcs_taskType";

    public const string D3010 = "DeviceM.Stacker.PLC.wcs_taskExe";
    public const string D3011 = "DeviceM.Stacker.PLC.wcs_stStop";
    public const string D3012 = "DeviceM.Stacker.PLC.wcs_stReset";
    public const string D3013 = "DeviceM.Stacker.PLC.wcs_taskFinishAffirm";
    public const string D3014 = "DeviceM.Stacker.PLC.wcs_taskErrorAffirm";
    public const string D3015 = "DeviceM.Stacker.PLC.wcs_taskInAffirm";
    public const string D3016 = "DeviceM.Stacker.PLC.wcs_taskNo";

    // Bạn có thể bổ sung thêm nếu sử dụng D3117–D3139.


    // ================================
    // PLC → PC (Confirmation Data)
    // ================================

    public const string D3040 = "DeviceM.Stacker.OPC.dev_heartBeat";
    public const string D3041 = "DeviceM.Stacker.OPC.dev_deviceCode";
    public const string D3042 = "DeviceM.Stacker.OPC.dev_tunnel";

    public const string D3043 = "DeviceM.Stacker.OPC.dev_startPosX";
    public const string D3044 = "DeviceM.Stacker.OPC.dev_startPosY";
    public const string D3045 = "DeviceM.Stacker.OPC.dev_startPosZ";

    public const string D3046 = "DeviceM.Stacker.OPC.dev_endPosX";
    public const string D3047 = "DeviceM.Stacker.OPC.dev_endPosY";
    public const string D3048 = "DeviceM.Stacker.OPC.dev_endPosZ";

    public const string D3049 = "DeviceM.Stacker.OPC.dev_taskType";

    public const string D3050 = "DeviceM.Stacker.OPC.dev_taskExe";
    public const string D3051 = "DeviceM.Stacker.OPC.dev_error";
    public const string D3052 = "DeviceM.Stacker.OPC.dev_taryType";
    public const string D3053 = "DeviceM.Stacker.OPC.dev_taskFinishState";
    public const string D3054 = "DeviceM.Stacker.OPC.dev_taskAbnormalFeedback";
    public const string D3055 = "DeviceM.Stacker.OPC.dev_claimGoods";
    public const string D3056 = "DeviceM.Stacker.OPC.dev_task";
    public const string D3057 = "DeviceM.Stacker.OPC.dev_dispatch";
    public const string D3059 = "DeviceM.Stacker.OPC.dev_pattern";

    // Vị trí / encoder crane (signal.json: D3064, D3066, D3068; không có D3065/D3067)
    public const string D3061 = "DeviceM.Stacker.OPC.dev_posX";
    public const string D3062 = "DeviceM.Stacker.OPC.dev_posY";
    public const string D3063 = "DeviceM.Stacker.OPC.dev_posZ";
    public const string D3064 = "DeviceM.Stacker.OPC.dev_locatX";
    public const string D3066 = "DeviceM.Stacker.OPC.dev_locatY";
    public const string D3068 = "DeviceM.Stacker.OPC.dev_locatZ";
}
