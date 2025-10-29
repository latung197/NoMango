using Cody_v3.Repositories.Entities;
using Cody_v3.Services.DTOs;
using Cody_v3.Services.Helpers;
using Cody_v3.Services.Interfaces;
using Cody_v3.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using System.Data;
using System.Diagnostics;

namespace Cody_v3.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment _environment;
        private readonly IRecloserDataService _recloserDataService;
        private readonly IDepartmentService _departmentService;
        public static string subDomainUrl;

        public HomeController(ILogger<HomeController> logger, IWebHostEnvironment environment, IRecloserDataService recloserDataService, IOptions<AppSettings> settings, IConfiguration configuration, IDepartmentService departmentService)
        {
            _logger = logger;
            _environment = environment;
            _recloserDataService = recloserDataService;
            ViewBag.MyTag = settings;
            subDomainUrl = string.IsNullOrEmpty(configuration.GetValue<string>("SubDomainUrl")) ? "" : configuration.GetValue<string>("SubDomainUrl");
            _departmentService = departmentService;
        }

        public async Task<IActionResult> Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet]
        public async Task<IActionResult> OpenExcel()
        {
            return PartialView();
        }

        [HttpPost]
        public async Task<IActionResult> ImportExcel(ExcelRequestDTO dto)
        {
            var filePath = Path.Combine(_environment.WebRootPath, FileHelper.EXCEL_UPLOAD_FILE_PATH);
            var isUploaded = await _recloserDataService.Uploads(dto.ExcelFiles, filePath);
            if (isUploaded)
            {
                try
                {
                    var rowEffects = await _recloserDataService.ImportData(dto.ExcelFiles, filePath);

                    return await Statistical(null);
                }
                catch (Exception ex)
                {
                    return Ok(ex.Message);
                }
            }
            else
                return BadRequest("Can not upload the file");
        }

        //public async Task<IActionResult> Statistical(int p=1)
        //{
        //    var recloserDataDTO = await _recloserDataService.GetWithPaging(p, 1000, nameof(Statistical));
        //    return View(recloserDataDTO);
        //}

        public async Task<IActionResult> Statistical(ReportRequestDTO dto)
        {
            dto = new ReportRequestDTO()
            {
                From = DateTime.Now.AddDays(-30),
                To = DateTime.Now,
                DepartmentId = null
            };
            var lsDepartment = (await _departmentService.GetAll()).ToList();
            lsDepartment.Insert(0, new Department() { Id = Guid.Empty, DepartmentName = "--All--" });
            ViewData["lsDepartment"] = new SelectList(lsDepartment, "Id", "DepartmentName", Guid.Empty);

            return View("Statistical", dto);
        }

        public async Task<IActionResult> OverLoadReport()
        {
            ReportRequestDTO dto = null;

            dto = new ReportRequestDTO()
            {
                From = DateTime.Now.AddDays(-30),
                To = DateTime.Now,
                DepartmentId = null
            };

            var lsDepartment = (await _departmentService.GetAll()).ToList();
            lsDepartment.Insert(0, new Department() { Id = Guid.Empty, DepartmentName = "--All--" });
            ViewData["lsDepartment"] = new SelectList(lsDepartment, "Id", "DepartmentName", Guid.Empty);

            return PartialView("OverLoadReport", dto);
        }

        [HttpPost]
        public async Task<IActionResult> SratisticalSearching(ReportRequestDTO dto)
        {
            if (dto?.From == null)
            {
                dto = new ReportRequestDTO()
                {
                    From = DateTime.Now.AddDays(-30),
                    To = DateTime.Now,
                    DepartmentId = null
                };
            }           

            var data = await _recloserDataService.GetRecloserDataResponseDTOs(dto);
            return PartialView("StatisticalDetail_partial", data);
        }

        [HttpPost]
        public async Task<IActionResult> OverloadReportSearching(ReportRequestDTO dto)
        {
            if (dto?.From == null)
            {
                dto = new ReportRequestDTO()
                {
                    From = DateTime.Now.AddDays(-30),
                    To = DateTime.Now,
                    DepartmentId = null
                };
            }
            var data = await _recloserDataService.GetOverloadReportResponseDTOs(dto);
            return PartialView("OverloadReportDetail_partial", data);
        }

        [HttpPost]
        public async Task<JsonResult> UploadFile(IFormFile image)
        {
            if (image!=null)
            {
                var fileName = await _recloserDataService.UploadImage(image);
                return Json(fileName);
            }
            return Json("File not found!");
        }
    }
}