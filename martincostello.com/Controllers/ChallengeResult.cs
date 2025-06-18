// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChallengeResult.cs" company="http://www.martincostello.com">
//   Martin Costello (c) 2015
// </copyright>
// <summary>
//   ChallengeResult.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Web;
using System.Web.Mvc;
using MartinCostello.Models.Identity;
using Microsoft.Owin.Security;

namespace MartinCostello.Controllers
{
    /// <summary>
    /// A class representing a challenge result for a third-party login. This class cannot be inherited.
    /// </summary>
    internal sealed class ChallengeResult : HttpUnauthorizedResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChallengeResult"/> class.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <param name="redirectUri">The redirect URI.</param>
        public ChallengeResult(string provider, string redirectUri)
            : this(provider, redirectUri, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChallengeResult"/> class.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <param name="redirectUri">The redirect URI.</param>
        /// <param name="userId">The user Id.</param>
        public ChallengeResult(string provider, string redirectUri, string userId)
        {
            this.LogOnProvider = provider;
            this.RedirectUri = redirectUri;
            this.UserId = userId;
        }

        /// <summary>
        /// Gets or sets the login provider.
        /// </summary>
        public string LogOnProvider { get; set; }

        /// <summary>
        /// Gets or sets the redirect URI.
        /// </summary>
        public string RedirectUri { get; set; }

        /// <summary>
        /// Gets or sets the user Id.
        /// </summary>
        public string UserId { get; set; }

        /// <inheritdoc />
        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            var properties = new AuthenticationProperties()
            {
                RedirectUri = this.RedirectUri,
            };

            if (this.UserId != null)
            {
                properties.Dictionary[IdentityConstants.XsrfKey] = this.UserId;
            }

            context.HttpContext.GetOwinContext().Authentication.Challenge(properties, this.LogOnProvider);
        }
    }
}