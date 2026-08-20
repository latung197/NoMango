namespace Wcs.Okamura;



public static class OpcTagsCvAgv

{

    // ============================================================

    // INBOUND (Nhập kho) - bạn đã đưa bảng trước đó

    // ============================================================



    // CV ---> AGV (Inbound) — PLC → PC

    public const string D2001 = "DeviceM.CvAgv.OPC.dev_in_ready";                         // Sẵn sàng hoạt động / Ready

    public const string D2002 = "DeviceM.CvAgv.OPC.dev_in_allowHandover";               // Cho phép bàn giao

    public const string D2003 = "DeviceM.CvAgv.OPC.dev_in_safetySensorOffOrClosed";       // Cảm biến an toàn tắt / đã đóng

    public const string D2004 = "DeviceM.CvAgv.OPC.dev_in_conveyorOrTurntableRunning";   // Băng tải/bàn quay đang chạy

    public const string D2021 = "DeviceM.CvAgv.OPC.dev_in_craneReady";   // Cần cẩu sẵn sàng

    public const string D2024 = "DeviceM.CvAgv.OPC.dev_in_safetySensorStatus";   // trạng thái của rèm an toàn, 0 là đang bật rèm, 1 là mute rèm

    public const string D2028 = "DeviceM.CvAgv.OPC.dev_in_hasBox";   // trạng thái hàng ở cửa kho xuất, nếu 1 là có hàng, 0 là ko có hàng

    public const string D2030 = "DeviceM.CvAgv.OPC.dev_in_hasBox2";   // trạng thái hàng ở cửa kho xuất, nếu 1 là có hàng, 0 là ko có hàng



    // public const string D2005 = "DeviceM.CvAgv.OPC.dev_in_reserved2005";

    // public const string D2006 = "DeviceM.CvAgv.OPC.dev_in_reserved2006";

    // public const string D2007 = "DeviceM.CvAgv.OPC.dev_in_reserved2007";

    // public const string D2008 = "DeviceM.CvAgv.OPC.dev_in_reserved2008";

    // public const string D2009 = "DeviceM.CvAgv.OPC.dev_in_reserved2009";

    // public const string D2010 = "DeviceM.CvAgv.OPC.dev_in_reserved2010";

    // D2351–D2360: 10 ô nhớ liên tiếp trên PLC, OPC server expose thành 1 node array/string

    public const string D2351_2360 = "DeviceM.CvAgv.OPC.dev_in_qr";  // block 10 words (D2351–D2360)

    public const string D2361 = "DeviceM.CvAgv.OPC.dev_in_qrReady";



    // AGV ---> CV (Inbound) — PC → PLC

    public const string D2101 = "DeviceM.CvAgv.PLC.wcs_in_ready";                       // Sẵn sàng hoạt động / Ready

    public const string D2102 = "DeviceM.CvAgv.PLC.wcs_in_arrived";                     // AGV đã đến

    public const string D2103 = "DeviceM.CvAgv.PLC.wcs_in_handoverInProgress";            // Đang bàn giao

    public const string D2104 = "DeviceM.CvAgv.PLC.wcs_in_intrusionDetected";            // Hành vi xâm nhập của AGV



    // public const string D2105 = "DeviceM.CvAgv.PLC.wcs_in_reserved2105";

    // public const string D2106 = "DeviceM.CvAgv.PLC.wcs_in_reserved2106";

    // public const string D2107 = "DeviceM.CvAgv.PLC.wcs_in_reserved2107";

    // public const string D2108 = "DeviceM.CvAgv.PLC.wcs_in_reserved2108";

    // public const string D2109 = "DeviceM.CvAgv.PLC.wcs_in_reserved2109";

    // public const string D2110 = "DeviceM.CvAgv.PLC.wcs_in_reserved2110";

    public const string D2451 = "DeviceM.CvAgv.PLC.wcs_in_qrFinished";





    // ============================================================

    // OUTBOUND (Xuất kho) - theo ẢNH bạn gửi (D2011..D2020 & D2111..D2120)

    // ============================================================



    // CV ---> AGV (Outbound) — PLC → PC

    public const string D2011 = "DeviceM.CvAgv.OPC.dev_out_ready";                      // Sẵn sàng hoạt động / trạng thái Ready

    public const string D2012 = "DeviceM.CvAgv.OPC.dev_out_requestAgvArrive";           // Tín hiệu yêu cầu AGV

    public const string D2013 = "DeviceM.CvAgv.OPC.dev_out_safetySensorOffOrClosed";    // Cảm biến an toàn đóng/tắt

    public const string D2014 = "DeviceM.CvAgv.OPC.dev_out_conveyorOrTurntableRunning"; // Băng tải quay/băng chuyền đang chạy

    public const string D2022 = "DeviceM.CvAgv.OPC.dev_out_craneReady";   // Cần cẩu sẵn sàng

    public const string D2026 = "DeviceM.CvAgv.OPC.dev_out_safetySensorStatus";   // trạng thái của rèm an toàn, 0 là đang bật rèm, 1 là mute rèm

    public const string D2027 = "DeviceM.CvAgv.OPC.dev_out_hasBox";   // trạng thái hàng ở cửa kho xuất, nếu 1 là có hàng, 0 là ko có hàng

    public const string D2029 = "DeviceM.CvAgv.OPC.dev_out_hasBox2";   // trạng thái hàng ở cửa kho xuất, nếu 1 là có hàng, 0 là ko có hàng



    // public const string D2015 = "DeviceM.CvAgv.OPC.dev_out_reserved2015";

    // public const string D2016 = "DeviceM.CvAgv.OPC.dev_out_reserved2016";

    // public const string D2017 = "DeviceM.CvAgv.OPC.dev_out_reserved2017";

    // public const string D2018 = "DeviceM.CvAgv.OPC.dev_out_reserved2018";

    // public const string D2019 = "DeviceM.CvAgv.OPC.dev_out_reserved2019";

    // public const string D2020 = "DeviceM.CvAgv.OPC.dev_out_reserved2020";

    // D2311–D2320: 10 ô nhớ liên tiếp trên PLC, OPC server expose thành 1 node array/string

    public const string D2311_2320 = "DeviceM.CvAgv.OPC.dev_out_qr"; // block 10 words (D2311–D2320)

    public const string D2321 = "DeviceM.CvAgv.OPC.dev_out_qrReady";



    // AGV ---> CV (Outbound) — PC → PLC

    public const string D2111 = "DeviceM.CvAgv.PLC.wcs_out_ready";                      // Sẵn sàng hoạt động / trạng thái Ready

    public const string D2112 = "DeviceM.CvAgv.PLC.wcs_out_arrived";                      // Tín hiệu AGV đã đến

    public const string D2113 = "DeviceM.CvAgv.PLC.wcs_out_movingOrCarrying";             // AGV đang di chuyển / đang chuyển hàng

    public const string D2114 = "DeviceM.CvAgv.PLC.wcs_out_intrusionDetected";            // Hành vi xâm nhập của AGV

    public const string D2115 = "DeviceM.CvAgv.PLC.wcs_out_armOrReceiveAction";           // Hành động tiếp nhận của AGV (AGV nhận hàng)



    // public const string D2116 = "DeviceM.CvAgv.PLC.wcs_out_reserved2116";

    // public const string D2117 = "DeviceM.CvAgv.PLC.wcs_out_reserved2117";

    // public const string D2118 = "DeviceM.CvAgv.PLC.wcs_out_reserved2118";

    // public const string D2119 = "DeviceM.CvAgv.PLC.wcs_out_reserved2119";

    // public const string D2120 = "DeviceM.CvAgv.PLC.wcs_out_reserved2120";

    public const string D2411 = "DeviceM.CvAgv.PLC.wcs_out_qrFinished";

}

