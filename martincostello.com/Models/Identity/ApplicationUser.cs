// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplicationUser.cs" company="http://www.martincostello.com">
//   Martin Costello (c) 2015
// </copyright>
// <summary>
//   ApplicationUser.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Security.Claims;
using System.Threading.Tasks;
using ElCamino.AspNet.Identity.AzureTable.Model;
using Microsoft.AspNet.Identity;

namespace MartinCostello.Models.Identity
{
    /// <summary>
    /// A class representing an application user.
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        /// <summary>
        /// Generates a <see cref="ClaimsIdentity"/> for the user as an asynchronous operation.
        /// </summary>
        /// <param name="manager">The <see cref="UserManager{T}"/> to use to create the identity.</param>
        /// <returns>
        /// A <see cref="Task{T}"/> representing the asynchronous operation to generate the identity.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="manager"/> is <see langword="null"/>.
        /// </exception>
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            if (manager == null)
            {
                throw new ArgumentNullException("manager");
            }

            var identity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);

            //// TODO Add any custom user claims here

            return identity;
        }
    }
}