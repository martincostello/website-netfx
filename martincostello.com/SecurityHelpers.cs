// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SecurityHelpers.cs" company="http://www.martincostello.com">
//   Martin Costello (c) 2014
// </copyright>
// <summary>
//   SecurityHelpers.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace MartinCostello
{
    using System.Globalization;
    using System.Web.Helpers;

    /// <summary>
    /// A class containing helpers for security.  This class cannot be inherited.
    /// </summary>
    public static class SecurityHelpers
    {
        /// <summary>
        /// The name of the HTTP header containing the anti-forgery token for AJAX POST requests.
        /// </summary>
        public const string AjaxAntiForgeryTokenName = "RequestVerificationToken";

        /// <summary>
        /// Returns an anti-forgery token to use to prevent CSRF in AJAX POST requests.
        /// </summary>
        /// <returns>
        /// An anti-forgery token to use to prevent CSRF in AJAX POST requests.
        /// </returns>
        public static string CreateTokenHeaderValue()
        {
            string cookieToken;
            string formToken;
            AntiForgery.GetTokens(null, out cookieToken, out formToken);
            return string.Format(CultureInfo.InvariantCulture, "{0}:{1}", cookieToken, formToken);
        }
    }
}