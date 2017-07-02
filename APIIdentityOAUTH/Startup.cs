using System;
using System.Configuration;
using System.Linq;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using System.Web.Http;
using APIIdentityOAUTH.Formats;
using APIIdentityOAUTH.Infrastructure;
using APIIdentityOAUTH.Providers;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataHandler.Encoder;
using Microsoft.Owin.Security.Jwt;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json.Serialization;
using Owin;

//Install-Package Microsoft.Owin.Security.Jwt -Version 3.0.0
//"Microsoft.Owin.Security.Jwt" est responsable de la protection des ressources du serveur de ressources
//en utilisant JWT,




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
            ConfigureOAuthTokenConsumption(app);

            ConfigureWebApi(httpConfig);
           
            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
            app.UseWebApi(httpConfig);


        }

        /*nous créons une nouvelle instance à partir de "ApplicationDbContext" et "ApplicationUserManager" pour chaque requête 
        *et la configurons dans le contexte Owin à l'aide de la méthode d'extension "CreatePerOwinContext". 
        *Les deux objets (ApplicationDbContext et AplicationUserManager) seront disponibles pendant toute la durée de la demande.
        */

        //The path for generating JWT will be as :”http://localhost:59822/oauth/token”.
        private void ConfigureOAuthTokenGeneration(IAppBuilder app)
        {
            // Configure the db context and user manager and role  to use a single instance per request
            app.CreatePerOwinContext(ApplicationDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            //affecter la classe de gestion de rôle au contexte d'Owin
            app.CreatePerOwinContext<ApplicationRoleManager>(ApplicationRoleManager.Create);




            OAuthAuthorizationServerOptions OAuthServerOptions = new OAuthAuthorizationServerOptions()
            {
                //For Dev enviroment only (on production should be AllowInsecureHttp = false)
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/oauth/token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(1),
               
                //We’ve specified the implementation on how to validate the Resource owner user credential
                //in a custom class named “CustomOAuthProvider”
                Provider = new CustomOAuthProvider(),
                //implementation on how to generate the access token using JWT formats
                AccessTokenFormat = new CustomJwtFormat("http://localhost:54152")
            };

            // OAuth 2.0 Bearer Access Token Generation
            app.UseOAuthAuthorizationServer(OAuthServerOptions);
        }




        ////Install-Package Microsoft.Owin.Security.Jwt -Version 3.0.0

        //Cette étape configurera notre API sur les jetons de confiance émis uniquement par notre serveur d'autorisation,
        //dans notre cas, l'Autorisation et le Serveur de ressources sont le même serveur (http: // localhost: 59822),
        //remarquez comment nous fournissons les valeurs pour le public et le 
        //Le secret d'audience que nous avions utilisé pour générer et émettre le jeton Web JSON à l'étape 3.

        // En fournissant ces valeurs au middleware "JwtBearerAuthentication", 
        //notre API ne pourra consommer que les jetons JWT émis par notre serveur d'autorisation approuvé,
        //tous les autres jetons JWT provenant de tout autre serveur d'autorisation seront rejetés.

        private void ConfigureOAuthTokenConsumption(IAppBuilder app)
        {
            var issuer = "http://localhost:54152";
            string audienceId = ConfigurationManager.AppSettings["as:AudienceId"];
            byte[] audienceSecret = TextEncodings.Base64Url.Decode(ConfigurationManager.AppSettings["as:AudienceSecret"]);

           
            // Api controllers with an [Authorize] attribute will be validated with JWT
            app.UseJwtBearerAuthentication(
                new JwtBearerAuthenticationOptions
                {
                    AuthenticationMode = AuthenticationMode.Active,
                    AllowedAudiences = new[] { audienceId },
                    IssuerSecurityTokenProviders = new IIssuerSecurityTokenProvider[]
                    {
                        new SymmetricKeyIssuerSecurityTokenProvider(issuer, audienceSecret)
                    },

                    //Provider = new OAuthBearerAuthenticationProvider
                    //{
                    //    OnValidateIdentity = context =>
                    //    {
                    //        context.Ticket.Identity.AddClaim(new System.Security.Claims.Claim("newCustomClaim", "newValue"));
                    //        return Task.FromResult<object>(null);
                    //    }
                    //}
                });


        }




        private void ConfigureWebApi(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();

            var jsonFormatter = config.Formatters.OfType<JsonMediaTypeFormatter>().First();
            jsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
        }
    }
}
