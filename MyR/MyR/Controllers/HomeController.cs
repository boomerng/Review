using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyR.Models;

namespace MyR.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            HomeModel model = new HomeModel(User.Identity.Name);

            return View(model);
        }
    }
}
