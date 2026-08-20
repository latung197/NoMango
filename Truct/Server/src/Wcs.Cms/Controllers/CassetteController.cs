using Microsoft.AspNetCore.Mvc;
using Wcs.Cms.Controllers.DTOs.Requests;
using Wcs.Cms.Controllers.DTOs.Responses;
using Wcs.Cms.Services;

namespace Wcs.Cms.Controllers;

[ApiController]
[Route("api/")]
public class CassetteController(CassetteService cassetteService, ILogger<CassetteController> logger) : ControllerBase
{
    private readonly CassetteService _cassetteService = cassetteService;
    private readonly ILogger<CassetteController> _logger = logger;

    /// <summary>
    /// Lấy danh sách tất cả cassettes
    /// </summary>
    [HttpGet("cassettes")]
    public async Task<ActionResult<BaseResponse<IEnumerable<CassetteResponse>>>> List()
    {
        var cassettes = await _cassetteService.GetAllAsync();
        
        return Ok(BaseResponse<IEnumerable<CassetteResponse>>.SuccessResult(
            [.. cassettes.Select(CassetteResponse.FromCassette)],
            "Lấy danh sách Cassette thành công"
        ));
    }

    /// <summary>
    /// Lấy chi tiết cassette theo id
    /// </summary>
    [HttpGet("cassettes/{id}")]
    public async Task<ActionResult<BaseResponse<CassetteResponse>>> Show(string id)
    {
        var cassette = await _cassetteService.ShowCassetteAsync(id);
        return Ok(BaseResponse<CassetteResponse>.SuccessResult(
            CassetteResponse.FromCassette(cassette), 
            "Lấy thông tin Cassette thành công"
        ));
    }

    /// <summary>
    /// Lấy chi tiết cassette theo code
    /// </summary>
    [HttpGet("cassettes/by-code/{code}")]
    public async Task<ActionResult<BaseResponse<CassetteResponse>>> GetStationByCode(string code)
    {
        var cassette = await _cassetteService.GetStationByCode(code);
        if (cassette == null)
        {
            return NotFound(BaseResponse<CassetteResponse>.ErrorResult($"Cassette với Code {code} không tồn tại", "CASSETTE_NOT_FOUND"));
        }
        return Ok(BaseResponse<CassetteResponse>.SuccessResult(CassetteResponse.FromCassette(cassette), "Lấy thông tin Cassette thành công"));
    }

    /// <summary>
    /// Tạo cassette mới
    /// </summary>
    [HttpPost("cassettes")]
    public async Task<ActionResult<BaseResponse<CassetteResponse>>> Create(CassetteCreateRequest request)
    {
        var cassette = await _cassetteService.CreateCassetteAsync(request);
        return Ok(BaseResponse<CassetteResponse>.SuccessResult(
            CassetteResponse.FromCassette(cassette), 
            "Tạo Cassette thành công"
        ));
    }

    /// <summary>
    /// Cập nhật cassette
    /// </summary>
    [HttpPut("cassettes/{id}")]
    public async Task<ActionResult<BaseResponse<CassetteResponse>>> Update(string id, CassetteUpdateRequest request)
    {
        var cassette = await _cassetteService.UpdateCassetteAsync(id, request);
        return Ok(BaseResponse<CassetteResponse>.SuccessResult(
            CassetteResponse.FromCassette(cassette), 
            "Cập nhật Cassette thành công"
        ));
    }

    /// <summary>
    /// Xóa cassette
    /// </summary>
    [HttpDelete("cassettes/{id}")]
    public async Task<ActionResult<BaseResponse>> Delete(string id)
    {
        await _cassetteService.DeleteCassetteAsync(id);
        return Ok(BaseResponse.SuccessResult("Xóa Cassette thành công"));
    }
}
