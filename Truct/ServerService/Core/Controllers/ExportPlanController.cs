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
    public class ExportPlanController : BaseController
    {
        #region Properties
        private readonly IExportPlanService _service;
        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="service"></param>
        public ExportPlanController(IExportPlanService service)
        {
            _service = service;
        }

        #endregion

        #region Search API

        [HttpPost("search-export-plan")]
        public async Task<IActionResult> SearchExportPlan(ExportPlanSearchImpl condition)
        {
            var result = await _service.SearchExportPlans(condition);
            return new ObjectResult(result);
        }

        [HttpPost("search-export-plan-pre")]
        public async Task<IActionResult> SearchExportPlanPre(ExportPlanPreSearchImpl condition)
        {
            var result = await _service.SearchExportPlanPre(condition);
            return new ObjectResult(result);
        }

        [HttpPost("search-prepare-product")]
        public async Task<IActionResult> SearchPrepareProduct(PrepareProductSearchImpl condition)
        {
            var result = await _service.SearchPrepareProduct(condition);
            return new ObjectResult(result);
        }

        [HttpPost("search-export-history-plan")]
        public async Task<IActionResult> SearchExportHistoryPlan(ExportPlanSearchImpl condition)
        {
            var result = await _service.SearchExportHistoryPlan(condition);
            return new ObjectResult(result);
        }

        [HttpGet("search-export-history-plan-by-id")]
        public async Task<IActionResult> SearchExportHistoryPlanById(int ExportplanID)
        {
            var result = await _service.SearchExportHistoryPlanById(ExportplanID);
            return new ObjectResult(result);
        }

        [HttpGet("search-export-history-plan-by-id-and-serial")]
        public async Task<IActionResult> SearchExportHistoryPlanByIdAndSerial(int ExportplanID, string BoxSerial)
        {
            var result = await _service.SearchExportHistoryPlanByIdAndSerial(ExportplanID, BoxSerial);
            return new ObjectResult(result);
        }

        [HttpGet("search-export-history-plan-by-serial")]
        public async Task<IActionResult> SearchExportHistoryPlanBySerial(string BoxSerial)
        {
            var result = await _service.SearchExportHistoryPlanBySerial(BoxSerial);
            return new ObjectResult(result);
        }

        [HttpGet("get-export-plan-by-id")]
        public async Task<IActionResult> GetExportPlanById(int ExportplanID)
        {
            var result = await _service.GetExportPlanById(ExportplanID);
            return new ObjectResult(result);
        }

        [AllowAnonymous]
        [HttpGet("get-list-export-plan-pre")]
        public async Task<IActionResult> GetListExportPlanPre()
        {
            var result = await _service.GetListExportPlanPre();
            return new ObjectResult(result);
        }

        [AllowAnonymous]
        [HttpGet("get-export-plan-stamp-info")]
        public async Task<IActionResult> GetExportPlanStampInfo(int id)
        {
            var result = await _service.GetExportPlansStampInfo(id);
            return new ObjectResult(result);
        }

        [AllowAnonymous]
        [HttpPost("get-special-stamp-info")]
        public async Task<IActionResult> GetSpecialStampInfo(SpecialStampInfo info)
        {
            var result = await _service.GetSpecialStampInfo(info);
            return new ObjectResult(result);
        }

        [HttpGet("check-box-info")]
        [AllowAnonymous]
        public async Task<IActionResult> CheckBoxInfo(string boxSerial)
        {
            var result = await _service.CheckBoxInfo(boxSerial);
            return new ObjectResult(result);
        }

        #endregion

        #region CRUD

        #region Insert

        [HttpPost("insert-export-plan")]
        public async Task<IActionResult> InsertExportPlan(ExportListPlanDto data)
        {
            var result = await _service.InsertExportPlan(data);
            return new ObjectResult(result);
        }

        [HttpPost("insert-list-export-plan")]
        public async Task<IActionResult> InsertListExportPlan(List<ExportListPlanImportDto> data)
        {
            var result = await _service.InsertListExportPlan(data);
            return new ObjectResult(result);
        }

        [HttpPost("insert-history-plan")]
        public async Task<IActionResult> InsertHistoryPlan(ExportHistoryListDto dto)
        {
            var result = await _service.InsertHistoryPlan(dto);
            return new ObjectResult(result);
        }

        [HttpPost("insert-list-warning-plan")]
        public async Task<IActionResult> InsertListWarningPlan(ImportPlan condition)
        {
            var result = await _service.InsertListWarningPlan(condition);
            return new ObjectResult(result);
        }

        [HttpPost("insert-list-export-history")]
        [AllowAnonymous]
        public async Task<IActionResult> InsertListExportHistory(List<ImportPlanHistory> lstData)
        {
            var result = await _service.InsertListExportHistory(lstData);
            return new ObjectResult(result);
        }

        /// <summary>
        /// Cập nhật thùng hàng sau khi quét Handy (Màn PC052)
        /// </summary>
        /// <param name="lstData"></param>
        /// <returns></returns>
        [HttpPost("insert-scan-box-result")]
        [AllowAnonymous]
        public async Task<IActionResult> InsertScanBoxResult(List<ExportPlanScanBoxResult> lstData)
        {
            var result = await _service.InsertScanBoxResult(lstData);
            return new ObjectResult(result);
        }

        [HttpPost("add-scan-box-result")]
        [AllowAnonymous]
        public async Task<IActionResult> AddScanBoxResult(List<ExportPlanScanBoxResult> lstData)
        {
            var result = await _service.AddScanBoxResult(lstData);
            return new ObjectResult(result);
        }

        [HttpPost("insert-scan-box-result-v2")]
        [AllowAnonymous]
        public async Task<IActionResult> InsertScanBoxV2Result(List<ExportPlanScanBoxResult> lstData)
        {
            var result = await _service.InsertScanBoxV2Result(lstData);
            return new ObjectResult(result);
        }

        [HttpPost("add-scan-box-result-v2")]
        [AllowAnonymous]
        public async Task<IActionResult> AddScanBoxV2Result(List<ExportPlanScanBoxResult> lstData)
        {
            var result = await _service.AddScanBoxV2Result(lstData);
            return new ObjectResult(result);
        }

        #endregion

        #region Update

        [HttpPost("update-export-plan")]
        public async Task<IActionResult> UpdateExportPlan(ExportListPlanDto data)
        {
            var result = await _service.UpdateExportPlan(data);
            return new ObjectResult(result);
        }

        [HttpPost("update-export-history")]
        [AllowAnonymous]
        public async Task<IActionResult> UpdateExportHistory(List<ExportHistoryListDto> lstData)
        {
            var result = await _service.UpdateListExportHistory(lstData);
            return new ObjectResult(result);
        }

        [HttpPost("update-history-plan")]
        public async Task<IActionResult> UpdateExportHistory(ExportHistoryListDto dto)
        {
            var result = await _service.UpdatetHistoryPlan(dto);
            return new ObjectResult(result);
        }

        [HttpPost("update-list-bill-state")]
        public async Task<IActionResult> UpdateListBillState(List<UpdateStatus> lstData)
        {
            var result = await _service.UpdateListBillState(lstData);
            return new ObjectResult(result);
        }

        [HttpPost("update-prepare-product-order-state")]
        public async Task<IActionResult> UpdatePrepareProductOrderState(List<UpdateStatus> lstData)
        {
            var result = await _service.UpdatePrepareProductOrderState(lstData);
            return new ObjectResult(result);
        }

        [HttpPost("update-prepare-boxinfo-order-state")]
        public async Task<IActionResult> UpdatePrepareBoxInfoOrderState(List<UpdateStatus> lstData)
        {
            var result = await _service.UpdatePrepareBoxInfoOrderState(lstData);
            return new ObjectResult(result);
        }

        #endregion

        #region Delete

        [HttpDelete("delete-export-plan")]
        public async Task<IActionResult> DeleteExportHistory(int ExportPlanID)
        {
            var result = await _service.DeleteExportPlan(ExportPlanID);
            return new ObjectResult(result);
        }

        [HttpDelete("delete-box-info")]
        public async Task<IActionResult> DeleteBoxInfo(int BoxID)
        {
            var result = await _service.DeleteBoxInfo(BoxID);
            return new ObjectResult(result);
        }

        #endregion

        #endregion

        #region Others
        #endregion
    }
}
