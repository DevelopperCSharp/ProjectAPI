using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity.EntityFramework;

namespace APIIdentityOAUTH.Infrastructure
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(): base("APIIdentityOAuth", false)
        {
            Configuration.ProxyCreationEnabled = false;
           // Configuration.LazyLoadingEnabled = false;
        }

        //Method “Create” will be called from our Owin Startup class
        public static ApplicationDbContext Create ()=>new ApplicationDbContext();
    }
}