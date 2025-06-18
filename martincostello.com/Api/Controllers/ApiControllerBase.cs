// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApiControllerBase.cs" company="http://www.martincostello.com">
//   Martin Costello (c) 2014-2015
// </copyright>
// <summary>
//   ApiControllerBase.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace MartinCostello.Api.Controllers
{
    using System;
    using System.Globalization;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Http;
    using log4net;
    using MartinCostello.Api.Models;

    /// <summary>
    /// The base class for API controllers.
    /// </summary>
    public abstract class ApiControllerBase : ApiController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApiControllerBase"/> class.
        /// </summary>
        protected ApiControllerBase()
            : base()
        {
            Log = LogManager.GetLogger(GetType());
        }

        /// <summary>
        /// Gets the <see cref="ILog"/> to use.
        /// </summary>
        protected internal ILog Log
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the current request Id.
        /// </summary>
        protected internal string RequestId
        {
            get { return Request.Properties[CustomHttpMessageHandler.RequestIdHeaderName] as string; }
        }

        /// <summary>
        /// Creates response for a bad request with the specified error code and reason.
        /// </summary>
        /// <param name="errorCode">The error code associated with the error.</param>
        /// <param name="reason">The reason to use when creating the response.</param>
        /// <returns>
        /// The created instance of <see cref="HttpResponseMessage"/> representing the error.
        /// </returns>
        protected HttpResponseMessage CreateBadResponse(string errorCode, string reason)
        {
            return CreateErrorResponse(HttpStatusCode.BadRequest, errorCode, reason);
        }

        /// <summary>
        /// Creates response for an entity that has been created.
        /// </summary>
        /// <typeparam name="T">The type of the content of the HTTP response message.</typeparam>
        /// <param name="value">The content of the HTTP response message.</param>
        /// <param name="routeValues">The route values to use create the <c>Location</c> header for the created resource.</param>
        /// <returns>
        /// The created instance of <see cref="HttpResponseMessage"/> representing the error.
        /// </returns>
        protected HttpResponseMessage CreateCreatedResponse<T>(T value, object routeValues)
        {
            var response = Request.CreateResponse(HttpStatusCode.Created, value);

            try
            {
                string locationUri = Url.Link(WebApiConfig.DefaultRouteName, routeValues);
                response.Headers.Location = new Uri(locationUri);
                return response;
            }
            catch (Exception)
            {
                response.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Creates response for an entity that already exists with an optional reason.
        /// </summary>
        /// <param name="reason">The optional reason to use when creating the response.</param>
        /// <returns>
        /// The created instance of <see cref="HttpResponseMessage"/> representing the error.
        /// </returns>
        protected HttpResponseMessage CreateExistsResponse(string reason = null)
        {
            return CreateErrorResponse(HttpStatusCode.Conflict, ErrorCodes.Exists, reason ?? "Exists.");
        }

        /// <summary>
        /// Creates response for an entity that is not found with an optional reason.
        /// </summary>
        /// <param name="reason">The optional reason to use when creating the response.</param>
        /// <returns>
        /// The created instance of <see cref="HttpResponseMessage"/> representing the error.
        /// </returns>
        protected HttpResponseMessage CreateInternalErrorResponse(string reason = null)
        {
            return CreateErrorResponse(
                HttpStatusCode.InternalServerError,
                ErrorCodes.InternalError,
                reason ?? "Internal server error.");
        }

        /// <summary>
        /// Creates response for an entity that is not found with an optional reason.
        /// </summary>
        /// <param name="reason">The optional reason to use when creating the response.</param>
        /// <returns>
        /// The created instance of <see cref="HttpResponseMessage"/> representing the error.
        /// </returns>
        protected HttpResponseMessage CreateNotFoundResponse(string reason = null)
        {
            return CreateErrorResponse(HttpStatusCode.NotFound, ErrorCodes.NotFound, reason ?? "Not found.");
        }

        /// <summary>
        /// Creates response for a resource that is not implemented.
        /// </summary>
        /// <param name="reason">The optional reason to use when creating the response.</param>
        /// <returns>
        /// The created instance of <see cref="HttpResponseMessage"/> representing the error.
        /// </returns>
        protected HttpResponseMessage CreateNotImplementedResponse(string reason = null)
        {
            return CreateErrorResponse(HttpStatusCode.NotImplemented, ErrorCodes.NotImplemented, reason ?? "Not implemented.");
        }

        /// <summary>
        /// Creates response for a resource that times out while performing a request.
        /// </summary>
        /// <param name="reason">The optional reason to use when creating the response.</param>
        /// <returns>
        /// The created instance of <see cref="HttpResponseMessage"/> representing the error.
        /// </returns>
        protected HttpResponseMessage CreateTimeoutResponse(string reason = null)
        {
            return CreateErrorResponse(HttpStatusCode.RequestTimeout, ErrorCodes.Timeout, reason ?? "The request timed out.");
        }

        /// <summary>
        /// Creates an error response with the specified HTTP status code, error code and reason.
        /// </summary>
        /// <param name="statusCode">The HTTP status code associated with the error.</param>
        /// <param name="errorCode">The error code associated with the error.</param>
        /// <param name="reason">The reason to use when creating the response.</param>
        /// <returns>
        /// The created instance of <see cref="HttpResponseMessage"/> representing the error.
        /// </returns>
        protected HttpResponseMessage CreateErrorResponse(HttpStatusCode statusCode, string errorCode, string reason)
        {
            var detail = new ErrorDetail()
            {
                ErrorCode = errorCode,
                Reason = reason,
                RequestId = RequestId,
                StatusCode = (int)statusCode,
            };

            return Request.CreateResponse(statusCode, detail);
        }

        /// <summary>
        /// Handles an error.
        /// </summary>
        /// <param name="exception">The exception to handle.</param>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        /// <returns>
        /// A <see cref="HttpResponseMessage"/> representing the error.
        /// </returns>
        protected HttpResponseMessage HandleError(Exception exception, string format, params object[] args)
        {
            if (exception is TaskCanceledException || exception is TimeoutException)
            {
                Log.Warn(string.Format(CultureInfo.InvariantCulture, format, args), exception);
                return CreateTimeoutResponse();
            }
            else
            {
                Log.Error(string.Format(CultureInfo.InvariantCulture, format, args), exception);
                return CreateInternalErrorResponse();
            }
        }
    }
}
