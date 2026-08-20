using Microsoft.AspNetCore.Mvc;
using Wcs.Cms.Controllers.DTOs.Requests;
using Wcs.Cms.Controllers.DTOs.Responses;
using Wcs.Cms.Services;

namespace Wcs.Cms.Controllers;

[ApiController]
[Route("api/")]
public class MaterialController(MaterialService materialService, ILogger<MaterialController> logger) : ControllerBase
{
    private readonly MaterialService _materialService = materialService;
    private readonly ILogger<MaterialController> _logger = logger;

    /// <summary>
    /// Lấy danh sách tất cả materials
    /// </summary>
    [HttpGet("materials")]
    public async Task<ActionResult<BaseResponse<IEnumerable<MaterialResponse>>>> List([FromQuery] MaterialListRequest request)
    {
        var materials = await _materialService.GetAllAsync(request);
        
        return Ok(BaseResponse<IEnumerable<MaterialResponse>>.SuccessResult(
            [.. materials.Select(MaterialResponse.FromMaterial)],
            "Lấy danh sách vật liệu thành công"
        ));
    }

    /// <summary>
    /// Lấy chi tiết material theo id
    /// </summary>
    [HttpGet("materials/{id}")]
    public async Task<ActionResult<BaseResponse<MaterialResponse>>> Show(string id)
    {
        var material = await _materialService.ShowMaterialAsync(id);
        return Ok(BaseResponse<MaterialResponse>.SuccessResult(
            MaterialResponse.FromMaterial(material),
            "Lấy thông tin vật liệu thành công"
        ));
    }

    /// <summary>
    /// Lấy chi tiết material theo code
    /// </summary>
    [HttpGet("materials/by-code/{code}")]
    public async Task<ActionResult<BaseResponse<MaterialResponse>>> GetStationByCode(string code)
    {
        var material = await _materialService.GetStationByCode(code);
        if (material == null)
        {
            return NotFound(BaseResponse<MaterialResponse>.ErrorResult($"Vật liệu với Code {code} không tồn tại", "MATERIAL_NOT_FOUND"));
        }
        return Ok(BaseResponse<MaterialResponse>.SuccessResult(MaterialResponse.FromMaterial(material), "Lấy thông tin vật liệu thành công"));
    }

    /// <summary>
    /// Tạo material mới
    /// </summary>
    [HttpPost("materials")]
    public async Task<ActionResult<BaseResponse<MaterialResponse>>> Create(MaterialCreateRequest request)
    {
        var material = await _materialService.CreateMaterialAsync(request);
        return Ok(BaseResponse<MaterialResponse>.SuccessResult(
            MaterialResponse.FromMaterial(material),
            "Tạo vật liệu thành công"
        ));
    }

    /// <summary>
    /// Cập nhật material
    /// </summary>
    [HttpPut("materials/{id}")]
    public async Task<ActionResult<BaseResponse<MaterialResponse>>> Update(string id, MaterialUpdateRequest request)
    {
        var material = await _materialService.UpdateMaterialAsync(id, request);
        return Ok(BaseResponse<MaterialResponse>.SuccessResult(
            MaterialResponse.FromMaterial(material),
            "Cập nhật vật liệu thành công"
        ));
    }

    /// <summary>
    /// Xóa material
    /// </summary>
    [HttpDelete("materials/{id}")]
    public async Task<ActionResult<BaseResponse>> Delete(string id)
    {
        await _materialService.DeleteMaterialAsync(id);
        return Ok(BaseResponse.SuccessResult("Xóa vật liệu thành công"));
    }
}
