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
using PuduIOT.Models.Robots.ListPoint;
using PuduIOT.Models.Robots.Cancel;
using System.Text.Json;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace PuduIOT.Controllers
{
    [KeepAlive]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IMstConfigurationService _configurationService;
        private readonly IMstRobotInfoService _mstRobotInfoService;
        private readonly IPuduApiService _puduApiService;

        private readonly IHubContext<RealTimeHub> _hubContext;

        public HomeController(ILogger<HomeController> logger, IMstConfigurationService configurationService, IMstRobotInfoService mstRobotInfoService, IHubContext<RealTimeHub> hubContext)
        {
            _configurationService = configurationService;
            _logger = logger;
            _mstRobotInfoService = mstRobotInfoService;
            _hubContext = hubContext;
        }

        public async Task<IActionResult> Index()
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

        [HttpGet("/reCharge")]
        public async Task<IActionResult> ReCharge(string sn)
        {

            var robot = await _mstRobotInfoService.ReCharge(sn);
            if (robot == null)
                return NotFound();

            return Json(new
            {
                imgName = robot
            }); ;
        }

        [HttpGet("/getListPoint")]
        public async Task<IActionResult> GetListPoint(string sn)
        {
            var response = await _mstRobotInfoService.GetListPoint(sn);
            if (response == null)
                return NotFound();
            return Ok(response);
        }

        [HttpPost("/custom_call")]
        public async Task<IActionResult> CustomCall([FromBody] CustomCallRequest request)
        {
            if (request == null)
                return Ok();
            var response = _mstRobotInfoService.CustomCall(request.Sn, request.MapName, request.PointName, request.PointType);
            if (response == null) return NotFound();
            return Ok(response);
        }

        [HttpPost("/cancelTask")]
        public async Task<IActionResult> CancelTask([FromBody] CancelTaskRequest request)
        {
            if (request == null)
                return Ok();
            var response = _mstRobotInfoService.CancelTask(request.taskId);
            if (response == null) return NotFound();
            return Ok(response);
        }

        [HttpPost("/callback")]
        public async Task<IActionResult> Callback([FromBody] JsonElement payload)
        {
            try
            {

                var callbackType = payload.GetProperty("callback_type").GetString();
                var data = payload.GetProperty("data").GetRawText();
                switch (callbackType)
                {
                    case "notifyRobotMoveState":
                        // Ví dụ state = MOVING / IDLE / APPROACHING …
                        var moveState = payload.GetProperty("data").GetProperty("state").GetString();
                        //await _hubContext.Clients.All.SendAsync("RobotCustomCallUpdate1", data);
                        break;

                    case "notifyCustomCall":
                        // Ví dụ state = CALL_COMPLETE / ARRIVE …
                        var callState = payload.GetProperty("data").GetProperty("state").GetString();
                        var point = payload.GetProperty("data").GetProperty("point").GetString();
                        await _hubContext.Clients.All.SendAsync("RobotCustomCallUpdate", data);
                        break;

                    default:
                        break;
                }
                return Ok(new { message = "Received" });
            }
            catch (Exception ex)
            {
                // Bắt mọi lỗi và trả lại 200 thay vì 500
                Console.WriteLine($"Lỗi callback: {ex}");
                return Ok(new { message = "Received with error", error = ex.Message });
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}