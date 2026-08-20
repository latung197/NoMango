using Microsoft.AspNetCore.SignalR;

namespace Wcs.Cms;

public class NotificationsHub : Hub
{
    public const string CraneInboundReady = "CraneInboundReady";
    public const string CraneOutboundComplete = "CraneOutboundComplete";
}