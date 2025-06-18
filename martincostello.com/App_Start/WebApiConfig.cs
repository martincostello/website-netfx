// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WebApiConfig.cs" company="http://www.martincostello.com">
//   Martin Costello (c) 2014
// </copyright>
// <summary>
//   WebApiConfig.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using MartinCostello.Api;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace MartinCostello
{
    /// <summary>
    /// A class representing the Web API configuration.  This class cannot be inherited.
    /// </summary>
    internal static class WebApiConfig
    {
        /// <summary>
        /// The name of the default Web API route.
        /// </summary>
        internal const string DefaultRouteName = "DefaultApi";

        /// <summary>
        /// Registers Web API with the specified <see cref="HttpConfiguration"/>.
        /// </summary>
        /// <param name="config">The HTTP configuration.</param>
        internal static void Register(HttpConfiguration config)
        {
            config.EnableCors();
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: DefaultRouteName,
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional });

            var formatter = new JsonMediaTypeFormatter();

            // Serialize and deserialize JSON as "myProperty" => "MyProperty" -> "myProperty"
            formatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            // Make JSON easier to read for debugging at the expense of larger payloads
            formatter.SerializerSettings.Formatting = Formatting.Indented;

            // Omit nulls to reduce payload size
            formatter.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;

            // Explicitly define behavior when serializing DateTime values
            formatter.SerializerSettings.DateFormatString = "yyyy'-'MM'-'dd'T'HH':'mm':'ssK";   // Only return DateTimes to a 1 second precision

            // Only support JSON for REST API requests
            config.Formatters.Clear();
            config.Formatters.Add(formatter);

            config.Services.Add(typeof(IExceptionLogger), new Log4NetExceptionLogger());

            // Add custom HTTP handler for extensibility
            config.MessageHandlers.Add(new CustomHttpMessageHandler());
        }
    }
}
