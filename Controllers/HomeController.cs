using Cab_Management_System.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Cab_Management_System.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                if (User.IsInRole("Admin"))
                    return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
                if (User.IsInRole("FinanceManager"))
                    return RedirectToAction("Index", "Dashboard", new { area = "Finance" });
                if (User.IsInRole("HRManager"))
                    return RedirectToAction("Index", "Dashboard", new { area = "HR" });
                if (User.IsInRole("TravelManager"))
                    return RedirectToAction("Index", "Dashboard", new { area = "Travel" });
            }
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
