using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Wcs.Cms.Controllers.DTOs.Requests;
using Wcs.Cms.Controllers.DTOs.Responses;

namespace Wcs.Cms.Controllers;

[ApiController]
[Route("api/")]
public class NoticeController(ILogger<NoticeController> logger, IHubContext<NotificationsHub> hub) : ControllerBase
{
    private readonly ILogger<NoticeController> _logger = logger;
    private readonly IHubContext<NotificationsHub> _hub = hub;

    [HttpPost("notice")]
    public async Task<ActionResult<BaseResponse>> CreateNotice(NoticeCreateRequest request)
    {
        await _hub.Clients.All.SendAsync("Notication", request);
        return Ok(BaseResponse.SuccessResult("Thông báo đã được tạo thành công"));
    }
}