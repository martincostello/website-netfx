using System.Globalization;
using System.Web.Http.ExceptionHandling;
using log4net;

namespace MartinCostello.Api
{
    /// <summary>
    /// A class representing an implementation of <see cref="ExceptionLogger"/> that logs to <c>log4net</c>. This class cannot be inherited.
    /// </summary>
    internal sealed class Log4NetExceptionLogger : ExceptionLogger
    {
        /// <summary>
        /// The <see cref="ILog"/> to use. This field is read-only.
        /// </summary>
        private static readonly ILog Logger = LogManager.GetLogger(typeof(Log4NetExceptionLogger));

        /// <inheritdoc/>
        public override void Log(ExceptionLoggerContext context)
        {
            if (context != null &&
                context.Exception != null &&
                context.Request != null)
            {
                string message = string.Format(
                    CultureInfo.InvariantCulture,
                    "Error processing HTTP API request '{0}': {1}",
                    context.Request.RequestUri,
                    context.Exception.Message);

                Logger.Error(message, context.Exception);
            }
        }
    }
}
