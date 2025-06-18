// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OAuthHeaderGenerator.cs" company="http://www.martincostello.com">
//   Martin Costello (c) 2015
// </copyright>
// <summary>
//   OAuthHeaderGenerator.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace MartinCostello.Models.Twitter
{
    /// <summary>
    /// A class that generates <c>OAuth</c> signatures. This class cannot be inherited.
    /// </summary>
    /// <remarks>
    /// This code was written following the documentation on the following two pages:
    /// 1) <c>https://dev.twitter.com/oauth/overview/authorizing-requests</c>;
    /// 2) <c>https://dev.twitter.com/oauth/overview/creating-signatures</c>.
    /// </remarks>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Microsoft.Naming",
        "CA1704:IdentifiersShouldBeSpelledCorrectly",
        MessageId = "Auth",
        Justification = "OAuth is the correct term.")]
    public static class OAuthHeaderGenerator
    {
        /// <summary>
        /// Generates an <c>OAuth</c> authorization header from the specified parameters.
        /// </summary>
        /// <param name="httpMethod">The HTTP method.</param>
        /// <param name="uri">The URI to generate the header for (excluding any parameters).</param>
        /// <param name="parameters">A collection containing all the query string and body parameters that will be used for the request.</param>
        /// <param name="keys">The keys to use.</param>
        /// <returns>
        /// The generated value for the Authorization header to use for the specified parameters.
        /// </returns>
        public static string GenerateHeaderValue(
            string httpMethod,
            Uri uri,
            NameValueCollection parameters,
            IOAuthKeys keys)
        {
            return GenerateHeaderValue(
                httpMethod,
                uri,
                parameters,
                keys,
                nonce: null,
                timestamp: null);
        }

        /// <summary>
        /// Generates an <c>OAuth</c> authorization header from the specified parameters.
        /// </summary>
        /// <param name="httpMethod">The HTTP method.</param>
        /// <param name="uri">The URI to generate the header for (excluding any parameters).</param>
        /// <param name="parameters">A collection containing all the query string and body parameters that will be used for the request.</param>
        /// <param name="keys">The keys to use.</param>
        /// <param name="nonce">The optional nonce value to use.</param>
        /// <param name="timestamp">The optional timestamp value to use.</param>
        /// <returns>
        /// The generated value for the Authorization header to use for the specified parameters.
        /// </returns>
        /// <remarks>
        /// Used for unit testing so that a static nonce and timestamp can be specified.
        /// </remarks>
        internal static string GenerateHeaderValue(
            string httpMethod,
            Uri uri,
            NameValueCollection parameters,
            IOAuthKeys keys,
            string nonce,
            string timestamp)
        {
            if (httpMethod == null)
            {
                throw new ArgumentNullException("httpMethod");
            }

            if (uri == null)
            {
                throw new ArgumentNullException("uri");
            }

            if (parameters == null)
            {
                throw new ArgumentNullException("parameters");
            }

            if (keys == null)
            {
                throw new ArgumentNullException("keys");
            }

            OAuthAuthorizationData oauthData = new OAuthAuthorizationData(keys);

            if (nonce == null)
            {
                // Generate a random sequence of bytes to use as the nonce
                byte[] nonceBytes = new byte[16];

                using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
                {
                    rng.GetBytes(nonceBytes);
                }

                // Only alpha-numeric characters are supported, so remove the dashes
                oauthData.Nonce = new Guid(nonceBytes).ToString().Replace("-", string.Empty);
            }
            else
            {
                // Use the specified nonce
                oauthData.Nonce = nonce;
            }

            // Override the generated timestamp if one was specified
            if (timestamp != null)
            {
                oauthData.Timestamp = timestamp;
            }

            // Generate the signature and set in the OAuthAuthorizationData object
            GenerateSignatureForOAuth(httpMethod, uri, parameters, oauthData);

            // Build the header value from the OAuth values
            StringBuilder builder = new StringBuilder();

            builder.AppendFormat(CultureInfo.InvariantCulture, @"oauth_consumer_key=""{0}"", ", Uri.EscapeDataString(oauthData.ConsumerKey));
            builder.AppendFormat(CultureInfo.InvariantCulture, @"oauth_nonce=""{0}"", ", Uri.EscapeDataString(oauthData.Nonce));
            builder.AppendFormat(CultureInfo.InvariantCulture, @"oauth_signature=""{0}"", ", Uri.EscapeDataString(oauthData.Signature));
            builder.AppendFormat(CultureInfo.InvariantCulture, @"oauth_signature_method=""{0}"", ", Uri.EscapeDataString(oauthData.SignatureMethod));
            builder.AppendFormat(CultureInfo.InvariantCulture, @"oauth_timestamp=""{0}"", ", Uri.EscapeDataString(oauthData.Timestamp));
            builder.AppendFormat(CultureInfo.InvariantCulture, @"oauth_token=""{0}"", ", Uri.EscapeDataString(oauthData.Token));
            builder.AppendFormat(CultureInfo.InvariantCulture, @"oauth_version=""{0}""", Uri.EscapeDataString(oauthData.Version));

            return builder.ToString();
        }

        /// <summary>
        /// Generates the <c>OAuth</c> signature for the specified parameters.
        /// </summary>
        /// <param name="httpMethod">The HTTP method name.</param>
        /// <param name="uri">The URI being requested.</param>
        /// <param name="parameters">The request query string and body parameters.</param>
        /// <param name="oauthData">The <c>OAuth</c> data.</param>
        private static void GenerateSignatureForOAuth(
            string httpMethod,
            Uri uri,
            NameValueCollection parameters,
            OAuthAuthorizationData oauthData)
        {
            // Create a new set of parameters from the specified set
            NameValueCollection signatureParameters = new NameValueCollection(parameters);

            // Add the OAuth parameters to the original user parameters
            signatureParameters["oauth_consumer_key"] = oauthData.ConsumerKey;
            signatureParameters["oauth_nonce"] = oauthData.Nonce;
            signatureParameters["oauth_signature_method"] = oauthData.SignatureMethod;
            signatureParameters["oauth_timestamp"] = oauthData.Timestamp;
            signatureParameters["oauth_token"] = oauthData.Token;
            signatureParameters["oauth_version"] = oauthData.Version;

            StringBuilder parameterBuilder = new StringBuilder();

            // Keys must be signed in alphabetical order as-per the OAuth specification
            foreach (string key in signatureParameters.AllKeys.OrderBy((p) => p, StringComparer.Ordinal))
            {
                string encodedKey = Uri.EscapeDataString(key);
                string encodedValue = Uri.EscapeDataString(signatureParameters[key]);

                parameterBuilder.AppendFormat(
                    CultureInfo.InvariantCulture,
                    "{0}={1}&",
                    encodedKey,
                    encodedValue);
            }

            // Trim off the trailing '&', if any
            if (parameterBuilder.Length > 0)
            {
                parameterBuilder.Length--;
            }

            // Strip the query string parameter(s) from the URI, if any.
            // They should not be present in the hashed URI as they are
            // dealt with separately in the signature parameters collection.
            UriBuilder builder = new UriBuilder(uri);
            builder.Query = string.Empty;

            // Generate the data to sign
            string signatureBaseString = string.Format(
                CultureInfo.InvariantCulture,
                "{0}&{1}&{2}",
                httpMethod.ToUpperInvariant(),
                Uri.EscapeDataString(builder.Uri.AbsoluteUri),
                Uri.EscapeDataString(parameterBuilder.ToString()));

            // Generate the key to use to sign the data
            string signingKey = string.Format(
                CultureInfo.InvariantCulture,
                "{0}&{1}",
                Uri.EscapeDataString(oauthData.ConsumerSecret),
                Uri.EscapeDataString(oauthData.TokenSecret));

            using (HMAC algorithm = HMACSHA1.Create())
            {
                Encoding encoding = Encoding.UTF8;

                // Set the key to use for the hash
                algorithm.Key = encoding.GetBytes(signingKey);

                // Generate the hash
                byte[] buffer = encoding.GetBytes(signatureBaseString);
                byte[] hashBytes = algorithm.ComputeHash(buffer);

                // Store the base-64 representation of the hash as the signature
                oauthData.Signature = Convert.ToBase64String(hashBytes);
            }
        }

        /// <summary>
        /// A class representing data to create an <c>OAuth</c> authorization header. This class cannot be inherited.
        /// </summary>
        private sealed class OAuthAuthorizationData
        {
            /// <summary>
            /// The <see cref="DateTime"/> representing the UNIX epoch. This field is read-only.
            /// </summary>
            internal static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

            /// <summary>
            /// Initializes a new instance of the <see cref="OAuthAuthorizationData"/> class.
            /// </summary>
            /// <param name="keys">The <c>OAuth</c> keys to use.</param>
            internal OAuthAuthorizationData(IOAuthKeys keys)
            {
                this.ConsumerKey = keys.ConsumerKey;
                this.ConsumerSecret = keys.ConsumerSecret;
                this.Token = keys.Token;
                this.TokenSecret = keys.TokenSecret;

                this.SignatureMethod = "HMAC-SHA1";
                this.Version = "1.0";
                this.Timestamp = ((int)(DateTime.UtcNow - UnixEpoch).TotalSeconds).ToString(CultureInfo.InvariantCulture);
            }

            /// <summary>
            /// Gets the consumer key.
            /// </summary>
            public string ConsumerKey
            {
                get;
                private set;
            }

            /// <summary>
            /// Gets the consumer secret.
            /// </summary>
            public string ConsumerSecret
            {
                get;
                private set;
            }

            /// <summary>
            /// Gets or sets the nonce.
            /// </summary>
            public string Nonce
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the signature.
            /// </summary>
            public string Signature
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the signature method.
            /// </summary>
            public string SignatureMethod
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the timestamp.
            /// </summary>
            public string Timestamp
            {
                get;
                set;
            }

            /// <summary>
            /// Gets the token.
            /// </summary>
            public string Token
            {
                get;
                private set;
            }

            /// <summary>
            /// Gets the token secret.
            /// </summary>
            public string TokenSecret
            {
                get;
                private set;
            }

            /// <summary>
            /// Gets or sets the <c>OAuth</c> version.
            /// </summary>
            public string Version
            {
                get;
                set;
            }
        }
    }
}