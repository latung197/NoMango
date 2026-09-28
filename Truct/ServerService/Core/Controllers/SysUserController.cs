using Core.Application.CustomModels.Dtos;
using Core.Application.CustomModels.Others;
using Core.Application.CustomModels.SearchConditions;
using Core.Application.Interface.SysInterface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Core.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SysUserController : BaseController
    {
        #region Properties
        private readonly ISysUserService _service;
        #endregion
        #region Constructor
        public SysUserController(ISysUserService service)
        {
            _service = service;
        }

        #endregion
        #region Search
        [HttpPost("search-user")]
        [Authorize(Policy = "FunctionAccess")]
        public async Task<IActionResult> SearchUser(SysUserSearchImpl condition)
        {
            var result = await _service.SearchUser(condition);
            return new ObjectResult(result);
        }
        [HttpGet("get-user-by-id")]
        [Authorize(Policy = "FunctionAccess")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var result = await _service.GetUserById(id);
            return new ObjectResult(result);
        }
        #endregion
        #region CRUD
        [HttpPost("insert-user")]
        [Authorize(Policy = "AccessAdmin")]
        public async Task<IActionResult> InsertUser(SysUserDto dto)
        {
            var result = await _service.InsertUser(dto);
            return new ObjectResult(result);
        }
        [HttpPost("update-user")]
        [Authorize(Policy = "AccessAdmin")]
        public async Task<IActionResult> UpdateUser(SysUserDto dto)
        {
            var result = await _service.UpdateUser(dto);
            return new ObjectResult(result);
        }
        [HttpDelete("delete-user")]
        [Authorize(Policy = "AccessAdmin")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var result = await _service.DeleteUser(id);
            return new ObjectResult(result);
        }
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(ChangePassword dto)
        {
            var result = await _service.ChangePassword(dto);
            return new ObjectResult(result);
        }
        #endregion
        #region Other
        [HttpPost("login")]
        [AllowAnonymous]
        [EnableRateLimiting("login")]
        public async Task<IActionResult> Login([FromBody] Login login)
        {
            var result = await _service.Authenticate(login);
            return new ObjectResult(result);
        }
        #endregion
    }
}
