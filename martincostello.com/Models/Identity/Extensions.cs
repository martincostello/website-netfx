// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Extensions.cs" company="http://www.martincostello.com">
//   Martin Costello (c) 2015
// </copyright>
// <summary>
//   Extensions.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Security.Claims;
using System.Security.Principal;

namespace MartinCostello.Models.Identity
{
    /// <summary>
    /// A class containing extension methods. This class cannot be inherited.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class Extensions
    {
        /// <summary>
        /// Gets the display name for the user.
        /// </summary>
        /// <param name="value">The <see cref="IPrincipal"/> to get the display name for.</param>
        /// <returns>
        /// The display name to use for <paramref name="value"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="value"/> is <see langword="null"/>.
        /// </exception>
        public static string GetUserDisplayName(this IPrincipal value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            ClaimsIdentity claimsIdentity = value.Identity as ClaimsIdentity;

            if (claimsIdentity == null)
            {
                return value.Identity.Name;
            }
            else
            {
                return claimsIdentity.GetUserDisplayName();
            }
        }

        /// <summary>
        /// Gets the Twitter handle for the user.
        /// </summary>
        /// <param name="value">The <see cref="IPrincipal"/> to get the Twitter handle for.</param>
        /// <returns>
        /// The Twitter handle associated with <paramref name="value"/>, if any; otherwise <see langword="null"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="value"/> is <see langword="null"/>.
        /// </exception>
        public static string GetUserTwitterHandle(this IPrincipal value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            ClaimsIdentity claimsIdentity = value.Identity as ClaimsIdentity;

            if (claimsIdentity == null)
            {
                return value.Identity.Name;
            }
            else
            {
                return claimsIdentity.GetUserTwitterHandle();
            }
        }

        /// <summary>
        /// Gets the display name for the user.
        /// </summary>
        /// <param name="value">The <see cref="ClaimsIdentity"/> to get the display name for.</param>
        /// <returns>
        /// The display name to use for <paramref name="value"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="value"/> is <see langword="null"/>.
        /// </exception>
        public static string GetUserDisplayName(this ClaimsIdentity value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            var claim = value.FindFirst(ClaimTypes.GivenName);

            if (claim != null && !string.IsNullOrEmpty(claim.Value))
            {
                return claim.Value;
            }
            else
            {
                string name = value.Name;

                // If the name is an email address, then hide the domain
                int index = name.IndexOf('@');

                if (index > -1)
                {
                    name = name.Substring(0, index);

                    // Try and extract a "name" out of the email address (like "john.smith@gmail.com")
                    index = name.IndexOf('.');

                    if (index > -1)
                    {
                        name = name.Substring(0, index);

                        if (name.Length > 1 &&
                            char.IsLetter(name[0]) &&
                            char.IsLower(name[0]))
                        {
                            name = char.ToUpperInvariant(name[0]) + name.Substring(1);
                        }
                    }
                }

                return name;
            }
        }

        /// <summary>
        /// Gets the Twitter handle for the user.
        /// </summary>
        /// <param name="value">The <see cref="ClaimsIdentity"/> to get the Twitter handle for.</param>
        /// <returns>
        /// The Twitter handle associated with <paramref name="value"/>, if any; otherwise <see langword="null"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="value"/> is <see langword="null"/>.
        /// </exception>
        public static string GetUserTwitterHandle(this ClaimsIdentity value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            var claim = value.FindFirst(IdentityConstants.TwitterScreenNameClaim);

            if (claim != null && !string.IsNullOrEmpty(claim.Value))
            {
                return claim.Value;
            }
            else
            {
                return null;
            }
        }
    }
}