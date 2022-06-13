using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ControladorModelos;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace ApiPushNotifications
{
    public class Startup
    {
        public static ControladorModelos.Connexion _Conexxion;
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            _Conexxion = new Connexion();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            _Conexxion.Servidor = Configuration["Contexto:Server"];

            _Conexxion.BaseDatos = Configuration["Contexto:Bd"];

            _Conexxion.Usuario = Configuration["Contexto:User"];

            _Conexxion.Clave = Configuration["Contexto:Password"];

            _Conexxion.Tipo = tipo.SQL;


            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Push API", Version = "v1" });
                c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
            });


            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
           
           app.UseDeveloperExceptionPage();
         
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

           
            app.UseSwagger();


            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("../swagger/v1/swagger.json", "Push API");
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
