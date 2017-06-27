using System.Net.Http;
using System.Web.Http;
using APIIdentityOAUTH.Infrastructure;
using Microsoft.AspNet.Identity.Owin;
using APIIdentityOAUTH.Models;
using Microsoft.AspNet.Identity;

namespace APIIdentityOAUTH.Controllers
{
    public class BaseApiController : ApiController
    {

        private ApplicationUserManager _AppUserManager;
        private ModelFactory _modelFactory;

        // instance og ApplicationUserManager
        protected ApplicationUserManager AppUserManager => _AppUserManager ??
                                                         Request.GetOwinContext().GetUserManager<ApplicationUserManager>();


        // Instance of ModelFactory

        protected ModelFactory TheModelFactory
        {
            get
            {
                if (_modelFactory == null)
                {
                    _modelFactory = new ModelFactory(this.Request, AppUserManager);
                }

                return _modelFactory;
            }
        }


        // Gestion the error 

        protected IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }

                if (ModelState.IsValid)
                {
                    // No ModelState errors are available to send, so just return an empty BadRequest.
                    return BadRequest();
                }

                return BadRequest(ModelState);
            }

            return null;
        }
    }
}

 