namespace MartinCostello
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using log4net;

    /// <summary>
    /// A class containing extension methods for the <see cref="ILog"/> interface.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class ILogExtensions
    {
        /// <summary>
        /// Logs a message for the specified exception at the Error level.
        /// </summary>
        /// <param name="value">The <see cref="ILog"/> to log the exception to.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="value"/> is <see langword="null"/>.
        /// </exception>
        public static void ErrorFormat(this ILog value, Exception exception, string format, params object[] args)
        {
            if (value != null)
            {
                value.Error(string.Format(CultureInfo.InvariantCulture, format, args), exception);
            }
        }
    }
}
