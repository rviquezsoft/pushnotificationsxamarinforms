using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiPushNotifications.Servicios;
using ControladorModelos;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ApiPushNotifications.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Notificacion : ControllerBase
    {
       
        [HttpPost]
        [Route("registrar")]
        public async Task<ActionResult<string>> Registrar([FromBody] PushFCMBody push)
        {
            string resultado = "";
            using (var manager=new NotificacionesManager())
            {
                resultado= await manager.registrar(push);
            }
            return Ok(resultado);
            
        }

        [HttpPost]
        [Route("enviar")]
        public async Task<ActionResult<string>> Enviar([FromBody] Push.PushNotificacionAF push)
        {
            string resultado = "";
            using (var manager = new NotificacionesManager())
            {
                resultado = await manager.enviar(push);
            }
            return Ok(resultado);

        }




    }
}
