using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Wcs.Cms.Controllers.DTOs.Requests;
using Wcs.Cms.Controllers.DTOs.Responses;
using Wcs.Cms.Services;
using Wcs.Common.Entities;

namespace Wcs.Cms.Controllers;

[ApiController]
[Route("api/")]
public class RequestController(RequestService requestService,
                               ILogger<RequestController> logger,
                               IHubContext<NotificationsHub> hub) : ControllerBase
{
    private readonly RequestService _requestService = requestService;
    private readonly ILogger<RequestController> _logger = logger;
    private readonly IHubContext<NotificationsHub> _hub = hub;

    /// <summary>
    /// Lấy danh sách tất cả requests
    /// </summary>
    [HttpGet("requests")]
    public async Task<ActionResult<BaseResponse<IEnumerable<RequestResponse>>>> List([FromQuery] RequestListRequest data)
    {
        var requests = await _requestService.GetAllAsync(data);
        
        return Ok(BaseResponse<IEnumerable<RequestResponse>>.SuccessResult(
            [.. requests.Select(RequestResponse.FromRequest)],
            "Lấy danh sách yêu cầu thành công"
        ));
    }

    /// <summary>
    /// Lấy chi tiết request theo id
    /// </summary>
    [HttpGet("requests/{id}")]
    public async Task<ActionResult<BaseResponse<RequestResponse>>> Show(string id)
    {
        var request = await _requestService.ShowRequestAsync(id);
        return Ok(BaseResponse<RequestResponse>.SuccessResult(
            RequestResponse.FromRequest(request),
            "Lấy thông tin yêu cầu thành công"
        ));
    }

    /// <summary>
    /// Lấy chi tiết request theo code
    /// </summary>
    [HttpGet("requests/by-code/{code}")]
    public async Task<ActionResult<BaseResponse<RequestResponse>>> GetRequestByCode(string code)
    {
        var request = await _requestService.GetRequestByCode(code);
        if (request == null)
        {
            return NotFound(BaseResponse<RequestResponse>.ErrorResult($"Yêu cầu với Code {code} không tồn tại", "REQUEST_NOT_FOUND"));
        }
        return Ok(BaseResponse<RequestResponse>.SuccessResult(RequestResponse.FromRequest(request), "Lấy thông tin yêu cầu thành công"));
    }

    /// <summary>
    /// Tạo request mới
    /// </summary>
    [HttpPost("requests")]
    public async Task<ActionResult<BaseResponse<RequestResponse>>> Create(RequestCreateRequest data)
    {
        var request = await _requestService.CreateRequestAsync(data);
        var response = RequestResponse.FromRequest(request);
        await _hub.Clients.All.SendAsync("MaterialRequested", response);
        return Ok(BaseResponse<RequestResponse>.SuccessResult(
            response,
            "Tạo yêu cầu thành công"
        ));
    }

    /// <summary>
    /// Cập nhật request
    /// </summary>
    [HttpPut("requests/{id}")]
    public async Task<ActionResult<BaseResponse<RequestResponse>>> Update(string id, RequestUpdateRequest data)
    {
        var request = await _requestService.UpdateRequestAsync(id, data);
        var message = "Cập nhật yêu cầu thành công";
        if (request.Note == "DeliverNextPoint")
        {
            message = "Giao thành công! Robot sẽ tiếp tục giao đến điểm tiếp theo.";
        }
        return Ok(BaseResponse<RequestResponse>.SuccessResult(
            RequestResponse.FromRequest(request),
            message
        ));
    }

    /// <summary>
    /// Cập nhật requests
    /// </summary>
    [HttpPatch("requests/updates")]
    public async Task<ActionResult<BaseResponse<IEnumerable<RequestResponse>>>> Updates([FromBody] List<RequestUpdateRequest> datas)
    {
        var requests = await _requestService.UpdatesRequestAsync(datas);

        //if (result.Count == datas.Count)
        return Ok(BaseResponse<IEnumerable<RequestResponse>>.SuccessResult(
            [.. requests.Select(RequestResponse.FromRequest)],
            "Cập nhật danh sách yêu cầu thành công"
        ));

        //return BadRequest(new { Message = "Có lỗi xảy ra khi cập nhật." });
    }

    /// <summary>
    /// Xóa request
    /// </summary>
    [HttpDelete("requests/{id}")]
    public async Task<ActionResult<BaseResponse>> Delete(string id)
    {
        await _requestService.DeleteRequestAsync(id);
        return Ok(BaseResponse.SuccessResult("Xóa yêu cầu thành công"));
    }

    /// <summary>
    /// Thêm nguyên vật liệu vào request
    /// </summary>
    [HttpPost("requests/{id}/materials")]
    public async Task<ActionResult<BaseResponse<MaterialRequest>>> AddMaterial(string id, MaterialRequestDto data)
    {
        var materialRequest = await _requestService.AddMaterialRequestAsync(id, data);
        return Ok(BaseResponse<MaterialRequest>.SuccessResult(
            materialRequest,
            "Thêm nguyên vật liệu thành công"
        ));
    }

    /// <summary>
    /// Cập nhật nguyên vật liệu trong request
    /// </summary>
    [HttpPut("requests/{id}/materials/{materialRequestId}")]
    public async Task<ActionResult<BaseResponse<MaterialRequest>>> UpdateMaterial(string id, string materialRequestId, MaterialRequestDto data)
    {
        var materialRequest = await _requestService.UpdateMaterialRequestAsync(id, materialRequestId, data);
        return Ok(BaseResponse<MaterialRequest>.SuccessResult(
            materialRequest,
            "Cập nhật nguyên vật liệu thành công"
        ));
    }

    /// <summary>
    /// Xóa nguyên vật liệu khỏi request
    /// </summary>
    [HttpDelete("requests/{id}/materials/{materialRequestId}")]
    public async Task<ActionResult<BaseResponse>> DeleteMaterial(string id, string materialRequestId)
    {
        await _requestService.DeleteMaterialRequestAsync(id, materialRequestId);
        return Ok(BaseResponse.SuccessResult("Xóa nguyên vật liệu thành công"));
    }
}
