using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;

namespace SyncTube.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Lobby");
            }

            return View();
        }

        public IActionResult Error()
        {
            return View("~/Views/Shared/Error.cshtml");
        }

        //        public IActionResult About()
        //        {
        //            ViewData["Message"] = "Your application description page.";
        //
        //            return View();
        //        }
        //
        //        public IActionResult Contact()
        //        {
        //            ViewData["Message"] = "Your contact page.";
        //
        //            return View();
        //        }
    }
}
