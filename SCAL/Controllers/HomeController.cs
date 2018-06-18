using SCAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SCAL.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Rooms()
        {
            var model = SalasModel.GetSalasStatus();
            return View(model);
        }
        public ActionResult Users()
        {
            return View();
        }
    }
}