using PuduIOT.BL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol;
using static System.Reflection.Metadata.BlobBuilder;
using System.Data;
using PuduIOT.Models;
using System.Globalization;
using System.Net.Mime;
using PuduIOT.BL.Commons;
using PuduIOT.DA.Middleware;

namespace PuduIOT.Controllers
{
    [KeepAlive]
    public class AirCompressorController : Controller
    {
        private readonly IBreadcrumbService _breadcrumbService;
        private readonly ILibs _libs;
        private readonly IMstUnitService _unitService;
        public AirCompressorController(IBreadcrumbService breadcrumbService, IMstUnitService unitService, ILibs libs)
        {
            _libs = libs;
            _unitService = unitService;
            _breadcrumbService = breadcrumbService;
        }
        public IActionResult Visualization()
        {
            var units = _unitService.GetUnitsByType(Constants.COMPRESSOR_TYPE);
            ViewBag.Resource = "air-visual";
            _breadcrumbService.AddBreadcrumbItem("COM-LBL-007", "");
            _breadcrumbService.AddBreadcrumbItem("COM-LBL-003", "");
            var data = new
            {
                Units = units,
            };


            return View(data);
        }
        public IActionResult Dashboard()
        {
            ViewBag.Title = "Dashboard";
            ViewBag.Resource = "air-dashboard";
            _breadcrumbService.AddBreadcrumbItem("COM-LBL-007", "");
            _breadcrumbService.AddBreadcrumbItem("COM-LBL-005", "");

            var units = _unitService.GetUnitsByType(Constants.COMPRESSOR_TYPE);
            var data = new
            {
                Units = units,
            };


            return View(data);
        }
        [HttpGet("[controller]/search_data")]
        public IActionResult SearchData(int type, string units, DateTime startTime, DateTime endTime, int eDashboard)
        {
            try
            {

                DataTable dtData = _libs.SearchDataApi(type, units, startTime, endTime, eDashboard);
                var data = new
                {
                    DataChart = dtData,
                };
                return Ok(data.ToJson());
            }
            catch (Exception ex)
            {
                // Xử lý lỗi và trả về lỗi 500 Internal Server Error

                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpGet("[controller]/data_real_time")]
        public IActionResult GetDataRealTime(string units, int eDashboard)
        {
            try
            {
                string dataJson = _libs.LoadDataApi(units, eDashboard);

                return Ok(dataJson);
            }
            catch (Exception ex)
            {
                // Xử lý lỗi và trả về lỗi 500 Internal Server Error
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }



        [HttpPost("[controller]/update_unit")]
        public IActionResult UpdateUnit(StandardValue data)
        {
            try
            {

                decimal result = decimal.Parse(data.Value, CultureInfo.InvariantCulture);
                _unitService.UpdateUnit(data.Id, result);

                return StatusCode(200);
            }
            catch (Exception ex)
            {
                // Xử lý lỗi và trả về lỗi 500 Internal Server Error
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpPost("[Controller]/export-data-PDF")]
        public IActionResult ExportDataPDF(MyData mdata)
        {
            byte[] fileContents = { };
            try
            {
                fileContents = _libs.ExportPdfApi(mdata);

            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }

            return File(fileContents, MediaTypeNames.Application.Pdf);

        }

        [HttpPost("[Controller]/export-data-CSV")]
        public IActionResult ExportDataCSV(MyData mdata)
        {
            byte[] fileContents = { };
            try
            {

                fileContents = _libs.ExportCsvApi(mdata);

            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }


            return File(fileContents, "text/csv");

        }
    }
}
