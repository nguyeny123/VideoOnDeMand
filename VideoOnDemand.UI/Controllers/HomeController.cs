using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using VideoOnDemand.Data.Data.Entities;
using VideoOnDemand.UI.Models;
using VideoOnDemand.UI.Repositories;

namespace VideoOnDemand.UI.Controllers
{
    public class HomeController : Controller
    {
        private SignInManager<User> _signInManager;
        public HomeController(SignInManager<User> signInMgr)
        {
            _signInManager = signInMgr;
        }

        public IActionResult Index()
        {
            if (!_signInManager.IsSignedIn(User))
            return RedirectToPage("/Account/Login", new { area = "Identity" });
            return RedirectToAction("Dashboard", "Membership");
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
