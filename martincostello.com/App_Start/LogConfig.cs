// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogConfig.cs" company="http://www.martincostello.com">
//   Martin Costello (c) 2014-2015
// </copyright>
// <summary>
//   LogConfig.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace MartinCostello
{
    using System;
    using System.Configuration;
    using System.Linq;
    using System.Reflection;
    using log4net;
    using log4net.Config;
    using log4net.Core;

    /// <summary>
    /// A class containing a method that configures logging.  This class cannot be inherited.
    /// </summary>
    internal static class LogConfig
    {
        /// <summary>
        /// Configures logging.
        /// </summary>
        internal static void Configure()
        {
            GlobalContext.Properties["buildVersion"] = MvcApplication.Version;

            XmlConfigurator.Configure();

            // Is there an <appSettings> override in effect?
            var overrideLevelString = ConfigurationManager.AppSettings["Logging:RootLevel"] ?? string.Empty;

            Level overrideLevel = null;

            if (!string.IsNullOrEmpty(overrideLevelString))
            {
                var levelField = typeof(Level)
                    .GetFields(BindingFlags.Static | BindingFlags.Public)
                    .Where((p) => string.Equals(overrideLevelString, p.Name, StringComparison.OrdinalIgnoreCase))
                    .FirstOrDefault();

                if (levelField != null)
                {
                    overrideLevel = (Level)levelField.GetValue(null);
                }
            }

            // If an override was specified, apply it
            if (overrideLevel != null)
            {
                var hierarchy = (log4net.Repository.Hierarchy.Hierarchy)LogManager.GetRepository();
                hierarchy.Root.Level = overrideLevel;
                hierarchy.RaiseConfigurationChanged(EventArgs.Empty);
            }
        }
    }
}
