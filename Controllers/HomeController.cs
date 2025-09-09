using System.Diagnostics;
using Contract_Montly_Claim_System.Models;
using Microsoft.AspNetCore.Mvc;

namespace Contract_Montly_Claim_System.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult LecturerDashboard()
        {
            return View();
        }

        public IActionResult CoordinatorDashboard()
        {
            return View();
        }

        public IActionResult ManagerDashboard()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        public IActionResult SubmitClaim()
        {
            return View();
        }

        public IActionResult TrackClaims()
        {
            return View();
        }

        public IActionResult CoordinatorApproval()
        {
            return View();
        }

        public IActionResult ManagerApproval()
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
    }
}
