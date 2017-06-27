using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using APIIdentityOAUTH.Services;
using APIIdentityOAUTH.Validators;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;

namespace APIIdentityOAUTH.Infrastructure
{
    public class ApplicationUserManager:UserManager<ApplicationUser>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> store) : base(store)
        {
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options ,IOwinContext  context)
        {
            var appDbContext = context.Get<ApplicationDbContext>();
            var appUserManager=new ApplicationUserManager(new UserStore<ApplicationUser>(appDbContext));

            //configuré le "Service Email", nous devons le confiner avec notre système Identité, 
            appUserManager.EmailService = new EmailService();
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                appUserManager.UserTokenProvider =
                    new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"))
                    {
                        //Code for email confirmation and reset password life time
                        TokenLifespan = TimeSpan.FromHours(6)
                    };
            }

            //Customer politic username
            appUserManager.UserValidator = new MyCustomUserValidator(appUserManager)
            {
                AllowOnlyAlphanumericUserNames = true,
                RequireUniqueEmail = true

            };


            // Change Password Policy
            appUserManager.PasswordValidator= new MyCustomPasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = true,
                RequireDigit = false,
                RequireLowercase = true,
                RequireUppercase = true,
            };
        




            return appUserManager;

        }








           
        }
    }

