// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BrowserStackOptions.cs" company="http://www.martincostello.com">
//   Martin Costello (c) 2014-2015
// </copyright>
// <summary>
//   BrowserStackOptions.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace MartinCostello
{
    /// <summary>
    /// A class containing options to use to create an <see cref="IWebDriver"/> to use with <c>BrowserStack</c>.
    /// </summary>
    public class BrowserStackOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BrowserStackOptions"/> class.
        /// </summary>
        public BrowserStackOptions()
        {
        }

        /// <summary>
        /// Gets or sets a value indicating whether to accept expired or invalid SSL certificates.
        /// </summary>
        public bool AcceptSslCertificates
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the <c>BrowserStack</c> API key.
        /// </summary>
        public string ApiKey
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the browser version, if any.
        /// </summary>
        public string BrowserVersion
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the build name/number, if any.
        /// </summary>
        public string Build
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to use debugging.
        /// </summary>
        public bool Debug
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Operating System to use, if any.
        /// </summary>
        public string OS
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Operating System version to use, if any.
        /// </summary>
        public string OSVersion
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the project name, if any.
        /// </summary>
        public string Project
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the URI to the <c>BrowserStack</c> remote server.
        /// </summary>
        public Uri RemoteUri
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the screen resolution to use, if any.
        /// </summary>
        public string Resolution
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the <c>BrowserStack</c> user name.
        /// </summary>
        public string UserName
        {
            get;
            set;
        }
    }
}