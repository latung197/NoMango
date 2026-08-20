namespace Wcs.OpcUa;

public class OpcUaOptions
{
    public string EndpointUrl { get; set; } = string.Empty;   
    public int ConnectionTimeout { get; set; } = 30;
    public int OperationTimeout { get; set; } = 60;
    public string ApplicationName { get; set; } = "WCS OPC UA Client";
    public string Organization { get; set; } = "WCS";
    public int PublishingIntervalMs { get; set; } = 200;
    public int SamplingIntervalMs { get; set; } = 100;
    public int KeepAliveIntervalMs { get; set; } = 3000;
    public bool AutoAcceptUntrustedCerts { get; set; } = true;
    public string TagClockPulseRead { get; set; } = string.Empty;
    public string TagClockPC { get; set; } = string.Empty;
    public string TagSystemStatus { get; set; } = string.Empty;
}
