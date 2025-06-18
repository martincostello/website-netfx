using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using log4net;

namespace MartinCostello.Api
{
    /// <summary>
    /// Specifies that an API token is required to authorize requests.  This class cannot be inherited.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Microsoft.Design",
        "CA1019:DefineAccessorsForAttributeArguments",
        Justification = "The accessor is provider, it's just of the parsed type.")]
    public sealed class TokenAuthorizeAttribute : AuthorizeAttribute
    {
        /// <summary>
        /// The name of the HTTP authentication scheme to use for authorization.
        /// </summary>
        internal const string AuthenticationSchemeName = "bearer";

        /// <summary>
        /// The minimum length of any API key.
        /// </summary>
        private const int MinApiTokenLength = 10;

        /// <summary>
        /// The <see cref="ILog"/> to use.
        /// </summary>
        private static readonly ILog _log = LogManager.GetLogger(typeof(TokenAuthorizeAttribute));

        /// <summary>
        /// The API tokens that are authorized to access the API.
        /// </summary>
        private readonly List<string> _tokens;

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenAuthorizeAttribute"/> class.
        /// </summary>
        public TokenAuthorizeAttribute()
        {
            _tokens = new List<string>()
            {
                "YQ8xYQaH3UKMVAJ098rx+bdSRs1Z3znzZhRAnsGh/wf4tv3ngRB5iH2NRc6WOFBSlgMzoN+MiZe8JU2yzHGJGw==",
            };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenAuthorizeAttribute"/> class.
        /// </summary>
        /// <param name="collection">A collection of API tokens that are accepted for authorized users.</param>
        /// <remarks>
        /// This constructor is used for unit testing.
        /// </remarks>
        internal TokenAuthorizeAttribute(IEnumerable<string> collection)
        {
            _tokens = new List<string>(collection);
        }

        /// <summary>
        /// Calls when an action is being authorized.
        /// </summary>
        /// <param name="actionContext">The context.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="actionContext"/> is <see langword="null"/>.
        /// </exception>
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            if (actionContext == null)
            {
                throw new ArgumentNullException(nameof(actionContext));
            }

            HttpStatusCode statusCode = HttpStatusCode.Unauthorized;
            IEnumerable<string> values;

            // Try and get the API key from the HTTP request headers.
            // Do not try and look for the API key in the query string.
            if (actionContext != null &&
                actionContext.Request.Headers.Authorization != null &&
                string.Equals(AuthenticationSchemeName, actionContext.Request.Headers.Authorization.Scheme, StringComparison.OrdinalIgnoreCase))
            {
                string token = actionContext.Request.Headers.Authorization.Parameter;

                if (!string.IsNullOrEmpty(token))
                {
                    // Is the API key one of the known API tokens?
                    // N.B. The API tokens are case-sensitive.
                    if (_tokens.Contains(token, StringComparer.Ordinal))
                    {
                        statusCode = HttpStatusCode.OK;
                    }
                    else
                    {
                        // An invalid/incorrect API token was specified
                        statusCode = HttpStatusCode.Forbidden;
                    }
                }
            }

            const string UnknownIPString = "?";
            string clientIP = UnknownIPString;

            if (actionContext.Request.Headers.TryGetValues("X-Forwarded-For", out values))
            {
                clientIP = values.DefaultIfEmpty(UnknownIPString).FirstOrDefault();
            }

            if (statusCode == HttpStatusCode.OK)
            {
                _log.DebugFormat(
                    CultureInfo.InvariantCulture,
                    "Authorization succeeded for {0} request to {1} from User Agent '{2}' and IP address '{3}'.",
                    actionContext.Request.Method,
                    actionContext.Request.RequestUri,
                    actionContext.Request.Headers.UserAgent,
                    clientIP);
            }
            else
            {
                _log.WarnFormat(
                    CultureInfo.InvariantCulture,
                    "Authorization failed with HTTP status code {0} ({1}) for request from IP address '{2}' and User Agent '{3}' for {4} {5}.",
                    (int)statusCode,
                    statusCode,
                    clientIP,
                    actionContext.Request.Headers.UserAgent,
                    actionContext.Request.Method,
                    actionContext.Request.RequestUri);

                actionContext.Response = new HttpResponseMessage(statusCode);
            }
        }
    }
}
