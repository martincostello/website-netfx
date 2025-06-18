// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OAuthKeys.cs" company="http://www.martincostello.com">
//   Martin Costello (c) 2015
// </copyright>
// <summary>
//   OAuthKeys.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace MartinCostello.Models.Twitter
{
    /// <summary>
    /// A class representing keys to use to generate <c>OAuth</c> authorization headers. This class cannot be inherited.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Microsoft.Naming",
        "CA1704:IdentifiersShouldBeSpelledCorrectly",
        MessageId = "Auth",
        Justification = "OAuth is the correct term.")]
    public sealed class OAuthKeys : IOAuthKeys
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OAuthKeys"/> class.
        /// </summary>
        public OAuthKeys()
        {
        }

        /// <summary>
        /// Gets or sets the consumer key.
        /// </summary>
        public string ConsumerKey
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the consumer secret.
        /// </summary>
        public string ConsumerSecret
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the access token.
        /// </summary>
        public string Token
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the access token secret.
        /// </summary>
        public string TokenSecret
        {
            get;
            set;
        }
    }
}