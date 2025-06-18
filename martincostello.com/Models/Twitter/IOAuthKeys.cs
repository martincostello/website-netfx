// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IOAuthKeys.cs" company="http://www.martincostello.com">
//   Martin Costello (c) 2015
// </copyright>
// <summary>
//   IOAuthKeys.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace MartinCostello.Models.Twitter
{
    /// <summary>
    /// Defines the <c>OAuth</c> keys for an external application.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Microsoft.Naming",
        "CA1704:IdentifiersShouldBeSpelledCorrectly",
        MessageId = "Auth",
        Justification = "OAuth is the correct term.")]
    public interface IOAuthKeys
    {
        /// <summary>
        /// Gets the consumer key.
        /// </summary>
        string ConsumerKey
        {
            get;
        }

        /// <summary>
        /// Gets the consumer secret.
        /// </summary>
        string ConsumerSecret
        {
            get;
        }

        /// <summary>
        /// Gets the access token.
        /// </summary>
        string Token
        {
            get;
        }

        /// <summary>
        /// Gets the access token secret.
        /// </summary>
        string TokenSecret
        {
            get;
        }
    }
}