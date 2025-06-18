// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ManageController.cs" company="http://www.martincostello.com">
//   Martin Costello (c) 2015
// </copyright>
// <summary>
//   ManageController.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using log4net;
using MartinCostello.Models;
using MartinCostello.Models.Identity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;

namespace MartinCostello.Controllers
{
    /// <summary>
    /// A class representing the controller for managing users and their identities.
    /// </summary>
    [Authorize]
    public class ManageController : IdentityControllerBase
    {
        /// <summary>
        /// The <see cref="ILog"/> to use. This field is read-only.
        /// </summary>
        private static readonly ILog Log = LogManager.GetLogger(typeof(ManageController));

        /// <summary>
        /// Initializes a new instance of the <see cref="ManageController"/> class.
        /// </summary>
        public ManageController()
            : base()
        {
        }

        /// <summary>
        /// Returns the <c>Index</c> GET action.
        /// </summary>
        /// <param name="message">The optional message to display.</param>
        /// <returns>
        /// The action result of the view.
        /// </returns>
        [HttpGet]
        public async Task<ActionResult> Index(ManageMessageId? message)
        {
            ViewBag.StatusMessage = message == ManageMessageId.Error ?
                "An error has occurred." :
                string.Empty;

            string userId = User.Identity.GetUserId();

            ManageIndexViewModel model = new ManageIndexViewModel()
            {
                BrowserRemembered = await AuthenticationManager.TwoFactorBrowserRememberedAsync(userId),
                Logins = await UserManager.GetLoginsAsync(userId),
            };

            return View(model);
        }

        /// <summary>
        /// Returns the <c>LinkLogin</c> POST action.
        /// </summary>
        /// <param name="provider">The login provider to link.</param>
        /// <returns>
        /// The action result of the view.
        /// </returns>
        [ActionName("LinkLogin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LinkLogOn(string provider)
        {
            string userId = User.Identity.GetUserId();
            string redirectUri = Url.Action("LinkLoginCallback", "Manage");

            // Request a redirect to the external login provider to link a login for the current user
            return new ChallengeResult(provider, redirectUri, userId);
        }

        /// <summary>
        /// Returns the <c>LinkLoginCallback</c> GET action.
        /// </summary>
        /// <returns>
        /// The action result of the view.
        /// </returns>
        [ActionName("LinkLoginCallback")]
        [HttpGet]
        public async Task<ActionResult> LinkLogOnCallback()
        {
            string userId = User.Identity.GetUserId();

            ExternalLoginInfo loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(
                IdentityConstants.XsrfKey,
                userId);

            bool succeeded;

            if (loginInfo == null)
            {
                succeeded = false;
            }
            else
            {
                succeeded = (await UserManager.AddLoginAsync(userId, loginInfo.Login, AuthenticationManager)).Succeeded;

                if (succeeded)
                {
                    Log.InfoFormat(CultureInfo.InvariantCulture, "Added login for user Id '{0}' and provider '{1}'.", User.Identity.Name, loginInfo.Login.LoginProvider);
                }
                else
                {
                    Log.ErrorFormat(CultureInfo.InvariantCulture, "Failed to add login for user Id '{0}' and provider '{1}'.", userId, loginInfo.Login.LoginProvider);
                }
            }

            object model = succeeded ? null : new { Message = ManageMessageId.Error };

            return RedirectToAction("ManageLogins", model);
        }

        /// <summary>
        /// Returns the <c>ManageLogins</c> GET action.
        /// </summary>
        /// <param name="message">The optional message to display.</param>
        /// <returns>
        /// The action result of the view.
        /// </returns>
        public async Task<ActionResult> ManageLogins(ManageMessageId? message)
        {
            ViewBag.StatusMessage = message == ManageMessageId.RemoveLogOnSuccess ?
                "The external login was removed." :
                message == ManageMessageId.Error ? "An error has occurred." : string.Empty;

            string userId = User.Identity.GetUserId();

            ApplicationUser user = await UserManager.FindByIdAsync(userId);

            if (user == null)
            {
                return View("Error");
            }

            var userLogins = await UserManager.GetLoginsAsync(userId);

            var otherLogins = AuthenticationManager.GetExternalAuthenticationTypes()
                .Where((auth) => userLogins.All((ul) => auth.AuthenticationType != ul.LoginProvider))
                .ToList();

            ViewBag.ShowRemoveButton = userLogins.Count > 1;

            var model = new ManageLoginsViewModel()
            {
                CurrentLogins = userLogins,
                OtherLogins = otherLogins,
            };

            return View(model);
        }

        /// <summary>
        /// Returns the <c>RemoveLogin</c> POST action.
        /// </summary>
        /// <param name="loginProvider">The login provider to remove.</param>
        /// <param name="providerKey">The provider key.</param>
        /// <returns>
        /// The action result of the view.
        /// </returns>
        [ActionName("RemoveLogin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Naming",
            "CA1726:UsePreferredTerms",
            MessageId = "login",
            Justification = "Matches ASP.NET Identity terms.")]
        public async Task<ActionResult> RemoveLogOn(string loginProvider, string providerKey)
        {
            string userId = User.Identity.GetUserId();
            UserLoginInfo login = new UserLoginInfo(loginProvider, providerKey);

            IdentityResult result;

            try
            {
                result = await UserManager.RemoveLoginAsync(userId, login);
            }
            catch (Exception ex)
            {
                Log.ErrorFormat(ex, "Failed to remove login for provider '{0}' from user '{1}': {2}", loginProvider, userId, ex.Message);
                throw;
            }

            ManageMessageId? message;

            if (result.Succeeded)
            {
                Log.InfoFormat(CultureInfo.InvariantCulture, "Removed login for provider '{0}' from user '{1}'.", loginProvider, User.Identity.Name);

                ApplicationUser user = await UserManager.FindByIdAsync(userId);

                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }

                message = ManageMessageId.RemoveLogOnSuccess;
            }
            else
            {
                Log.ErrorFormat(CultureInfo.InvariantCulture, "Failed to remove login for provider '{0}' from user '{1}'.", loginProvider, userId);
                message = ManageMessageId.Error;
            }

            return RedirectToAction("ManageLogins", new { Message = message });
        }

        /// <summary>
        /// Handles the <c>Users</c> GET action.
        /// </summary>
        /// <returns>
        /// The action result of the view.
        /// </returns>
        [Authorize(Roles = RoleNames.Admin)]
        [HttpGet]
        public ActionResult Users()
        {
            var model = UserManager.Users
                .ToList()
                .OrderBy((p) => p.Email, StringComparer.OrdinalIgnoreCase)
                .ToList();

            return View(model);
        }

        /// <summary>
        /// Handles the <c>AddRole</c> POST action.
        /// </summary>
        /// <param name="userId">The user Id to add the role to.</param>
        /// <param name="role">The name of the role to add to the user.</param>
        /// <returns>
        /// The action result of the view.
        /// </returns>
        [Authorize(Roles = RoleNames.Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddRole(string userId, string role)
        {
            var result = await UserManager.AddToRoleAsync(userId, role);

            if (result != null && result.Succeeded)
            {
                Log.InfoFormat(CultureInfo.InvariantCulture, "User '{0}' added user '{1}' to role '{2}'.", User.Identity.Name, userId, role);
            }

            return RedirectToAction("Users");
        }

        /// <summary>
        /// Handles the <c>RemoveRole</c> POST action.
        /// </summary>
        /// <param name="userId">The user Id to remove the role from.</param>
        /// <param name="role">The name of the role to remove from the user.</param>
        /// <returns>
        /// The action result of the view.
        /// </returns>
        [Authorize(Roles = RoleNames.Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemoveRole(string userId, string role)
        {
            var result = await UserManager.RemoveFromRoleAsync(userId, role);

            if (result != null && result.Succeeded)
            {
                Log.InfoFormat(CultureInfo.InvariantCulture, "User '{0}' removed user '{1}' from role '{2}'.", User.Identity.Name, userId, role);
            }

            return RedirectToAction("Users");
        }
    }
}
