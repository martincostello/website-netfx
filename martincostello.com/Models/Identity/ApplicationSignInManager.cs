// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplicationSignInManager.cs" company="http://www.martincostello.com">
//   Martin Costello (c) 2015
// </copyright>
// <summary>
//   ApplicationSignInManager.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;

namespace MartinCostello.Models.Identity
{
    /// <summary>
    /// A class representing the sign-in manager for the application.
    /// </summary>
    public class ApplicationSignInManager : SignInManager<ApplicationUser, string>
    {
        /// <summary>
        /// The <see cref="ApplicationUserManager"/> in use by the instance.
        /// </summary>
        private readonly ApplicationUserManager _userManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationSignInManager"/> class.
        /// </summary>
        /// <param name="userManager">The <see cref="ApplicationUserManager"/> to use.</param>
        /// <param name="authenticationManager">The <see cref="IAuthenticationManager"/> to use.</param>
        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
            _userManager = userManager;
        }

        /// <inheritdoc />
        public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            return user.GenerateUserIdentityAsync(_userManager);
        }

        /// <summary>
        /// Creates a new instance of <see cref="ApplicationUserManager"/>.
        /// </summary>
        /// <param name="options">The identity factory options.</param>
        /// <param name="context">The current OWIN context.</param>
        /// <returns>
        /// The created instance of <see cref="ApplicationUserManager"/>.
        /// </returns>
        internal static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        {
            Debug.Assert(options != null, "options cannot be null.");
            Debug.Assert(context != null, "context cannot be null.");

            var userManager = context.Get<ApplicationUserManager>();

            return new ApplicationSignInManager(userManager, context.Authentication);
        }
    }
}