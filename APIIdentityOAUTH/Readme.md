// Configuration Asp.Net API WEB 2 + serveur OWIN + ASP Identity +OAUTH 2

Etape 1: Installer les packages suivantes:

---------------------------------------------------------------
/* Configuration AspNet.Identity:needed NuGet packages to add support for registering and validating user credentials, */

Install-Package Microsoft.AspNet.Identity.Owin -Version 2.1.0
Install-Package Microsoft.AspNet.Identity.EntityFramework -Version 2.1.0
------------------------------------------------------------------
/* Configuration Owin*/

Install-Package Microsoft.Owin.Host.SystemWeb -Version 3.0.0
Install-Package Microsoft.AspNet.WebApi.Owin -Version 5.2.2
-------------------------------------------------------------------------*
/* Configuration OAuth*/

Install-Package Microsoft.Owin.Security.OAuth -Version 3.0.0
-------------------------------------------------------------------------------
Install-Package Microsoft.Owin.Cors -Version 3.0.0

  Startup:app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
--------------------------------------------------------------------------------

Etape2: /* Add Owin “Startup” Class*/
         - Adding class startup
		 -cette classe sera déclenchée une fois que notre serveur commencera.
		 - notez l'attribut "assembly" qui indique la classe à déclencher lors du démarrage.
		 -La méthode "Configuration" accepte le paramètre du type "IAppBuilder".
		 -Ce paramètre sera fourni par l'hôte au moment de l'exécution. 
		 -Ce paramètre "app" est une interface qui sera utilisée pour composer l'application pour notre serveur Owin.
         -L'objet "HttpConfiguration" sert à configurer les chemins de l'API.
		 -Nous passons donc cet objet à la méthode "Enregistrer" dans la classe "WebApiConfig".
		 -Enfin, nous passerons l'objet "config" à la méthode d'extension "UseWebApi" qui sera responsable de transférer
		 l'API Web ASP.NET vers notre pipeline du serveur Owin.

sing Microsoft.Owin;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;


[assembly: OwinStartup(typeof(APIDataAcess.Startup))]

namespace APIDataAcess
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=316888
            HttpConfiguration config = new HttpConfiguration();
            WebApiConfig.Register(config);
            app.UseWebApi(config);
        }
    }
}
------------------------------------------------------------------------------------------------------------------------------------

Etape 3://ASPNET .Identity
              (A):  -  un contexte de base de données d'application 
				- methode create sera appeler par la classe startup de owin
				 public ApplicationContext():base("AuthentificationUser")
					{
						Configuration.ProxyCreationEnabled = false;
						Configuration.LazyLoadingEnabled = false;
          
					}

					public static ApplicationContext Create() => new ApplicationContext();
----------------------------------------------------------------------------------------------
            (B):ajouter "UserModel" qui contient les propriétés à envoyer une fois que nous enregistrons un utilisateur
			
			public class UserModel dans notre cas c'est User ou un user herite de IdentityUser
				{
					[Required]
					[Display(Name = "User name")]
					public string UserName { get; set; }
 
					[Required]
					[StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
					[DataType(DataType.Password)]
					[Display(Name = "Password")]
					public string Password { get; set; }
 
					[DataType(DataType.Password)]
					[Display(Name = "Confirm password")]
					[Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
					public string ConfirmPassword { get; set; }
				}
				------------------------------------------------------------------------------------------------------------
				(C): Add the ConnectionString in web.config

				<connectionStrings>
				 <add name="AuthContext"  providerName="System.Data.SqlClient" connectionString="Data Source=.\sqlexpress;Initial Catalog=AngularJSAuth;Integrated Security=SSPI;" providerName="System.Data.SqlClient" />
				 </connectionStrings>
				-------------------------------------------------------------------------------------------------------------------
				(D): Add the ConnectionString in web.config
						-Enable Migrations
						-Add Migration CreateIdentityUser
						- Remplir seed
						- update-Database

						protected override void Seed(AspNetIdentity.WebApi.Infrastructure.ApplicationDbContext context)
							{
								//  This method will be called after migrating to the latest version.
 
								var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
 
								var user = new ApplicationUser()
								{
									UserName = "SuperPowerUser",
									Email = "taiseer.joudeh@mymail.com",
									EmailConfirmed = true,
									FirstName = "Taiseer",
									LastName = "Joudeh",
									Level = 1,
									JoinDate = DateTime.Now.AddYears(-3)
								};
 
								manager.Create(user, "MySuperP@ssword!");
							}

			-------------------------------------------------------------------------------------------------------------------
				(D): Add the class Application User Manager: UserManger<ApplicationUser>
				

					public class ApplicationUserManager : UserManager<ApplicationUser>
					{
						public ApplicationUserManager(IUserStore<ApplicationUser> store)
							: base(store)
						{
						}

						public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
						{
							var appDbContext = context.Get<ApplicationDbContext>();
							var appUserManager = new ApplicationUserManager(new UserStore<ApplicationUser>(appDbContext));

							return appUserManager;
						}
					}

    

			  -------------------------------------------------------------------------------------------------------------------
				(E): Ajouter une classe de référentiel pour prendre en charge le système d'identité ASP.NET (Class AuthRepostory)
				    -Methodes: RegisterUser et FindUser

				public class AuthRepository:IDisposable
					{
						private ApplicationContext _context;
						private UserManager<ApplicationUser> _userManager;

						public AuthRepository()
						{
							_context=new ApplicationContext();
							_userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>());
						}

						// GetData
						public IEnumerable<ApplicationUser> GetUsersI() => _userManager.Users.ToList();
       
						public async Task<ApplicationUser> FindUser(string username, string pass) => await _userManager.FindAsync(username, pass);

						public async Task<ApplicationUser> FindUser(string id) => await _userManager.FindByIdAsync(id);

						// Post Data
						public async Task<IdentityResult> RegisterUser(ApplicationUser appUser)
						{
							var user = new ApplicationUser
							{
								UserName = appUser.UserName,
								FirstName = appUser.FirstName,
								LastName = appUser.LastName,
								DateOfBirth = appUser.DateOfBirth,
								Email = appUser.Email,
								Password = appUser.Password,
								ConfirmPassword = appUser.ConfirmPassword,
								Actif = appUser.Actif

							};

							return await _userManager.CreateAsync(user, user.Password);

						}

						public async Task<ApplicationUser> DeleteUser(ApplicationUser user)
						{
							await _userManager.DeleteAsync(user);
							return user;
						} 

						public void Dispose()
						{
						   _context.Dispose();
							_userManager.Dispose();
						}
					}

	 -------------------------------------------------------------------------------------------------------------------
				(F):Ajoutez notre contrôleur "AccountController" pour gerer le compte d'utilisateurs '









************************************Partie II****************************
-Envoyer des e-mails de confirmation après la création du compte.
-Configurez l'utilisateur (nom d'utilisateur, courrier électronique) et la politique de mot de passe.
-Activer la modification du mot de passe et la suppression du compte.



Etape1:  Scenario: "EmailConfirmed", cette colonne sert à signaler si le courrier électronique
          fourni par l'utilisateur enregistré est valide et appartient à cet utilisateur.

		  Le scénario que nous voulons mettre en œuvre cet utilisateur s'inscrira dans le système,
		  puis un e-mail de confirmation sera envoyé au courrier électronique fourni lors de l'inscription.
		  Ce courrier électronique comprendra un lien d'activation et un jeton (code) qui est lié à cet utilisateur uniquement
		  et Valable pour une certaine période.

Une fois que l'utilisateur ouvre ce courrier électronique et clique sur le lien d'activation,
et si le token (code) est valide, le champ "EmailConfirmed" sera défini sur "true"
et cela prouve que le courrier électronique appartient à l'utilisateur enregistré.

Etape2: Install package SendGrid (pourenvvoyer des emails)
