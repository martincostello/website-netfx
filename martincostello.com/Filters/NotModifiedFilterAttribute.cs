// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NotModifiedFilterAttribute.cs" company="http://www.martincostello.com">
//   Martin Costello (c) 2014
// </copyright>
// <summary>
//   NotModifiedFilterAttribute.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Globalization;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace MartinCostello.Filters
{
    /// <summary>
    /// A filter that checks whether the content has not been modified.  This class cannot be inherited.
    /// </summary>
    /// <remarks>
    /// Based on the code from <c>http://www.58bits.com/blog/2009/07/26/asp-net-mvc-304-not-modified-filter-for-syndication-content</c>.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public sealed class NotModifiedFilterAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotModifiedFilterAttribute"/> class.
        /// </summary>
        public NotModifiedFilterAttribute()
            : base()
        {
        }

        /// <summary>
        /// Called by the ASP.NET MVC framework before the action method executes.
        /// </summary>
        /// <param name="filterContext">The filter context.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="filterContext"/> is <see langword="null"/>.
        /// </exception>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext == null)
            {
                throw new ArgumentNullException("filterContext");
            }

            if (MvcApplication.BuildTimestampValue.HasValue)
            {
                filterContext.HttpContext.Response.AddHeader("Last-Modified", MvcApplication.BuildTimestampValue.Value.ToString("R", CultureInfo.InvariantCulture));
            }

            base.OnActionExecuting(filterContext);
        }

        /// <summary>
        /// Called by the ASP.NET MVC framework after the action result executes.
        /// </summary>
        /// <param name="filterContext">The filter context.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="filterContext"/> is <see langword="null"/>.
        /// </exception>
        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            if (filterContext == null)
            {
                throw new ArgumentNullException("filterContext");
            }

            var response = filterContext.HttpContext.Response;
            var request = filterContext.HttpContext.Request;

            if (!IsSourceModified(request, response))
            {
                response.SuppressContent = true;
                response.StatusCode = (int)HttpStatusCode.NotModified;

                // Explicitly set the Content-Length header so the client doesn't wait for
                // content but keeps the connection open for other requests
                response.AddHeader("Content-Length", "0");
            }
        }

        /// <summary>
        /// Returns whether the source has been modified.
        /// </summary>
        /// <param name="request">The current HTTP request.</param>
        /// <param name="response">The current HTTP response.</param>
        /// <returns>
        /// <see langword="true"/> if the source is modified; otherwise <see langword="false"/>
        /// </returns>
        private static bool IsSourceModified(HttpRequestBase request, HttpResponseBase response)
        {
            bool wasDateModified = false;
            bool wasETagModified = false;

            string requestETagHeader = request.Headers["If-None-Match"] ?? string.Empty;
            string requestIfModifiedSinceHeader = request.Headers["If-Modified-Since"] ?? string.Empty;

            DateTime requestIfModifiedSince;

            if (!DateTime.TryParse(requestIfModifiedSinceHeader, out requestIfModifiedSince))
            {
                requestIfModifiedSince = DateTime.MinValue;
            }

            string responseETagHeader = response.Headers["ETag"] ?? string.Empty;
            string responseLastModifiedHeader = response.Headers["Last-Modified"] ?? string.Empty;

            DateTime responseLastModified;

            if (!DateTime.TryParse(responseLastModifiedHeader, out responseLastModified))
            {
                responseLastModified = DateTime.MinValue;
            }

            if (requestIfModifiedSince != DateTime.MinValue && responseLastModified != DateTime.MinValue)
            {
                if (responseLastModified > requestIfModifiedSince)
                {
                    TimeSpan diff = responseLastModified - requestIfModifiedSince;

                    if (diff > TimeSpan.FromSeconds(1))
                    {
                        wasDateModified = true;
                    }
                }
            }
            else
            {
                wasDateModified = true;
            }

            // Leave the default for eTagModified = false so that if we
            // don't get an ETag from the server we will rely on the fileDateModified only
            if (!string.IsNullOrEmpty(responseETagHeader))
            {
                wasETagModified = !string.Equals(responseETagHeader, requestETagHeader, StringComparison.Ordinal);
            }

            return wasDateModified || wasETagModified;
        }
    }
}