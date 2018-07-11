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
        [Route("users")]
        [HttpPost]
        public async Task<string> SetUsuarioAsync(Usuario usuario)
        {       
            var res = await UsuariosModel.SetUsuarioAsync(usuario);
            await ActualizaPermisosSalasAsync();
            return res;
        }
        [Route("users/{id:int}")]
        [HttpDelete]
        public async Task<string> DeleteUsuarioAsync(int id)
        {
            var res = await UsuariosModel.BorrarUsuarioAsync(id);
            await ActualizaPermisosSalasAsync();
            return res;
        }
        [Route("users/{id:int}")]
        [HttpGet]
        public async Task<Usuario> GetUsuariosAsync(int id)
        {
            var res = await UsuariosModel.getUsuarios();
            return res.Where(u => u.id == id).FirstOrDefault();
        }
        [Route("guardaridtarjeta/{ip}/{id_tarjeta}/{resultado}")]
        [HttpGet]
        public async Task<string> GuardarIdTarjeta(string ip, string id_tarjeta, string resultado)
        {
            return await TarjetaModel.GuardarLogRFID(ip, id_tarjeta, resultado);
        }
        [Route("log")]
        [HttpGet]
        public async Task<List<Log>> GetLogsAsync()
        {
            return await TarjetaModel.GetLogsAsync();
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
        [Route("encenderluz")]
        [HttpGet]
        public string EncenderLuz(string ip)
        {
            return SalasModel.EncenderLuz(ip);
        }
        [Route("apagarluz")]
        [HttpGet]
        public string ApagarLuz(string ip)
        {
            return SalasModel.ApagarLuz(ip);
        }
        [NonAction]
        public async Task ActualizaPermisosSalasAsync()
        {
            var salas = await SalasModel.GetSalas();
            foreach(var s in salas)
            {
                var tarjetasPermitidas = SalasModel.GetTarjetasPermitidas(s.ip);
                SalasModel.SetPermisos(s.ip, tarjetasPermitidas);
            }
        }
    }
}
