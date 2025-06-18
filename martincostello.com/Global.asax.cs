// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Global.asax.cs" company="http://www.martincostello.com">
//   Martin Costello (c) 2014
// </copyright>
// <summary>
//   Global.asax.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Configuration;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using log4net;

namespace MartinCostello
{
    /// <summary>
    /// A class representing the MVC application.
    /// </summary>
    public partial class MvcApplication : HttpApplication
    {
        /// <summary>
        /// The <see cref="ILog"/> to use. This field is read-only.
        /// </summary>
        private static readonly ILog Log = LogManager.GetLogger(typeof(MvcApplication));

        /// <summary>
        /// The copyright to use for the application. This field is read-only.
        /// </summary>
        private static readonly string _copyright = typeof(MvcApplication).Assembly.GetCustomAttribute<AssemblyCopyrightAttribute>().Copyright;

        /// <summary>
        /// The application version. This field is read-only.
        /// </summary>
        private static readonly string _version = typeof(MvcApplication).Assembly.GetCustomAttribute<AssemblyFileVersionAttribute>().Version;

        /// <summary>
        /// The date and time the application was started.  This field is read-only.
        /// </summary>
        private static readonly DateTime _startupTime = DateTime.UtcNow;

        /// <summary>
        /// A <see cref="Stopwatch"/> that records the amount of time the website has been up for. This field is read-only.
        /// </summary>
        private static readonly Stopwatch _uptimeTimer = Stopwatch.StartNew();

        /// <summary>
        /// Initializes a new instance of the <see cref="MvcApplication"/> class.
        /// </summary>
        public MvcApplication()
            : base()
        {
        }

        /// <summary>
        /// Gets the build label for the website, if any.
        /// </summary>
        public static string BuildLabel
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the build timestamp for the website, if any.
        /// </summary>
        public static string BuildTimestamp
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the parsed build timestamp value for the website, if any.
        /// </summary>
        public static DateTime? BuildTimestampValue
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the copyright for the website.
        /// </summary>
        public static string Copyright
        {
            get { return _copyright; }
        }

        /// <summary>
        /// Gets the date and time the application was started.
        /// </summary>
        public static DateTime StartupTime
        {
            get { return _startupTime; }
        }

        /// <summary>
        /// Gets the version for the website.
        /// </summary>
        public static string Version
        {
            get { return _version; }
        }

        /// <summary>
        /// Gets the period of time for which the website has been running.
        /// </summary>
        public static TimeSpan WebsiteUptime
        {
            get { return _uptimeTimer.Elapsed; }
        }

        /// <summary>
        /// Gets the Google API key.
        /// </summary>
        public static string GoogleApiKey
        {
            get { return ConfigurationManager.AppSettings["GoogleMaps:ApiKey"]; }
        }

        /// <summary>
        /// Handles the Start event of the HTTP Application.
        /// </summary>
        [ExcludeFromCodeCoverage]
        [SuppressMessage(
            "Microsoft.Maintainability",
            "CA1506:AvoidExcessiveClassCoupling",
            Justification = "Class coupling is OK.")]
        [SuppressMessage(
            "Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "Required so can be called by the IIS pipeline.")]
        protected void Application_Start()
        {
            AppDomain.CurrentDomain.UnhandledException += AppDomainUnhandledException;
            TaskScheduler.UnobservedTaskException += UnobservedTaskException;

            try
            {
                LogConfig.Configure();
                AreaRegistration.RegisterAllAreas();
                GlobalConfiguration.Configure(WebApiConfig.Register);
                FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
                RouteConfig.RegisterRoutes(RouteTable.Routes);
                BundleConfig.RegisterBundles(BundleTable.Bundles);

                ViewEngines.Engines.Clear();
                ViewEngines.Engines.Add(new RazorViewEngine());

                MvcHandler.DisableMvcResponseHeader = true;

#if !DEBUG
            BundleTable.EnableOptimizations = true;
#endif

                Assembly thisAssembly = typeof(MvcApplication).Assembly;

                string buildLabelString = thisAssembly
                    .GetCustomAttributes<AssemblyMetadataAttribute>()
                    .Where((p) => string.Equals("BuildLabel", p.Key, StringComparison.OrdinalIgnoreCase))
                    .Select((p) => p.Value)
                    .FirstOrDefault();

                string buildTimestampString = thisAssembly
                    .GetCustomAttributes<AssemblyMetadataAttribute>()
                    .Where((p) => string.Equals("BuildTimestamp", p.Key, StringComparison.OrdinalIgnoreCase))
                    .Select((p) => p.Value)
                    .FirstOrDefault();

                if (!string.IsNullOrWhiteSpace(buildLabelString))
                {
                    BuildLabel = buildLabelString;
                }

                if (!string.IsNullOrWhiteSpace(buildTimestampString))
                {
                    BuildTimestamp = buildTimestampString;

                    DateTime buildTimestampValue;

                    if (DateTime.TryParseExact(buildTimestampString, "r", CultureInfo.InvariantCulture, DateTimeStyles.None, out buildTimestampValue))
                    {
                        BuildTimestampValue = buildTimestampValue;
                    }
                }

                Log.InfoFormat(CultureInfo.InvariantCulture, "Started martincostello.com {0} at {1:u}.", Version, DateTime.UtcNow);
            }
            catch (Exception ex)
            {
                Log.Error("Failed to initialize martincostello.com.", ex);
                throw;
            }
        }

        /// <summary>
        /// Handles the <c>Application_End</c> event.
        /// </summary>
        [ExcludeFromCodeCoverage]
        [SuppressMessage(
            "Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "Must be an instance method to be called by IIS.")]
        protected void Application_End()
        {
            Log.InfoFormat(CultureInfo.InvariantCulture, "Shut down martincostello.com {0} at {1:u}.", Version, DateTime.UtcNow);
        }

        /// <summary>
        /// Handles the Error event of the HTTP Application.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "Required so can be called by the IIS pipeline.")]
        protected void Application_Error()
        {
            Exception error = Server.GetLastError();

            if (error != null)
            {
                HttpException httpError = error as HttpException;

                if (httpError == null)
                {
                    Log.Error(error.Message, error);
                }
                else
                {
                    int httpCode = httpError.GetHttpCode();

                    // Don't log HTTP 404s
                    if (httpCode == 401 || httpCode == 403)
                    {
                        Log.WarnFormat(
                            CultureInfo.InvariantCulture,
                            "HTTP {0}: Access denied to {1} {2} (Source: {3}; User Agent: '{4}')",
                            httpCode,
                            Request.HttpMethod,
                            Request.Url,
                            Request.UserHostAddress,
                            Request.UserAgent);
                    }
                    else if (httpCode == 400)
                    {
                        Log.ErrorFormat(
                            CultureInfo.InvariantCulture,
                            "HTTP {0}: Bad request to {1} {2} (Source: {3}; User Agent: '{4}'). {5}",
                            httpCode,
                            Request.HttpMethod,
                            Request.Url,
                            Request.UserHostAddress,
                            Request.UserAgent,
                            httpError.Message);
                    }
                    else if (httpCode != 404)
                    {
                        Log.ErrorFormat(
                            CultureInfo.InvariantCulture,
                            "HTTP {0}: Error servicing request to {1} {2} (Source: {3}; User Agent: '{4}'). {5}",
                            httpCode,
                            Request.HttpMethod,
                            Request.Url,
                            Request.UserHostAddress,
                            Request.UserAgent,
                            httpError.Message);
                    }
                }
            }
        }

        /// <summary>
        /// Handles the <see cref="AppDomain.UnhandledException"/> event of the <see cref="TaskScheduler"/> class.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="UnhandledExceptionEventArgs"/> instance containing the event data.</param>
        [ExcludeFromCodeCoverage]
        private static void AppDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e != null)
            {
                Exception ex = e.ExceptionObject as Exception;

                if (ex != null)
                {
                    Log.Error("Unhandled AppDomain exception.", ex);
                }
            }
        }

        /// <summary>
        /// Handles the <see cref="TaskScheduler.UnobservedTaskException"/> event of the <see cref="TaskScheduler"/> class.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="UnobservedTaskExceptionEventArgs"/> instance containing the event data.</param>
        [ExcludeFromCodeCoverage]
        private static void UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            if (e != null && e.Exception != null)
            {
                Log.Error("Unobserved task exception.", e.Exception);
            }
        }
    }
}
