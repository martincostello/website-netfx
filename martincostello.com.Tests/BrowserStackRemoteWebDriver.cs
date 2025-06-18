// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BrowserStackRemoteWebDriver.cs" company="http://www.martincostello.com">
//   Martin Costello (c) 2014-2015
// </copyright>
// <summary>
//   BrowserStackRemoteWebDriver.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace MartinCostello
{
    /// <summary>
    /// A class representing an implementation of <see cref="RemoteWebDriver"/> for use with <c>BrowserStack</c>.
    /// </summary>
    public class BrowserStackRemoteWebDriver : RemoteWebDriver
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BrowserStackRemoteWebDriver"/> class.
        /// </summary>
        /// <param name="remoteAddress">URI containing the address of the <c>WebDriver</c> remote server.</param>
        /// <param name="desiredCapabilities">An <see cref="ICapabilities"/> object containing the desired capabilities of the browser.</param>
        public BrowserStackRemoteWebDriver(Uri remoteAddress, DesiredCapabilities desiredCapabilities)
            : base(remoteAddress, desiredCapabilities)
        {
        }
    }
}