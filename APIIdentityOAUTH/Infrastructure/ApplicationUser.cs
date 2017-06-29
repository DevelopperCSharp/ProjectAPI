using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace APIIdentityOAUTH.Infrastructure
{
    public class ApplicationUser:IdentityUser
    {
        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; }

        [Required]
        public byte Level { get; set; }

        [Required]
        public DateTime JoinDate { get; set; }

        //Maintenant, nous allons ajouter la méthode helper qui sera responsable d'obtenir l'identité de l'utilisateur authentifiée (tous les rôles et les revendications sont mappés à l'utilisateur).
        //La classe "UserManager" contient une méthode nommée "CreateIdentityAsync" pour effectuer cette tâche,
        //elle interroge la base de données et obtient tous les rôles et réclamations pour cet utilisateur

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager, string authenticationType)
        {
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
            // Add custom user claims here
            return userIdentity;
        }





    }


   
}