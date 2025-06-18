// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IdentityConstants.cs" company="http://www.martincostello.com">
//   Martin Costello (c) 2015
// </copyright>
// <summary>
//   IdentityConstants.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace MartinCostello.Models.Identity
{
    /// <summary>
    /// A class containing identity-related constants. This class cannot be inherited.
    /// </summary>
    public static class IdentityConstants
    {
        /// <summary>
        /// The claim type for a Twitter screen name.
        /// </summary>
        public const string TwitterScreenNameClaim = "urn:twitter:screenname";

        /// <summary>
        /// The Cross-site Request Forgery (XSRF) parameter key name.
        /// </summary>
        internal const string XsrfKey = "XsrfId";
    }
}