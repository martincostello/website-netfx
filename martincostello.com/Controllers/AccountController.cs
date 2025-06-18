// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AccountController.cs" company="http://www.martincostello.com">
//   Martin Costello (c) 2015
// </copyright>
// <summary>
//   AccountController.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Web.Mvc;
using log4net;
using MartinCostello.Models.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;

namespace MartinCostello.Controllers
{
    /// <summary>
    /// A class representing the controller for managing accounts.
    /// </summary>
    [Authorize]
    public class AccountController : IdentityControllerBase
    {
        /// <summary>
        /// The <see cref="ILog"/> to use. This field is read-only.
        /// </summary>
        private static readonly ILog Log = LogManager.GetLogger(typeof(AccountController));

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountController"/> class.
        /// </summary>
        public AccountController()
            : base()
        {
        }

        /// <summary>
        /// Handles the <c>ExternalLogin</c> POST action.
        /// </summary>
        /// <param name="provider">The external login provider.</param>
        /// <param name="returnUrl">The return URL.</param>
        /// <returns>
        /// The action result of the view.
        /// </returns>
        [ActionName("ExternalLogin")]
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Design",
            "CA1054:UriParametersShouldNotBeStrings",
            MessageId = "1#",
            Justification = "Required for use with MVC routing.")]
        public ActionResult ExternalLogOn(string provider, string returnUrl)
        {
            object routeValues = new { ReturnUrl = returnUrl };
            string redirectUri = Url.Action("ExternalLoginCallback", "Account", routeValues);

            return new ChallengeResult(provider, redirectUri);
        }

        /// <summary>
        /// Handles the <c>ExternalLogInCallback</c> GET action.
        /// </summary>
        /// <param name="returnUrl">The return URL.</param>
        /// <returns>
        /// The action result of the view.
        /// </returns>
        [ActionName("ExternalLogInCallback")]
        [AllowAnonymous]
        [HttpGet]
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Design",
            "CA1054:UriParametersShouldNotBeStrings",
            MessageId = "0#",
            Justification = "Required for use with MVC routing.")]
        public async Task<ActionResult> ExternalLogOnCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();

            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);

            switch (result)
            {
                case SignInStatus.Success:
                    Log.InfoFormat(
                        CultureInfo.InvariantCulture,
                        "User '{0}' ('{1}') has logged on using provider '{2}' from {3} with User Agent '{4}'.",
                        loginInfo.ExternalIdentity.Name,
                        loginInfo.Email,
                        loginInfo.Login.LoginProvider,
                        Request.UserHostAddress,
                        Request.UserAgent);

                    return RedirectToLocal(returnUrl);

                case SignInStatus.LockedOut:
                    return View("Lockout");

                case SignInStatus.RequiresVerification:
                    throw new NotImplementedException();

                case SignInStatus.Failure:
                default:
                    break;
            }

            // If the user does not have an account, then create one using the email address
            if (loginInfo.Email == null)
            {
                return RedirectToAction("Login", new { Error = "NoEmail" });
            }

            var user = new ApplicationUser()
            {
                UserName = loginInfo.Email,
                Email = loginInfo.Email,
            };

            var createResult = await UserManager.CreateAsync(user);

            if (!createResult.Succeeded)
            {
                return RedirectToAction("Login", new { Error = "CreateFailed" });
            }

            Log.InfoFormat(CultureInfo.InvariantCulture, "Created user '{0}' with email address '{1}'.", user.UserName, user.Email);

            var addLoginResult = await UserManager.AddLoginAsync(user.Id, loginInfo.Login, AuthenticationManager);

            if (!addLoginResult.Succeeded)
            {
                return RedirectToAction("Login", new { Error = "LoginFailed" });
            }

            Log.InfoFormat(CultureInfo.InvariantCulture, "Added login for user Id '{0}' for provider '{1}'.", user.Id, loginInfo.Login.LoginProvider);

            await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            return RedirectToLocal(returnUrl);
        }

        /// <summary>
        /// Handles the <c>Login</c> GET action.
        /// </summary>
        /// <param name="returnUrl">The return URL.</param>
        /// <returns>
        /// The action result of the view.
        /// </returns>
        [ActionName("Login")]
        [AllowAnonymous]
        [HttpGet]
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Design",
            "CA1054:UriParametersShouldNotBeStrings",
            MessageId = "0#",
            Justification = "Required for use with MVC routing.")]
        public ActionResult LogOn(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        /// <summary>
        /// Handles the <c>LogOff</c> POST action.
        /// </summary>
        /// <returns>
        /// The action result of the view.
        /// </returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            string userId = User.Identity.Name;

            AuthenticationManager.SignOut();

            Log.InfoFormat(CultureInfo.InvariantCulture, "User '{0}' has logged off.", userId);

            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// Redirects to the specified local URL.
        /// </summary>
        /// <param name="returnUrl">The return local URL.</param>
        /// <returns>
        /// The URL to redirect to.
        /// </returns>
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
