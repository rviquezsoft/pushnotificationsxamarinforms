
using ControladorModelos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace ApiPushNotifications.Servicios
{
    public class NotificacionesManager:IDisposable
    {
        private  ConexionesBD.Payloads Payloads;

        private const string urlGoogle = "https://fcm.googleapis.com/fcm/send";

        public NotificacionesManager()
        {
            Payloads = new ConexionesBD.Payloads();
        }

        //registrar un dispositivo sea android o ios
        public async Task<string> registrar(PushFCMBody pushFCM)
        {

            try
            {

                int result = 0;

                if (!string.IsNullOrWhiteSpace(pushFCM.TOKEN))
                {
                    Dictionary<string, object> parametros = new Dictionary<string, object>();

                    parametros.Add("@email", pushFCM.EMAIL);

                    parametros.Add("@token", pushFCM.TOKEN);

                    parametros.Add("@plataforma", pushFCM.PLATAFORMA);

                    parametros.Add("@bundleid", pushFCM.BUNDLEID);

                    parametros.Add("@apikey", pushFCM.APIKEY);

                     result = await Payloads.InsertOrUpdateOrDeleteDatabase(Startup._Conexxion, MapaConsultas.registro, parametros);
                }


                return  result==1?"registrado": "error";
            }
            catch (Exception r)
            {

                return "error";
            }


        }


        //se Notifica a un usuario en específico de acuerdo al email
        public async Task<string> enviar(Push.PushNotificacionAF pnotification)
        {
            try
            {
                //si el email es igual a "99" se envia a todos los dispositivos, sino solo a el email recibido
                List<PushFCMBody> registros =
                    (pnotification ?? new Push.PushNotificacionAF()).email?.ToLowerInvariant()
                    == "99" ? await recuperarIDS(pnotification?.email ?? "",true):
                    await recuperarIDS(pnotification?.email ?? "");

                
                if (registros.Count>0)
                {
                    List<string> tokens = new List<string>();

                    foreach (var item in registros)
                    {
                        tokens.Add(item.TOKEN);
                    }

                    Push.Notification notificacion = pnotification.notification;


                    List<SoftlandPaginacion.PaginacionObjeto> paginados = null;
                    if (tokens.Count>998)
                    {
                        paginados= SoftlandPaginacion.Paginacion.Paginar(tokens.Count,998);

                        foreach (var pagina in paginados)
                        {
                            var listado = tokens.Skip((int)pagina.Inicio).Take((int)pagina.CantidadRegistros)?.ToList();

                            Push.PushNotification pushNotification = new Push.PushNotification
                            {
                                registration_ids =listado,
                                notification = notificacion
                            };

                            bool envio = await enviarFirebase(pnotification.firebaseKey, pushNotification);
                        }
                      
                    }
                    else
                    {
                        Push.PushNotification pushNotification = new Push.PushNotification
                        {
                            registration_ids = tokens,
                            notification = notificacion
                        };

                        bool envio = await enviarFirebase(pnotification.firebaseKey, pushNotification);
                    }

                 
                }



                return "enviado";
            }
            catch (Exception e)
            {
                return "error";
            }

        }

        /// <summary>
        /// Consulta los tokens en la base de datos
        /// </summary>
        /// <param name="correo"></param>
        /// <param name="todos"></param>
        /// <returns></returns>
        private async Task<List<PushFCMBody>> recuperarIDS(string correo,bool todos = false)
        {
            List<PushFCMBody> lista = new List<PushFCMBody>();

            Dictionary<string, object> pairs = new Dictionary<string, object>();

            pairs.Add("@correo",correo);

            try
            {
                if (todos)
                {
                    List<Dictionary<string, object>> respuesta =
                               await Payloads.SelectFromDatabaseDiccionary(Startup._Conexxion, MapaConsultas.enviarTodos);



                    respuesta.ForEach((x) => {
                        lista.Add(new PushFCMBody { 
                        
                            EMAIL=x["EMAIL"]?.ToString(),
                            PLATAFORMA= x["PLATAFORMA"]?.ToString(),
                            TOKEN= x["TOKEN"]?.ToString()
                        });
                    });
                  
                }
                else
                {
                    List<Dictionary<string, object>> respuesta =
                             await Payloads.SelectFromDatabaseDiccionary(Startup._Conexxion, MapaConsultas.envio,pairs);

                    respuesta.ForEach((x) => {
                        lista.Add(new PushFCMBody
                        {

                            EMAIL = x["EMAIL"]?.ToString(),
                            PLATAFORMA = x["PLATAFORMA"]?.ToString(),
                            TOKEN = x["TOKEN"]?.ToString()
                        });
                    });
                }

            }
            catch (Exception r)
            {
            }

            return lista;
        }



        /// <summary>
        /// Envia el push a servidores de Google
        /// </summary>
        /// <param name="tokens"></param>
        /// <param name="keyFirebase"></param>
        /// <param name="push"></param>
        /// <returns></returns>
        public async Task<bool> enviarFirebase(string keyFirebase,Push.PushNotification push)
        {
            try
            {
                await Task.Run(()=> {

                    var client = new RestClient(urlGoogle);
                    
                    var request = new RestRequest(Method.POST);

                    request.AddHeader("Authorization",$"key={keyFirebase}");
                    
                    request.AddHeader("Content-Type", "application/json");

                    string json = JsonConvert.SerializeObject(push, Formatting.Indented);

                    request.AddParameter("application/json", json, ParameterType.RequestBody);

                    IRestResponse response = client.Execute(request);

                });

                return true;
            }
            catch (Exception v)
            {
                return false;
            }
        }

        public void Dispose()
        {
            Payloads = null;
        }
    }
}
