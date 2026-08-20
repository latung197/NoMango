namespace Wcs.Rcs;

public sealed class RcsOptions
{
    public string BaseUrl { get; set; } = "http://127.0.0.1:8182/rcms/services/rest/hikRpcService";
    public string DpsUrl  { get; set; } = "http://127.0.0.1:8083/rcms-dps/rest";
    public string ClientCode { get; set; } = "WCS";
    public string TokenCode  { get; set; } = ""; // nạp từ secret/ENV
}