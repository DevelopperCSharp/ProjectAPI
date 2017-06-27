using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using APIIdentityOAUTH.Infrastructure;
using System.Threading.Tasks;
using APIIdentityOAUTH.Models;
using Microsoft.AspNet.Identity;

namespace APIIdentityOAUTH.Controllers
{
    [RoutePrefix("api/accounts")]
    public class AccountsController : BaseApiController
    {

        [Route("users")]
        public IHttpActionResult GetUsers()=> Ok(AppUserManager.Users.ToList()
                                                 .Select(u => TheModelFactory.Create(u)));
        




        [Route("user/{id:guid}", Name = "GetUserById")]
        public async Task<IHttpActionResult> GetUser(string id)
        {
          var user = await AppUserManager.FindByIdAsync(id);
          if (user != null) return Ok(TheModelFactory.Create(user));
          return NotFound();
        }




        [Route("user/{username}")]
        public async Task<IHttpActionResult> GetUserByName(string username)
        {
            var user = await AppUserManager.FindByNameAsync(username);
            if (user != null) return Ok(TheModelFactory.Create(user));
            return NotFound();
        }



        [Route("Register")]
        public async Task<IHttpActionResult> RegisterUser(CreateUserBindingModel createUserModel)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = new ApplicationUser
            {
                UserName = createUserModel.Username,
                Email = createUserModel.Email,
                FirstName = createUserModel.FirstName,
                LastName = createUserModel.LastName,
                Level = 3,
                JoinDate = DateTime.Now.Date,

            };

            IdentityResult addResult = await AppUserManager.CreateAsync(user);
            if (!addResult.Succeeded) return GetErrorResult(addResult);

            Uri locationHeader=new Uri(Url.Link("GetUserById", new {id=user.Id}));
            return Created(locationHeader, TheModelFactory.Create(user));
           
        }
       



    }
}
