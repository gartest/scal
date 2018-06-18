using SCAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace SCAL.Controllers
{
    [RoutePrefix("api")]
    public class APIController : ApiController
    {
        [Route("users")]
        [HttpGet]
        public async Task<List<Usuario>> GetUsuariosAsync()
        {
            return await UsuariosModel.getUsuarios();
        }
        [Route("users/{id:int}")]
        [HttpGet]
        public async Task<Usuario> GetUsuariosAsync(int id)
        {
            var res = await UsuariosModel.getUsuarios();
            return res.Where(u => u.id == id).FirstOrDefault();
        }
        [Route("guardaridtarjeta/{id}")]
        [HttpGet]
        public async Task<string> GuardarIdTarjeta(string id)
        {
            return await TarjetaModel.GuardarIdTarjeta(id);
        }
        [Route("salas_status")]
        [HttpGet]
        public List<SalaStatus> GetSalaStatuses()
        {
            return SalasModel.GetSalasStatus();
        }
        [Route("salas")]
        [HttpPost]
        public string SetSala(Sala sala)
        {
            return SalasModel.SetSala(sala);
        }
        [Route("salas/{id:int}")]
        [HttpDelete]
        public string DeleteSala(int id)
        {
            return SalasModel.BorrarSala(id);
        }
    }
}
