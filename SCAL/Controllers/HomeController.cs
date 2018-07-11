using SCAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        public async Task<ActionResult> Users()
        {
            ViewBag.Rooms = await SalasModel.GetSalas();
            ViewBag.Users = await UsuariosModel.getUsuarios();
            await ActualizaPermisosSalasAsync();
            return View();
        }
        [NonAction]
        public async Task ActualizaPermisosSalasAsync()
        {
            var salas = await SalasModel.GetSalas();
            foreach (var s in salas)
            {
                var tarjetasPermitidas = SalasModel.GetTarjetasPermitidas(s.ip);
                SalasModel.SetPermisos(s.ip, tarjetasPermitidas);
            }
        }
    }
}