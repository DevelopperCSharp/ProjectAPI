using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using APIIdentityOAUTH.Infrastructure;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;

namespace APIIdentityOAUTH.Validators
{
   
        public class MyCustomUserValidator : UserValidator<ApplicationUser>
        {

            List<string> _allowedEmailDomains = new List<string> { "orange.fr", "outlook.com", "hotmail.com", "gmail.com", "yahoo.com" };

            public MyCustomUserValidator(ApplicationUserManager appUserManager)
                : base(appUserManager)
            {
            }

            public override async Task<IdentityResult> ValidateAsync(ApplicationUser user)
            {
                IdentityResult result = await base.ValidateAsync(user);

                var emailDomain = user.Email.Split('@')[1];

                if (!_allowedEmailDomains.Contains(emailDomain.ToLower()))
                {
                    var errors = result.Errors.ToList();

                    errors.Add($"Email domain '{emailDomain}' is not allowed");

                    result = new IdentityResult(errors);
                }

                return result;
            }
        }
}