// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IdentityControllerBase.cs" company="http://www.martincostello.com">
//   Martin Costello (c) 2015
// </copyright>
// <summary>
//   IdentityControllerBase.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Web;
using System.Web.Mvc;
using MartinCostello.Models.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;

namespace MartinCostello.Controllers
{
    /// <summary>
    /// The base class for identity-related controllers.
    /// </summary>
    [Authorize]
    public abstract class IdentityControllerBase : Controller
    {
        /// <summary>
        /// The <see cref="ApplicationSignInManager"/> in use, if any.
        /// </summary>
        private ApplicationSignInManager _signInManager;

        /// <summary>
        /// The <see cref="ApplicationUserManager"/> in use, if any.
        /// </summary>
        private ApplicationUserManager _userManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityControllerBase"/> class.
        /// </summary>
        protected IdentityControllerBase()
            : base()
        {
        }

        /// <summary>
        /// Gets or sets the <see cref="ApplicationSignInManager"/> in use.
        /// </summary>
        public ApplicationSignInManager SignInManager
        {
            get { return _signInManager ?? this.HttpContext.GetOwinContext().Get<ApplicationSignInManager>(); }
            protected set { _signInManager = value; }
        }

        /// <summary>
        /// Gets or sets the <see cref="ApplicationUserManager"/> in use.
        /// </summary>
        public ApplicationUserManager UserManager
        {
            get { return _userManager ?? this.HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>(); }
            protected set { _userManager = value; }
        }

        /// <summary>
        /// Gets the current <see cref="IAuthenticationManager"/>.
        /// </summary>
        protected IAuthenticationManager AuthenticationManager
        {
            get { return this.HttpContext.GetOwinContext().Authentication; }
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing && _userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }
    }
}