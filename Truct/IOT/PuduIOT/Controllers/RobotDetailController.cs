using Microsoft.AspNetCore.Mvc;
using PuduIOT.BL.Interfaces;

namespace PuduIOT.Controllers
{
    public class RobotDetailController : Controller
    {
        private readonly IMstRobotInfoService _mstRobotInfoService;
        public RobotDetailController(IMstRobotInfoService statRobotInfoService)
        {
            _mstRobotInfoService = statRobotInfoService;
        }
        public async Task< IActionResult> Index(string id)
        {
            var robot = await _mstRobotInfoService.GetById(id);
            return View(robot);
        }
    }
}
