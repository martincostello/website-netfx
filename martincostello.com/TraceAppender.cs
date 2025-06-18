namespace MartinCostello
{
    using System.Diagnostics;
    using log4net.Core;

    /// <summary>
    /// A class representing an implementation of <see cref="log4net.Appender.TraceAppender"/> that can
    /// log at more trace levels that just <c>Verbose</c>. This class cannot be inherited.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Microsoft.Naming",
        "CA1704:IdentifiersShouldBeSpelledCorrectly",
        MessageId = "Appender",
        Justification = "Matches terminology of log4net.")]
    public sealed class TraceAppender : log4net.Appender.TraceAppender
    {
        /// <inheritdoc />
        protected override void Append(LoggingEvent loggingEvent)
        {
            if (loggingEvent == null)
            {
                return;
            }

            string message = RenderLoggingEvent(loggingEvent);

            if (loggingEvent.Level == Level.Critical || loggingEvent.Level == Level.Error || loggingEvent.Level == Level.Fatal)
            {
                Trace.TraceError(message);
            }
            else if (loggingEvent.Level == Level.Warn)
            {
                Trace.TraceWarning(message);
            }
            else if (loggingEvent.Level == Level.Info)
            {
                Trace.TraceInformation(message);
            }
            else
            {
                // Equates to Verbose
                Trace.WriteLine(message, loggingEvent.LoggerName);
            }

            if (ImmediateFlush)
            {
                Trace.Flush();
            }
        }
    }
}
