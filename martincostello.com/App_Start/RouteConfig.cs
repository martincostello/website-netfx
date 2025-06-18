// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RouteConfig.cs" company="http://www.martincostello.com">
//   Martin Costello (c) 2014
// </copyright>
// <summary>
//   RouteConfig.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Web.Mvc;
using System.Web.Routing;

namespace MartinCostello
{
    /// <summary>
    /// A class that configures the MVC routes. This class cannot be inherited.
    /// </summary>
    internal static class RouteConfig
    {
        /// <summary>
        /// Registers the routes for the website in the specified collection.
        /// </summary>
        /// <param name="collection">The collection to register routes in.</param>
        internal static void RegisterRoutes(RouteCollection collection)
        {
            collection.LowercaseUrls = true;
            collection.AppendTrailingSlash = true;

            collection.IgnoreRoute("{resource}.axd/{*pathInfo}");
            collection.IgnoreRoute("fonts/{*pathInfo}");

            collection.MapRoute(
                name: "Sitemap",
                url: "sitemap.html/{*pathInfo}",
                defaults: new { controller = "Sitemap", action = "Index", id = UrlParameter.Optional });

            collection.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional });
        }
    }
}
