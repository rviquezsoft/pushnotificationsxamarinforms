using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiPushNotifications.Servicios
{
    public class MapaConsultas
    {
        public static string registro { get; set; } = "INSERT INTO [dbo].[REGISTRO] ([EMAIL]" +
            " ,[TOKEN] ,[PLATAFORMA] ,[BUNDLEID] ,[APIKEY])" +
            " VALUES (@email ,@token" +
            " ,@plataforma ,@bundleid ,@apikey)";

        public static string envio { get; set; } = $"select EMAIL,TOKEN,PLATAFORMA from REGISTRO where lower(EMAIL)= lower(@correo)";

        public static string enviarTodos { get; set; } = $"select EMAIL,TOKEN,PLATAFORMA from REGISTRO";
    }
}
