// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplicationUserManager.cs" company="http://www.martincostello.com">
//   Martin Costello (c) 2015
// </copyright>
// <summary>
//   ApplicationUserManager.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ElCamino.AspNet.Identity.AzureTable;
using log4net;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;

namespace MartinCostello.Models.Identity
{
    /// <summary>
    /// A class representing a user manager.
    /// </summary>
    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        /// <summary>
        /// The <see cref="ILog"/> to use. This field is read-only.
        /// </summary>
        private static readonly ILog Log = LogManager.GetLogger(typeof(ApplicationUserManager));

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationUserManager"/> class.
        /// </summary>
        /// <param name="store">The user store to use.</param>
        public ApplicationUserManager(IUserStore<ApplicationUser> store)
            : base(store)
        {
        }

        /// <summary>
        /// Associates a login with a user and updates the user's claims as an asynchronous operation.
        /// </summary>
        /// <param name="userId">The user Id to associate the login with.</param>
        /// <param name="login">The login to associate with the user.</param>
        /// <param name="authenticationManager">The <see cref="IAuthenticationManager"/> to use.</param>
        /// <returns>
        /// A <see cref="Task{T}"/> representing the asynchronous operation to associate the login.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Naming",
            "CA1726:UsePreferredTerms",
            MessageId = "login",
            Justification = "Matches ASP.NET Identity terms.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Naming",
            "CA1726:UsePreferredTerms",
            MessageId = "Login",
            Justification = "Matches ASP.NET Identity terms.")]
        public async Task<IdentityResult> AddLoginAsync(string userId, UserLoginInfo login, IAuthenticationManager authenticationManager)
        {
            IdentityResult result;

            try
            {
                result = await AddLoginAsync(userId, login);
            }
            catch (Exception ex)
            {
                Log.ErrorFormat(ex, "Failed to add login for user Id '{0}': {1}", userId, ex.Message);
                throw;
            }

            if (result.Succeeded && authenticationManager != null)
            {
                // Get the claims identity
                ClaimsIdentity cookieClaims = await authenticationManager.GetExternalIdentityAsync(DefaultAuthenticationTypes.ExternalCookie);

                if (cookieClaims != null)
                {
                    // Retrieve the existing claims
                    var claimsFromStore = await GetClaimsAsync(userId);

                    var claimsOfInterest = new string[]
                    {
                        ClaimTypes.Email,
                        ClaimTypes.GivenName,
                        ClaimTypes.Surname,
                        IdentityConstants.TwitterScreenNameClaim,
                    };

                    foreach (string claimType in claimsOfInterest)
                    {
                        var cookieClaim = cookieClaims.Claims
                            .Where((p) => p.Type == claimType)
                            .FirstOrDefault();

                        if (cookieClaim != null)
                        {
                            var storeClaim = claimsFromStore
                                .Where((p) => p.Type == claimType)
                                .FirstOrDefault();

                            if (storeClaim == null)
                            {
                                // We don't have this claim, so add it
                                await AddClaimAsync(userId, cookieClaim);
                            }
                            else if (storeClaim.Value != cookieClaim.Value)
                            {
                                // The claim has changed, so update it
                                await RemoveClaimAsync(userId, storeClaim);
                                await AddClaimAsync(userId, cookieClaim);
                            }
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Creates a new instance of <see cref="ApplicationUserManager"/>.
        /// </summary>
        /// <param name="options">The identity factory options.</param>
        /// <param name="context">The current OWIN context.</param>
        /// <returns>
        /// The created instance of <see cref="ApplicationUserManager"/>.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Reliability",
            "CA2000:Dispose objects before losing scope",
            Justification = "The user store is owned by the ApplicationUserManager instance.")]
        internal static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
        {
            Debug.Assert(options != null, "options cannot be null.");
            Debug.Assert(context != null, "context cannot be null.");

            var userContext = context.Get<ApplicationUserContext>();
            var store = new UserStore<ApplicationUser>(userContext);

            try
            {
                if (string.Equals(System.Configuration.ConfigurationManager.AppSettings["AspNetIdentity:CreateTables"], bool.TrueString, StringComparison.OrdinalIgnoreCase))
                {
                    store.CreateTablesIfNotExists();
                }

                var manager = new ApplicationUserManager(store);

                try
                {
                    // Configure validation logic for usernames
                    manager.UserValidator = new UserValidator<ApplicationUser>(manager)
                    {
                        AllowOnlyAlphanumericUserNames = false,
                        RequireUniqueEmail = true,
                    };

                    // Configure validation logic for passwords
                    manager.PasswordValidator = new PasswordValidator()
                    {
                        RequiredLength = 8,
                        RequireNonLetterOrDigit = true,
                        RequireDigit = true,
                        RequireLowercase = true,
                        RequireUppercase = true,
                    };

                    // Configure user lockout defaults
                    manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
                    manager.MaxFailedAccessAttemptsBeforeLockout = 5;
                    manager.UserLockoutEnabledByDefault = true;

                    var dataProtectionProvider = options.DataProtectionProvider;

                    if (dataProtectionProvider != null)
                    {
                        var purposes = new string[] { "ASP.NET Identity" };
                        var protector = dataProtectionProvider.Create(purposes);
                        manager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser>(protector);
                    }

                    return manager;
                }
                catch (Exception)
                {
                    manager.Dispose();
                    throw;
                }
            }
            catch (Exception)
            {
                store.Dispose();
                throw;
            }
        }
    }
}
