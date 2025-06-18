// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HttpResponseHeadersModule.cs" company="http://www.martincostello.com">
//   Martin Costello (c) 2014
// </copyright>
// <summary>
//   HttpResponseHeadersModule.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Web;

namespace MartinCostello
{
    /// <summary>
    /// A class representing an HTTP module that sets custom HTTP response headers.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Microsoft.Performance",
        "CA1812:AvoidUninstantiatedInternalClasses",
        Justification = "Instantiated using reflection by IIS.")]
    internal sealed class HttpResponseHeadersModule : IHttpModule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HttpResponseHeadersModule"/> class.
        /// </summary>
        internal HttpResponseHeadersModule()
        {
        }

        /// <summary>
        /// Disposes of the resources (other than memory) used by the module that implements <see cref="IHttpModule" />.
        /// </summary>
        public void Dispose()
        {
            // Nothing to dispose of
        }

        /// <summary>
        /// Initializes a module and prepares it to handle requests.
        /// </summary>
        /// <param name="context">An <see cref="HttpApplication" /> that provides access to the methods, properties, and events common to all application objects within an ASP.NET application.</param>
        public void Init(HttpApplication context)
        {
            if (context != null)
            {
                context.PreSendRequestHeaders += OnPreSendRequestHeaders;
            }
        }

        /// <summary>
        /// Handles the <see cref="HttpApplication.PreSendRequestHeaders"/> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private static void OnPreSendRequestHeaders(object sender, EventArgs e)
        {
            var context = HttpContext.Current;
            context.Response.Headers.Remove("Server");
            context.Response.Headers["X-Label"] = MvcApplication.BuildLabel ?? string.Empty;
            context.Response.Headers["X-Timestamp"] = MvcApplication.BuildTimestamp ?? string.Empty;
            context.Response.Headers["X-Instance"] = Environment.MachineName;
        }
    }
}
