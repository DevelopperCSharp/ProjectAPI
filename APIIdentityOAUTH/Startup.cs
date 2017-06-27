using System;
using System.Linq;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using System.Web.Http;
using APIIdentityOAUTH.Infrastructure;
using Microsoft.Owin;
using Newtonsoft.Json.Serialization;
using Owin;

[assembly: OwinStartup(typeof(APIIdentityOAUTH.Startup))]

namespace APIIdentityOAUTH
{
    // classe Owin "Démarrage" qui sera déclenchée une fois que notre serveur démarrera.
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=316888

            HttpConfiguration httpConfig=new HttpConfiguration();

            ConfigureOAuthTokenGeneration(app);
            ConfigureWebApi(httpConfig);
           
            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
            app.UseWebApi(httpConfig);


        }

        /*nous créons une nouvelle instance à partir de "ApplicationDbContext" et "ApplicationUserManager" pour chaque requête 
        *et la configurons dans le contexte Owin à l'aide de la méthode d'extension "CreatePerOwinContext". 
        *Les deux objets (ApplicationDbContext et AplicationUserManager) seront disponibles pendant toute la durée de la demande.
        */
        private void ConfigureOAuthTokenGeneration(IAppBuilder app)
        {
            // Configure the db context and user manager to use a single instance per request
            app.CreatePerOwinContext(ApplicationDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);

            // Plugin the OAuth bearer JSON Web Token tokens generation and Consumption will be here

        }



        private void ConfigureWebApi(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();

            var jsonFormatter = config.Formatters.OfType<JsonMediaTypeFormatter>().First();
            jsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
        }
    }
}
