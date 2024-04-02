using Microsoft.AspNetCore.Mvc;
using SafeIPList.Models;
using System.Diagnostics;
using System.Reflection;

namespace SafeIPList.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public ActionResult Index()
        {
            var ipAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString();

            ViewData["IpAddress"] = "My IP : "+ipAddress;


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
    }
}
