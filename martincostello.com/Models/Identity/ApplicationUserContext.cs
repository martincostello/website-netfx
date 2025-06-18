// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplicationUserContext.cs" company="http://www.martincostello.com">
//   Martin Costello (c) 2015
// </copyright>
// <summary>
//   ApplicationUserContext.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Configuration;
using ElCamino.AspNet.Identity.AzureTable;
using ElCamino.AspNet.Identity.AzureTable.Model;

namespace MartinCostello.Models.Identity
{
    /// <summary>
    /// A class representing an Azure Table Storage-based store for application users.
    /// </summary>
    public class ApplicationUserContext : IdentityCloudContext<ApplicationUser>
    {
        /// <summary>
        /// The lazily-initialized instance of <see cref="IdentityConfiguration"/>. This field is read-only.
        /// </summary>
        private static readonly Lazy<IdentityConfiguration> Configuration = new Lazy<IdentityConfiguration>(CreateConfiguration);

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationUserContext"/> class.
        /// </summary>
        /// <param name="config">The configuration to use.</param>
        public ApplicationUserContext(IdentityConfiguration config)
            : base(config)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="ApplicationUserContext"/>.
        /// </summary>
        /// <returns>
        /// The created instance of <see cref="ApplicationUserContext"/>.
        /// </returns>
        internal static ApplicationUserContext Create()
        {
            return new ApplicationUserContext(Configuration.Value);
        }

        /// <summary>
        /// Creates a new instance of <see cref="IdentityConfiguration"/>.
        /// </summary>
        /// <returns>
        /// The created instance of <see cref="IdentityConfiguration"/>.
        /// </returns>
        private static IdentityConfiguration CreateConfiguration()
        {
            return new IdentityConfiguration()
            {
                StorageConnectionString = ConfigurationManager.ConnectionStrings["AzureStorageAccount"].ConnectionString,
                TablePrefix = ConfigurationManager.AppSettings["AspNetIdentity:TablePrefix"],
            };
        }
    }
}