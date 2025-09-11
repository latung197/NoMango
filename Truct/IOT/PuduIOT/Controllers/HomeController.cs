using PuduIOT.BL;
using PuduIOT.BL.Commons;
using PuduIOT.BL.Interfaces;
using PuduIOT.DA.Middleware;
using PuduIOT.Models;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol;
using System.Data;
using System.Diagnostics;
using System.Globalization;

namespace PuduIOT.Controllers
{
    [KeepAlive]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IMstConfigurationService _configurationService;
        private readonly IMstRobotInfoService _mstRobotInfoService;

        public HomeController(ILogger<HomeController> logger, IMstConfigurationService configurationService, IMstRobotInfoService mstRobotInfoService)
        {
            _configurationService = configurationService;
            _logger = logger;
            _mstRobotInfoService = mstRobotInfoService;
        }

        public async Task <IActionResult> Index()
        {
            var robotInfors = await _mstRobotInfoService.GetAll();
            ViewBag.Resource = "pac-visual";
            var data = new
            {
                RobotInfors = robotInfors,
            };
            return View(data);
        }


        [HttpGet]
        public async Task<IActionResult> GetRobotDetails(string sn)
        {
            var robot = await _mstRobotInfoService.GetById(sn); // Lấy dữ liệu từ DB
            if (robot == null)
                return NotFound();

            return Json(new
            {
                sn = robot.Sn,
                name = robot.Name,
                companyId = robot.CompanyId,
                companyName = robot.CompanyName,
                imgName = robot.ImgName
            });
        }

        [HttpGet("/changing")]
        public async Task<IActionResult> Charging(string sn)
        {
            
            var robot = await _mstRobotInfoService.GetById(sn); 
            if (robot == null)
                return NotFound();

            return Json(new
            {
                imgName = robot.ImgName
            });
        }


        [HttpPost("/update-electric-cost")]
        public IActionResult UpdateUnit(MstConfiguration data)
        {
            try
            {
                _configurationService.UpdateValueByKey(data.Key, data.Value);

                return StatusCode(200);
            }
            catch (Exception ex)
            {
                // Xử lý lỗi và trả về lỗi 500 Internal Server Error
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}