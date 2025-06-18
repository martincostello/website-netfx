// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ErrorController.cs" company="http://www.martincostello.com">
//   Martin Costello (c) 2014
// </copyright>
// <summary>
//   ErrorController.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Net;
using System.Web.Mvc;

namespace MartinCostello.Controllers
{
    /// <summary>
    /// A class representing the controller for the <c>Error</c> views.
    /// </summary>
    public class ErrorController : Controller
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorController"/> class.
        /// </summary>
        public ErrorController()
            : base()
        {
        }

        /// <summary>
        /// The action method for the BadRequest view.
        /// </summary>
        /// <returns>
        /// The action result.
        /// </returns>
        public ActionResult BadRequest()
        {
            return Index((int)HttpStatusCode.BadRequest);
        }

        /// <summary>
        /// The action method for the Index view.
        /// </summary>
        /// <param name="status">The HTTP status code.</param>
        /// <returns>
        /// The action result.
        /// </returns>
        public ActionResult Index(int? status)
        {
            HttpStatusCode httpCode =
                status.HasValue ?
                (HttpStatusCode)status.Value :
                HttpStatusCode.InternalServerError;

            if (!Enum.IsDefined(typeof(HttpStatusCode), httpCode))
            {
                httpCode = HttpStatusCode.InternalServerError;
            }

            this.Response.StatusCode = (int)httpCode;

            string message;
            string title = null;

            switch (httpCode)
            {
                case HttpStatusCode.BadRequest:
                    message = "The specified request is invalid.";
                    title = "Bad Request";
                    break;

                case HttpStatusCode.MethodNotAllowed:
                    message = "The specified HTTP method is not allowed.";
                    break;

                case HttpStatusCode.ServiceUnavailable:
                    message = "Your request cannot be processed at this time.";
                    break;

                case HttpStatusCode.NotFound:
                    message = "The page you requested could not be found.";
                    title = "Not Found";
                    break;

                default:
                    message = "An error occurred while processing your request.";
                    break;
            }

            this.ViewBag.Message = message;
            this.ViewBag.StatusCode = this.Response.StatusCode;
            this.ViewBag.Title = title;

            return View("Error");
        }

        /// <summary>
        /// The action method for the InternalServerError view.
        /// </summary>
        /// <returns>
        /// The action result.
        /// </returns>
        public ActionResult InternalServerError()
        {
            return Index((int)HttpStatusCode.InternalServerError);
        }

        /// <summary>
        /// The action method for the MethodNotAllowed view.
        /// </summary>
        /// <returns>
        /// The action result.
        /// </returns>
        public ActionResult MethodNotAllowed()
        {
            return Index((int)HttpStatusCode.MethodNotAllowed);
        }

        /// <summary>
        /// The action method for the NotFound view.
        /// </summary>
        /// <returns>
        /// The action result.
        /// </returns>
        public ActionResult NotFound()
        {
            return Index((int)HttpStatusCode.NotFound);
        }

        /// <summary>
        /// The action method for the ServiceUnavailable view.
        /// </summary>
        /// <returns>
        /// The action result.
        /// </returns>
        public ActionResult ServiceUnavailable()
        {
            return Index((int)HttpStatusCode.ServiceUnavailable);
        }
    }
}