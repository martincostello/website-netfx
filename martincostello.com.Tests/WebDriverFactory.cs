// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WebDriverFactory.cs" company="http://www.martincostello.com">
//   Martin Costello (c) 2014
// </copyright>
// <summary>
//   WebDriverFactory.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Safari;

namespace MartinCostello
{
    /// <summary>
    /// A class representing a factory for creating instances of <see cref="IWebDriver"/>.
    /// </summary>
    public class WebDriverFactory : IWebDriverFactory
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WebDriverFactory"/> class.
        /// </summary>
        public WebDriverFactory()
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="IWebDriver" />.
        /// </summary>
        /// <param name="options">The options to use to create the instance.</param>
        /// <param name="context">The <see cref="TestContext" /> associated with the current test.</param>
        /// <returns>
        /// The created instance of <see cref="IWebDriver" />.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="options"/> is <see langword="null"/>.
        /// </exception>
        public IWebDriver Create(WebDriverFactoryOptions options, TestContext context)
        {
            if (options == null)
            {
                throw new ArgumentNullException("options");
            }

            if (options.BrowserStackOptions == null)
            {
                return CreateLocalDriver(options);
            }
            else
            {
                return CreateBrowserStackDriver(options, context);
            }
        }

        /// <summary>
        /// Creates an instance of <see cref="IWebDriver"/> for using <c>BrowserStack</c>.
        /// </summary>
        /// <param name="options">The options to use to create the instance.</param>
        /// <param name="context">The <see cref="TestContext" /> associated with the current test.</param>
        /// <returns>
        /// The created instance of <see cref="IWebDriver" />.
        /// </returns>
        /// <exception cref="NotSupportedException">
        /// The browser specified by <paramref name="options"/> is not supported.
        /// </exception>
        private static IWebDriver CreateBrowserStackDriver(WebDriverFactoryOptions options, TestContext context)
        {
            BrowserStackOptions browserStack = options.BrowserStackOptions;

            DesiredCapabilities desiredCapabilities;

            switch (options.Browser)
            {
                case WebBrowserType.Chrome:
                    desiredCapabilities = DesiredCapabilities.Chrome();
                    break;

                case WebBrowserType.Firefox:
                    desiredCapabilities = DesiredCapabilities.Firefox();
                    FirefoxProfile profile = CreateFirefoxProfile();
                    desiredCapabilities.SetCapability(FirefoxDriver.ProfileCapabilityName, profile.ToBase64String());
                    break;

                case WebBrowserType.InternetExplorer:
                    desiredCapabilities = DesiredCapabilities.InternetExplorer();
                    break;

                case WebBrowserType.Safari:
                    desiredCapabilities = DesiredCapabilities.Safari();
                    break;

                default:
                    throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, "The browser '{0}' is not supported.", options.Browser));
            }

            desiredCapabilities.SetCapability("browserstack.user", browserStack.UserName);
            desiredCapabilities.SetCapability("browserstack.key", browserStack.ApiKey);
            desiredCapabilities.SetCapability("browserstack.debug", browserStack.Debug);
            desiredCapabilities.SetCapability("acceptSslCerts", browserStack.AcceptSslCertificates);

            if (!string.IsNullOrEmpty(browserStack.Project))
            {
                desiredCapabilities.SetCapability("project", browserStack.Project);
            }

            if (!string.IsNullOrEmpty(browserStack.Build))
            {
                desiredCapabilities.SetCapability("build", browserStack.Build);
            }

            if (!string.IsNullOrEmpty(browserStack.BrowserVersion))
            {
                desiredCapabilities.SetCapability("browser_version", browserStack.BrowserVersion);
            }

            if (!string.IsNullOrEmpty(browserStack.OS))
            {
                desiredCapabilities.SetCapability("os", browserStack.OS);
            }

            if (!string.IsNullOrEmpty(browserStack.OSVersion))
            {
                desiredCapabilities.SetCapability("os_version", browserStack.OSVersion);
            }

            if (!string.IsNullOrEmpty(browserStack.Resolution))
            {
                desiredCapabilities.SetCapability("resolution", browserStack.Resolution);
            }

            if (context != null)
            {
                desiredCapabilities.SetCapability("name", context.TestName);
            }

            return new BrowserStackRemoteWebDriver(browserStack.RemoteUri, desiredCapabilities);
        }

        /// <summary>
        /// Creates an instance of <see cref="IWebDriver"/> for using a local browser.
        /// </summary>
        /// <param name="options">The options to use to create the instance.</param>
        /// <returns>
        /// The created instance of <see cref="IWebDriver" />.
        /// </returns>
        private static IWebDriver CreateLocalDriver(WebDriverFactoryOptions options)
        {
            switch (options.Browser)
            {
                case WebBrowserType.Chrome:
                    return CreateChromeDriver();

                case WebBrowserType.Firefox:
                    return CreateFirefoxDriver();

                case WebBrowserType.InternetExplorer:
                    return CreateIEDriver();

                case WebBrowserType.Safari:
                    return CreateSafariDriver();

                default:
                    throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, "The browser '{0}' is not supported.", options.Browser));
            }
        }

        /// <summary>
        /// Creates an <see cref="IWebDriver"/> for use with Google Chrome.
        /// </summary>
        /// <returns>
        /// The created instance of <see cref="IWebDriver"/>.
        /// </returns>
        private static IWebDriver CreateChromeDriver()
        {
            ChromeOptions options = CreateChromeOptions();
            return new ChromeDriver(options);
        }

        /// <summary>
        /// Creates a <see cref="ChromeOptions"/> for use with Google Chrome.
        /// </summary>
        /// <returns>
        /// The created instance of <see cref="ChromeOptions"/>.
        /// </returns>
        private static ChromeOptions CreateChromeOptions()
        {
            Dictionary<string, object> preferences = new Dictionary<string, object>()
            {
                 { "download.prompt_for_download", false },
                 { "download.default_directory", Environment.CurrentDirectory },
            };

            var options = new ChromeOptionsWithPreferences();
            options.prefs = preferences;

            // Ensure we always start Chrome using US English
            options.AddArgument("--lang=en-US");

            return options;
        }

        /// <summary>
        /// Creates an <see cref="IWebDriver"/> for use with Mozilla Firefox.
        /// </summary>
        /// <returns>
        /// The created instance of <see cref="IWebDriver"/>.
        /// </returns>
        private static IWebDriver CreateFirefoxDriver()
        {
            FirefoxProfile profile = CreateFirefoxProfile();
            return new FirefoxDriver(profile);
        }

        /// <summary>
        /// Creates a <see cref="FirefoxProfile"/> for use with Mozilla Firefox.
        /// </summary>
        /// <returns>
        /// The created instance of <see cref="FirefoxProfile"/>.
        /// </returns>
        private static FirefoxProfile CreateFirefoxProfile()
        {
            FirefoxProfile profile = new FirefoxProfile();

            ////profile.AcceptUntrustedCertificates = true;

            // Ensure we always have Firefox using US English
            profile.SetPreference("intl.accept_languages", "en-US");

            profile.SetPreference("browser.download.downloadDir", Environment.CurrentDirectory);
            profile.SetPreference("browser.download.dir", Environment.CurrentDirectory);

            profile.SetPreference("browser.download.folderList", 2);
            profile.SetPreference("browser.download.manager.addToRecentDocs", false);
            profile.SetPreference("browser.download.manager.alertOnEXEOpen", false);
            profile.SetPreference("browser.download.manager.closeWhenDone", true);
            profile.SetPreference("browser.download.manager.focusWhenStarting", false);
            profile.SetPreference("browser.download.manager.retention", 0);
            profile.SetPreference("browser.download.manager.scanWhenDone", false);
            profile.SetPreference("browser.download.manager.showAlertOnComplete", false);
            profile.SetPreference("browser.download.manager.useWindow", false);
            profile.SetPreference("browser.download.useDownloadDir", true);
            profile.SetPreference("browser.helperApps.alwaysAsk.force", false);
            profile.SetPreference("browser.helperApps.neverAsk.saveToDisk", "application/octet-stream");

            return profile;
        }

        /// <summary>
        /// Creates an <see cref="IWebDriver"/> for use with Microsoft Internet Explorer.
        /// </summary>
        /// <returns>
        /// The created instance of <see cref="IWebDriver"/>.
        /// </returns>
        private static IWebDriver CreateIEDriver()
        {
            var options = new InternetExplorerOptions()
            {
                IgnoreZoomLevel = true,

                // This is required to work around an issue with support for IE 11.
                // See the following links:
                // http://code.google.com/p/selenium/wiki/InternetExplorerDriver#Required_Configuration
                // http://code.google.com/p/selenium/issues/detail?id=6511
                IntroduceInstabilityByIgnoringProtectedModeSettings = true,
            };

            return new InternetExplorerDriver(options);
        }

        /// <summary>
        /// Creates an <see cref="IWebDriver"/> for use with Apple Safari.
        /// </summary>
        /// <returns>
        /// The created instance of <see cref="IWebDriver"/>.
        /// </returns>
        private static IWebDriver CreateSafariDriver()
        {
            var options = new SafariOptions();
            return new SafariDriver(options);
        }

        /// <summary>
        /// A class representing an instance of <see cref="ChromeOptions"/> that allows
        /// the <c>prefs</c> data to be set in the Chrome browser profile.
        /// </summary>
        private class ChromeOptionsWithPreferences : ChromeOptions
        {
            /// <summary>
            /// Gets or sets the user preferences.
            /// </summary>
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Required for correct JSON serialization.")]
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Used for serialization.")]
            public Dictionary<string, object> prefs
            {
                get;
                set;
            }
        }
    }
}