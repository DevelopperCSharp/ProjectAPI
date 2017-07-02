using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using APIIdentityOAUTH.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;


namespace APIIdentityOAUTH.Controllers
{
    [Authorize(Roles = "Admin")]
    [RoutePrefix("api/roles")]
    public class RolesController : BaseApiController
    {

        [Route("", Name = "GetAllRoles")]
        public IHttpActionResult GetAllRoles() =>  Ok(AppRoleManager.Roles.ToList()
                                                                                   .Select(r => TheModelFactory.Create(r)));
   

    [Route("role/{id:guid}",Name="GetRoleById")]
     public  async Task<IHttpActionResult> GetRole(string Id)
        {
            var role = await AppRoleManager.FindByIdAsync(Id);
            if (role != null) return Ok(TheModelFactory.Create(role));
            return  NotFound();
        }


        [Route("Register")]
        public  async Task<IHttpActionResult> RegisterRole(CreateRoleBindingModel modelRole)
        {
            if (ModelState.IsValid) return BadRequest(ModelState);

            var role = new IdentityRole
            {
                Name = modelRole.Name
            };

            IdentityResult addRoleResult = await AppRoleManager.CreateAsync(role);
            if (!addRoleResult.Succeeded)  return GetErrorResult(addRoleResult);
            var locationHeader= new Uri(Url.Link("GetRoleById", new {id=role.Id}));
            return Created(locationHeader, TheModelFactory.Create(role));

        }

        [Route("DeleteRole/{id:guid}")]
        public async Task<IHttpActionResult> DeleteRole(string Id)
        {
            var role = await AppRoleManager.FindByIdAsync(Id);
            if (role != null)
            {
                IdentityResult result = await AppRoleManager.DeleteAsync(role);

                if(!result.Succeeded) GetErrorResult(result);
                return Ok();
            }

            return NotFound();

        }

        [Route("ManageUsersInRole")]
        public async Task<IHttpActionResult> ManageUsersInRole(UsersInRoleModel model)
        {
            var role = await AppRoleManager.FindByIdAsync(model.Id);
            if (role == null)
            {
               ModelState.AddModelError("","Role dos not exist");
                return BadRequest(ModelState);
            }

            foreach (var user in model.EnrolledUsers )
            {
                var appUser = await AppUserManager.FindByIdAsync(user);
                if (appUser == null)
                {
                    ModelState.AddModelError("",$"User dos not exist {user}");
                    continue;
                }

                if (!AppUserManager.IsInRole(user, role.Name))
                {
                    IdentityResult result = await AppUserManager.AddToRoleAsync(user, role.Name);

                    if(! result.Succeeded) ModelState.AddModelError("User",$" User can't not added to role {user}");
                }
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok();
        }



    }

}
