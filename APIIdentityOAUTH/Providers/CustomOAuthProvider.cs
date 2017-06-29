using System.Security.Claims;
using System.Threading.Tasks;
using APIIdentityOAUTH.Infrastructure;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;

namespace APIIdentityOAUTH.Providers
{

    // Implement Authorization avec le jeton JWT
    public class CustomOAuthProvider:OAuthAuthorizationServerProvider
    {
        //Comme vous remarquez que " ValidateClientAuthentication " est vide,
        //nous considérons la demande toujours valide, car dans notre implémentation,
        //notre client (AngularJS front-end) est un client de confiance et nous n'avons pas besoin de le valid
        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
            return Task.FromResult<object>(null);
        }

        //La méthode " GrantResourceOwnerCredentials " est responsable de recevoir le nom d'utilisateur et le mot de passe de la demande 
        //et de les valider par rapport à notre système d'identité ASP.NET 2.1
        public override  async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            var allowedOrigin = "*";
            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { allowedOrigin });

            // si les informations d'identification sont valides et que le courrier électronique est confirmé,
            //nous créons une identité pour l'utilisateur connecté

            var userManager = context.OwinContext.GetUserManager<ApplicationUserManager>();
            ApplicationUser user = userManager.FindByName(context.UserName);
            //await userManager.FindAsync(context.UserName, context.Password);

            if (user == null)
            {
                context.SetError("Invalid grant","the user name or pass is invalid");
                return;
            }
            if (!user.EmailConfirmed)
            {
               context.SetError("Invalid_grant","User is not confirm email"); 
            }

            //Cette méthode sera responsable de récupérer l'identité d'utilisateur authentifiée à partir de la base de données 
            //et renvoie un objet du type "ClaimIdentity"

            ClaimsIdentity OAuthIdentity = await user.GenerateUserIdentityAsync(userManager, "JWT");

            //nous créons un ticket d'authentification qui contient l'identité de l'utilisateur authentifié,
            //et lorsque nous appelons "contextValidated (ticket)", cela transférera cette identité vers un jeton d'accès au porteur OAuth 2.
            var ticket =new AuthenticationTicket(OAuthIdentity,null);
            context.Validated(ticket);


        }
    }
}