// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CustomHandleErrorAttribute.cs" company="http://www.martincostello.com">
//   Martin Costello (c) 2014
// </copyright>
// <summary>
//   CustomHandleErrorAttribute.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Globalization;
using System.Web;
using System.Web.Mvc;
using log4net;

namespace MartinCostello.Filters
{
    /// <summary>
    /// Represents an attribute that handles errors that occur in the Customer Portal.  This class cannot be inherited.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public sealed class CustomHandleErrorAttribute : HandleErrorAttribute
    {
        /// <summary>
        /// The <see cref="ILog"/> to use. This field is read-only.
        /// </summary>
        private static readonly ILog Log = LogManager.GetLogger(typeof(CustomHandleErrorAttribute));

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomHandleErrorAttribute" /> class.
        /// </summary>
        public CustomHandleErrorAttribute()
            : base()
        {
        }

        /// <summary>
        /// Called when an exception occurs.
        /// </summary>
        /// <param name="filterContext">The action-filter context.</param>
        public override void OnException(ExceptionContext filterContext)
        {
            // N.B. This implementation is copied from the base class, but with
            // some modifications to wrap the exception to add extra detail and log.
            if (filterContext == null)
            {
                throw new ArgumentNullException("filterContext");
            }

            if (filterContext.IsChildAction)
            {
                return;
            }

            // If custom errors are disabled, we need to let the normal ASP.NET exception handler
            // execute so that the user can see useful debugging information.
            if (filterContext.ExceptionHandled ||
                !filterContext.HttpContext.IsCustomErrorEnabled)
            {
                return;
            }

            Exception exception = filterContext.Exception;

            if (!this.ExceptionType.IsInstanceOfType(exception))
            {
                // Not the type of exception we want to handle
                return;
            }

            int httpCode = new HttpException(null, exception).GetHttpCode();

            // If this is not an HTTP 500 (for example, if somebody throws an HTTP 404 from an action method), ignore it.
            if (httpCode != 500)
            {
                return;
            }

            bool isBadRequestException =
                filterContext.Exception is HttpRequestValidationException ||
                filterContext.Exception.InnerException is HttpRequestValidationException ||
                filterContext.Exception.InnerException is HttpAntiForgeryException;

            if (isBadRequestException)
            {
                Log.ErrorFormat(
                    CultureInfo.InvariantCulture,
                    "HTTP {0}: Bad request to {1} {2} (Source: {3}; User Agent: '{4}'). {5}",
                    400,
                    filterContext.HttpContext.Request.HttpMethod,
                    filterContext.HttpContext.Request.Url,
                    filterContext.HttpContext.Request.UserHostAddress,
                    filterContext.HttpContext.Request.UserAgent,
                    exception.Message);
            }
            else
            {
                Log.ErrorFormat(
                    exception,
                    "Unhandled exception of type {0} processing request for {1} {2} from IP {3} (User Agent: '{4}'). {5}",
                    exception.GetType().FullName,
                    filterContext.HttpContext.Request.HttpMethod,
                    filterContext.HttpContext.Request.Url,
                    filterContext.HttpContext.Request.UserHostAddress,
                    filterContext.HttpContext.Request.UserAgent,
                    exception.Message);
            }

            string controllerName = string.Empty;
            string actionName = string.Empty;

            if (filterContext.RouteData != null)
            {
                controllerName = filterContext.RouteData.Values["controller"] as string;
                actionName = filterContext.RouteData.Values["action"] as string;
            }

            HandleErrorInfo model = new HandleErrorInfo(filterContext.Exception, controllerName, actionName);

            filterContext.Result = new ViewResult
            {
                ViewName = this.View,
                MasterName = this.Master,
                ViewData = new ViewDataDictionary<HandleErrorInfo>(model),
                TempData = filterContext.Controller == null ? new TempDataDictionary() : filterContext.Controller.TempData,
            };

            filterContext.ExceptionHandled = true;

            if (filterContext.HttpContext.Response != null)
            {
                filterContext.HttpContext.Response.Clear();

                if (isBadRequestException)
                {
                    // Improve the UX by stating its a "Bad Request" instead of a server error
                    filterContext.HttpContext.Response.StatusCode = 400;
                }
                else
                {
                    filterContext.HttpContext.Response.StatusCode = 500;
                }

                // Certain versions of IIS will sometimes use their own error page when
                // they detect a server error. Setting this property indicates that we
                // want it to try to render ASP.NET MVC's error page instead.
                filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
            }
        }
    }
}