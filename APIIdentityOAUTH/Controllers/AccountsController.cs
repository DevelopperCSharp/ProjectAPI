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


            // Send mail after  creation an account
            //la création d'un code unique (token) qui est valide pour les 6 prochaines heures 
            //et lié à cet ID utilisateur, cela se produit uniquement lorsque vous appelez la méthode "GenerateEmailConfirmationTokenAsync",
            string token = await AppUserManager.GenerateEmailConfirmationTokenAsync(user.Id);
            //code = System.Web.HttpUtility.UrlEncode (code); code = System.Web.HttpUtility.UrlDecode (code)
            Uri callUrl = new Uri(Url.Link("ConfirmEmailRoute", new {id=user.Id, token = System.Web.HttpUtility.UrlEncode(token)}));
            await AppUserManager.SendEmailAsync(user.Id, "Activate your Account", "Please confirm your account by clicking href=\"" + callUrl + "\">here</a>");



            Uri locationHeader=new Uri(Url.Link("GetUserById", new {id=user.Id}));
            return Created(locationHeader, TheModelFactory.Create(user));
           
        }

        //Ajouter l'URL Confirmer Email:http://localhost/api/account/ConfirmEmail?userid=xxxx&code=xxxx
        [HttpGet]
        [Route("ConfirmEmail", Name = "ConfirmEmailRoute")]
        public async  Task<IHttpActionResult> ConfirmEmail(string userId = "", string token = "")
        {
            string code = System.Web.HttpUtility.UrlDecode(token);
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(code))
            {
                ModelState.AddModelError("", "User Id and Code are required");
                return BadRequest(ModelState);
            }

            IdentityResult result = await AppUserManager.ConfirmEmailAsync(userId, code);

            return !result.Succeeded ? GetErrorResult(result) : Ok();
        }




        // Changer le mot de pass
        [Route("ChangePassword")]
        public async Task<IHttpActionResult> ChangePassword(ChangePasswordBindingModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            // ID User Authentifié
            IdentityResult result = await AppUserManager.ChangePasswordAsync(User.Identity.GetUserId(),
                model.OldPassword,
                model.NewPassword);
            return result.Succeeded ? Ok() : GetErrorResult(result);
        }

        // Delete User

        [Route("user/{id:guid}")]
        public async Task<IHttpActionResult> DeleteUser(string id)
        {
            var user = await AppUserManager.FindByIdAsync(id);
            if (user != null)
            {
                IdentityResult result = await AppUserManager.DeleteAsync(user);
                return result.Succeeded ? GetErrorResult(result):Ok();
            }
            return NotFound();
        }


    }
}
