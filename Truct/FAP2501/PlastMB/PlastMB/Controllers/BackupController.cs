using PlastMB.Application.CustomModels.Others;
using PlastMB.Application.CustomModels.SearchConditions;
using PlastMB.Application.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PlastMB.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BackupController : BaseController
    {

        #region Properties
        private readonly IBackupService _service;
        #endregion
        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="service"></param>
        public BackupController(IBackupService service)
        {
            _service = service;
        }
        #endregion

        #region Search
        [HttpPost("search-file-backup")]
        public async Task<IActionResult> SearchFileBackup(FileBackupSearchImpl condition)
        {
            var result = await _service.SearchFileBackup(condition);
            return new ObjectResult(result);
        }
        #endregion
        #region Others
        [HttpGet("database")]
        public async Task<IActionResult> Database()
        {
            var result = await _service.Database();
            return new ObjectResult(result);
        }
        #endregion
    }
}
