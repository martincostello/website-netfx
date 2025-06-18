// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CustomHttpMessageHandler.cs" company="http://www.martincostello.com">
//   Martin Costello (c) 2014-2015
// </copyright>
// <summary>
//   CustomHttpMessageHandler.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace MartinCostello.Api
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Formatting;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.Http;
    using System.Web.Http.Controllers;
    using System.Web.Http.Hosting;
    using MartinCostello.Api.Models;

    /// <summary>
    /// A class representing a HTTP request handler that adds custom HTTP headers to
    /// the HTTP responses returned to the client.  This class cannot be inherited.
    /// </summary>
    public sealed class CustomHttpMessageHandler : DelegatingHandler
    {
        /// <summary>
        /// The name of the custom HTTP response header for the request Id.
        /// </summary>
        internal const string RequestIdHeaderName = "X-Request-Id";

        /// <summary>
        /// The name of the custom HTTP response header for the request duration.
        /// </summary>
        private const string RequestDurationHeaderName = "X-Request-Duration";

        /// <summary>
        /// The name of the custom HTTP response header to override the HTTP verb.
        /// </summary>
        private const string MethodOverrideHeader = "X-HTTP-Method-Override";

        /// <summary>
        /// The HTTP verbs that support being overridden from a POST verb. This field is read-only.
        /// </summary>
        private static readonly string[] SupportedOverrideMethods = new[] { "DELETE", "HEAD", "PATCH", "PUT" };

        /// <inheritdoc />
        protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            // Generate an Id for the request and place in the properties for use elsewhere
            string requestId = request.GetCorrelationId().ToString();
            request.Properties[RequestIdHeaderName] = requestId;

            IEnumerable<string> values;

            // Check for HTTP POST with the X-HTTP-Method-Override header
            if (request.Method == HttpMethod.Post &&
                request.Headers.TryGetValues(MethodOverrideHeader, out values) &&
                values != null)
            {
                // Is the header specified a supported override?
                string overrideMethod = values.FirstOrDefault();

                if (SupportedOverrideMethods.Contains(overrideMethod, StringComparer.OrdinalIgnoreCase))
                {
                    // Change the request method to the one specified
                    request.Method = new HttpMethod(overrideMethod.ToUpperInvariant());
                }
            }

            // Process the rest of the request
            HttpResponseMessage response = await base.SendAsync(request, cancellationToken);

            // Add the request Id as a response header
            response.Headers.Add(RequestIdHeaderName, requestId);

            if (!response.IsSuccessStatusCode)
            {
                CustomizeErrorIfWebApiDefault(
                    response,
                    requestId,
                    () => ((HttpRequestContext)request.Properties[HttpPropertyKeys.RequestContextKey]).Configuration.Formatters.JsonFormatter);
            }

            stopwatch.Stop();
            response.Headers.Add(RequestDurationHeaderName, stopwatch.Elapsed.TotalMilliseconds.ToString("0.00ms", System.Globalization.CultureInfo.InvariantCulture));

            return response;
        }

        /// <summary>
        /// Customizes the error response if a Web API default error response is returned.
        /// </summary>
        /// <param name="response">The HTTP response message to customize.</param>
        /// <param name="requestId">The current request Id.</param>
        /// <param name="formatterProvider">A delegate to a method to use to get the media type formatter to use.</param>
        private static void CustomizeErrorIfWebApiDefault(
            HttpResponseMessage response,
            string requestId,
            Func<MediaTypeFormatter> formatterProvider)
        {
            if (response.Content == null)
            {
                response.Content = CreateErrorContent(requestId, response.StatusCode, formatterProvider());
            }
            else
            {
                ObjectContent<HttpError> defaultError = response.Content as ObjectContent<HttpError>;

                // Is the error an error generated by the ASP.NET Web API code itself?
                if (defaultError != null)
                {
                    HttpError httpError = defaultError.Value as HttpError;

                    // Was there some error detail?
                    if (httpError != null)
                    {
                        // If so, override it...
                        response.Content = CreateErrorContent(requestId, response.StatusCode, formatterProvider(), httpError.Message);

                        // ...and dispose of the old one
                        defaultError.Dispose();
                    }
                }
            }
        }

        /// <summary>
        /// Creates an <see cref="HttpContent"/> representing an error.
        /// </summary>
        /// <param name="requestId">The request Id.</param>
        /// <param name="statusCode">The HTTP status code.</param>
        /// <param name="formatter">The media type formatter to use.</param>
        /// <param name="reason">The optional error reason.</param>
        /// <returns>
        /// The created instance of <see cref="HttpContent"/>.
        /// </returns>
        private static HttpContent CreateErrorContent(
            string requestId,
            HttpStatusCode statusCode,
            MediaTypeFormatter formatter,
            string reason = null)
        {
            var value = new ErrorDetail()
            {
                ErrorCode = statusCode.ToString(),
                Reason = reason,
                RequestId = requestId,
                StatusCode = (int)statusCode,
            };

            return new ObjectContent<ErrorDetail>(value, formatter);
        }
    }
}
