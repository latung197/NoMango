using Core.Application.CustomModels.Dtos;
using Core.Application.CustomModels.Others;
using Core.Application.CustomModels.SearchConditions;
using Core.Application.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Core.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserController : BaseController
    {
        #region Properties
        private readonly IMstUserService _service;
        #endregion
        #region Constructor
        public UserController(IMstUserService service)
        {
            _service = service;
        }

        #endregion
        #region Search
        [HttpPost("search-user")]
        public async Task<IActionResult> SearchUser(MstUserSearchImpl condition)
        {
            var result = await _service.SearchUser(condition);
            return new ObjectResult(result);
        }
        [HttpGet("get-user-by-id")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var result = await _service.GetUserById(id);
            return new ObjectResult(result);
        }
        #endregion
        #region CRUD
        [HttpPost("insert-user")]
        public async Task<IActionResult> InsertUser(MstUserDto dto)
        {
            var result = await _service.InsertUser(dto);
            return new ObjectResult(result);
        }
        [HttpPost("update-user")]
        public async Task<IActionResult> UpdateUser(MstUserDto dto)
        {
            var result = await _service.UpdateUser(dto);
            return new ObjectResult(result);
        }
        [HttpDelete("delete-user")]
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
        public async Task<IActionResult> Login([FromBody] Login login)
        {
            var result = await _service.Authenticate(login);
            return new ObjectResult(result);
        }
        #endregion
    }
}
