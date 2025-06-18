// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OwinStartup.cs" company="http://www.martincostello.com">
//   Martin Costello (c) 2015
// </copyright>
// <summary>
//   OwinStartup.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MartinCostello.OwinStartup))]

namespace MartinCostello
{
    /// <summary>
    /// A class representing the OWIN start-up class. This class cannot be inherited.
    /// </summary>
    public partial class OwinStartup
    {
        /// <summary>
        /// Configures the application.
        /// </summary>
        /// <param name="app">The <see cref="IAppBuilder"/> to use.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "Instance method required to be called by OWIN.")]
        public void Configuration(IAppBuilder app)
        {
            AuthConfig.ConfigureAuth(app);
        }
    }
}