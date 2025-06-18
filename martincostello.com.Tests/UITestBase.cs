// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UITestBase.cs" company="http://www.martincostello.com">
//   Martin Costello (c) 2014
// </copyright>
// <summary>
//   UITestBase.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Reflection;
using MartinCostello.PageTemplates;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace MartinCostello
{
    /// <summary>
    /// The base class for Selenium UI tests.
    /// </summary>
    [TestClass]
    public abstract class UITestBase : IDisposable
    {
        /// <summary>
        /// The name of the data file containing the browsers to test for.
        /// </summary>
        public const string BrowsersDataFileName = @"|DataDirectory|\Browsers.xml";

        /// <summary>
        /// The name of the property containing the browser type.
        /// </summary>
        public const string BrowserTypePropertyName = "BrowserType";

        /// <summary>
        /// The name of the property containing the browser version.
        /// </summary>
        public const string BrowserVersionPropertyName = "BrowserVersion";

        /// <summary>
        /// The name of the property containing the OS.
        /// </summary>
        public const string OSPropertyName = "OS";

        /// <summary>
        /// The name of the property containing the OS version.
        /// </summary>
        public const string OSVersionPropertyName = "OSVersion";

        /// <summary>
        /// The name of the browser table.
        /// </summary>
        public const string BrowserTypeTableName = "browser";

        /// <summary>
        /// The invariant name of the data provider to use.
        /// </summary>
        public const string ProviderName = "Microsoft.VisualStudio.TestTools.DataSource.XML";

        /// <summary>
        /// A <see cref="string"/> containing information about the build of this assembly.
        /// </summary>
        private static readonly string Build = GetAssemblyBuild();

        /// <summary>
        /// Whether the instance has been disposed.
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="UITestBase"/> class.
        /// </summary>
        protected UITestBase()
        {
            DriverFactory = new WebDriverFactory();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UITestBase"/> class.
        /// </summary>
        /// <param name="browserType">The type of browser the unit test is for.</param>
        protected UITestBase(WebBrowserType browserType)
        {
            BrowserType = browserType;
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="UITestBase"/> class.
        /// </summary>
        ~UITestBase()
        {
            Dispose(false);
        }

        /// <summary>
        /// Gets the <c>BrowserStack</c> API key to use.
        /// </summary>
        public static string BrowserStackApiKey
        {
            get { return ConfigurationManager.AppSettings["BrowserStack.ApiKey"]; }
        }

        /// <summary>
        /// Gets the base URI for the <c>BrowserStack</c> service to use.
        /// </summary>
        public static Uri BrowserStackUri
        {
            get { return new Uri(ConfigurationManager.AppSettings["BrowserStack.Uri"], UriKind.Absolute); }
        }

        /// <summary>
        /// Gets the <c>BrowserStack</c> user name to use.
        /// </summary>
        public static string BrowserStackUserName
        {
            get { return ConfigurationManager.AppSettings["BrowserStack.UserName"]; }
        }

        /// <summary>
        /// Gets a value indicating whether <c>BrowserStack</c> debugging is enabled.
        /// </summary>
        public static bool IsBrowserStackDebuggingEnabled
        {
            get { return string.Equals(ConfigurationManager.AppSettings["BrowserStack.Debug"], bool.TrueString, StringComparison.OrdinalIgnoreCase); }
        }

        /// <summary>
        /// Gets a value indicating whether <c>BrowserStack</c> is enabled.
        /// </summary>
        public static bool IsBrowserStackEnabled
        {
            get
            {
                string settingName = "BrowserStack.Enabled";
                string value = Environment.GetEnvironmentVariable(settingName);

                if (string.IsNullOrWhiteSpace(value))
                {
                    value = ConfigurationManager.AppSettings[settingName];
                }

                return string.Equals(value, bool.TrueString, StringComparison.OrdinalIgnoreCase);
            }
        }

        /// <summary>
        /// Gets a value indicating whether <c>BrowserStack</c> Local is enabled.
        /// </summary>
        public static bool IsBrowserStackLocalEnabled
        {
            get { return string.Equals(ConfigurationManager.AppSettings["BrowserStack.Local"], bool.TrueString, StringComparison.OrdinalIgnoreCase); }
        }

        /// <summary>
        /// Gets or sets the <see cref="IWebDriverFactory"/> to use.
        /// </summary>
        public IWebDriverFactory DriverFactory
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the base URI for the website to test.
        /// </summary>
        protected virtual Uri BaseUri
        {
            get { return new Uri(ConfigurationManager.AppSettings["SiteBaseUri"], UriKind.Absolute); }
        }

        /// <summary>
        /// Gets the <see cref="WebBrowserType"/> for the current instance.
        /// </summary>
        protected WebBrowserType BrowserType
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the <see cref=""/> in use by the instance.
        /// </summary>
        protected IWebDriver Driver
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the default web browser type to use.
        /// </summary>
        protected virtual WebBrowserType DefaultBrowser
        {
            get { return WebBrowserType.Firefox; }
        }

        /// <summary>
        /// Gets the default implicit wait to use.
        /// </summary>
        protected virtual TimeSpan DefaultImplicitWait
        {
            get { return TimeSpan.FromSeconds(10); }
        }

        /// <summary>
        /// Gets the default timeout to use.
        /// </summary>
        protected virtual TimeSpan DefaultTimeout
        {
            get
            {
                return IsUsingBrowserStack ?
                    TimeSpan.FromSeconds(30) :
                    TimeSpan.FromSeconds(10);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the tests are running via <c>BrowserStack</c>.
        /// </summary>
        protected virtual bool IsUsingBrowserStack
        {
            get { return IsBrowserStackEnabled; }
        }

        /// <summary>
        /// Gets a value indicating whether the tests are running via <c>BrowserStack</c> Local.
        /// </summary>
        protected virtual bool IsUsingBrowserStackLocal
        {
            get { return IsBrowserStackEnabled && IsBrowserStackLocalEnabled; }
        }

        /// <summary>
        /// Initializes the test assembly.
        /// </summary>
        /// <param name="context">The test context.</param>
        [AssemblyInitialize]
        public static void AssemblyInitialize(TestContext context)
        {
            if (IsBrowserStackEnabled)
            {
                EnsureBrowserStackAutomateSessionAvailable();
            }

            // Ensure the website has been started for local testing
            if (!IsBrowserStackEnabled || IsBrowserStackLocalEnabled)
            {
                // TODO Start the website using IIS Express
                // TODO Start up BrowserStackLocal.exe, if required
            }

            // Ensure the site is warm before testing
            using (WebClient client = new WebClient())
            {
                client.DownloadString(ConfigurationManager.AppSettings["SiteBaseUri"]);
            }
        }

        /// <summary>
        /// Cleans up the test assembly.
        /// </summary>
        [AssemblyCleanup]
        public static void AssemblyCleanup()
        {
            // TODO Shutdown the IIS Express website
            // TODO Shutdown BrowserStackLocal.exe, if running
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Initializes the test.
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
            OnTestInitialize();
        }

        /// <summary>
        /// Cleans up the test.
        /// </summary>
        [TestCleanup]
        public void TestCleanup()
        {
            OnTestCleanup();
        }

        /// <summary>
        /// Creates an instance of <see cref="WebDriverWait"/>.
        /// </summary>
        /// <returns>
        /// The created instance of <see cref="WebDriverWait"/>.
        /// </returns>
        protected virtual WebDriverWait CreateWait()
        {
            WebDriverWait wait = new WebDriverWait(Driver, DefaultTimeout);

            wait.IgnoreExceptionTypes(typeof(StaleElementReferenceException));

            return wait;
        }

        /// <summary>
        /// Creates a new instance of <see cref="IWebDriver"/>.
        /// </summary>
        /// <param name="options">The options to use to create the instance.</param>
        /// <param name="context">The <see cref="TestContext"/> associated with the current test.</param>
        /// <returns>
        /// The created instance of <see cref="IWebDriver"/>.
        /// </returns>
        protected virtual IWebDriver CreateWebDriver(WebDriverFactoryOptions options, TestContext context)
        {
            IWebDriver driver = DriverFactory.Create(options, context);

            try
            {
                InitializeWebDriver(driver);
                LogDriverProperties(driver);

                return driver;
            }
            catch (Exception)
            {
                driver?.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Releases unmanaged and, optionally, managed resources.
        /// </summary>
        /// <param name="disposing">
        /// <see langword="true"/> to release both managed and unmanaged resources;
        /// <see langword="false"/> to release only unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing && Driver != null)
                {
                    Driver.Dispose();
                    Driver = null;
                }

                _disposed = true;
            }
        }

        /// <summary>
        /// Initializes the specified instance of <see cref="IWebDriver"/>.
        /// </summary>
        /// <param name="driver">The <see cref="IWebDriver"/> to initialize.</param>
        protected virtual void InitializeWebDriver(IWebDriver driver)
        {
            if (driver == null)
            {
                throw new ArgumentNullException(nameof(driver));
            }

            IOptions options = driver.Manage();
            options.Timeouts().ImplicitlyWait(DefaultImplicitWait);
            options.Window.Maximize();
        }

        /// <summary>
        /// Loads a new web page in the current browser window.
        /// </summary>
        /// <typeparam name="T">The type of the page being navigated to.</typeparam>
        /// <param name="url">The URL of the page to load.</param>
        /// <returns>
        /// The page navigated to.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Usage",
            "CA2234:PassSystemUriObjectsInsteadOfStrings",
            Justification = "Fits the Selenium API.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Design",
            "CA1054:UriParametersShouldNotBeStrings",
            MessageId = "0#",
            Justification = "Fits the Selenium API.")]
        protected virtual T GoToUrl<T>(string url)
            where T : PageBase
        {
            Uri uri = new Uri(url, UriKind.RelativeOrAbsolute);

            if (!uri.IsAbsoluteUri)
            {
                uri = new Uri(BaseUri, uri);
            }

            GoToUrl(uri);
            return Activator.CreateInstance(typeof(T), Driver) as T;
        }

        /// <summary>
        /// Loads a new web page in the current browser window.
        /// </summary>
        /// <param name="url">The URL to load.</param>
        protected virtual void GoToUrl(Uri url)
        {
            Driver.Navigate().GoToUrl(url);
        }

        /// <summary>
        /// Initializes the test.
        /// </summary>
        protected virtual void OnTestInitialize()
        {
            if (BrowserType == WebBrowserType.None)
            {
                string browserTypeString = null;

                // First see if there's an appropriate data row associated with the current test
                if (TestContext != null)
                {
                    if (TestContext.DataRow != null &&
                        TestContext.DataRow.Table.Columns.Contains(BrowserTypePropertyName))
                    {
                        browserTypeString = TestContext.DataRow[BrowserTypePropertyName] as string;
                    }
                    else
                    {
                        // Otherwise, see if there's a test property overriding the browser to use
                        browserTypeString = TestContext.Properties[BrowserTypePropertyName] as string;
                    }
                }

                WebBrowserType browserType;

                if (string.IsNullOrEmpty(browserTypeString))
                {
                    // We couldn't determine a browser, so use the default
                    browserType = DefaultBrowser;
                }
                else
                {
                    if (!Enum.TryParse<WebBrowserType>(browserTypeString, true, out browserType))
                    {
                        Assert.Fail("'{0}' is not a supported browser type.", browserTypeString);
                    }
                }

                BrowserType = browserType;
            }

            WebDriverFactoryOptions options = new WebDriverFactoryOptions()
            {
                Browser = BrowserType,
            };

            if (IsUsingBrowserStack)
            {
                options.BrowserStackOptions = new BrowserStackOptions()
                {
                    AcceptSslCertificates = IsUsingBrowserStackLocal,
                    ApiKey = BrowserStackApiKey,
                    Build = Build,
                    Debug = IsBrowserStackDebuggingEnabled,
                    Project = "martincostello.com",
                    RemoteUri = BrowserStackUri,
                    UserName = BrowserStackUserName,
                };

                if (TestContext != null && TestContext.DataRow != null)
                {
                    const string BrowserVersionRowName = "BrowserVersion";
                    const string OSRowName = "OS";
                    const string OSVersionRowName = "OSVersion";
                    const string ResolutionRowName = "Resolution";

                    if (TestContext.DataRow.Table.Columns.Contains(BrowserVersionRowName))
                    {
                        options.BrowserStackOptions.BrowserVersion = Convert.ToString(TestContext.DataRow[BrowserVersionRowName], CultureInfo.InvariantCulture);
                    }

                    if (TestContext.DataRow.Table.Columns.Contains(OSRowName))
                    {
                        options.BrowserStackOptions.OS = Convert.ToString(TestContext.DataRow[OSRowName], CultureInfo.InvariantCulture);
                    }

                    if (TestContext.DataRow.Table.Columns.Contains(OSVersionRowName))
                    {
                        options.BrowserStackOptions.OSVersion = Convert.ToString(TestContext.DataRow[OSVersionRowName], CultureInfo.InvariantCulture);
                    }

                    if (TestContext.DataRow.Table.Columns.Contains(ResolutionRowName))
                    {
                        options.BrowserStackOptions.Resolution = Convert.ToString(TestContext.DataRow[ResolutionRowName], CultureInfo.InvariantCulture);
                    }
                }
                else
                {
                    options.BrowserStackOptions.BrowserVersion = TestContext.Properties[BrowserVersionPropertyName] as string;
                    options.BrowserStackOptions.OS = TestContext.Properties[OSPropertyName] as string;
                    options.BrowserStackOptions.OSVersion = TestContext.Properties[OSVersionPropertyName] as string;
                }

                if (string.IsNullOrEmpty(options.BrowserStackOptions.OS))
                {
                    options.BrowserStackOptions.OS = "Windows";
                }

                if (string.IsNullOrEmpty(options.BrowserStackOptions.OSVersion))
                {
                    options.BrowserStackOptions.OSVersion = "8.1";
                }

                if (string.IsNullOrEmpty(options.BrowserStackOptions.Resolution))
                {
                    options.BrowserStackOptions.Resolution = "1920x1080";
                }
            }

            Driver = CreateWebDriver(options, TestContext);
        }

        /// <summary>
        /// Cleans up the test.
        /// </summary>
        protected virtual void OnTestCleanup()
        {
            if (Driver != null)
            {
                if (TestContext.CurrentTestOutcome != UnitTestOutcome.Passed)
                {
                    Trace.WriteLine(string.Format(CultureInfo.InvariantCulture, "Title: '{0}'; URL: {1}", Driver.Title, Driver.Url));
                }

                Driver.Quit();
            }
        }

        /// <summary>
        /// Logs the properties of the specified<see cref="IWebDriver"/>.
        /// </summary>
        /// <param name="driver">The <see cref="IWebDriver"/> to log the properties for.</param>
        protected virtual void LogDriverProperties(IWebDriver driver)
        {
            if (driver == null)
            {
                return;
            }

            Trace.WriteLine(string.Format(CultureInfo.InvariantCulture, "Web Driver Type: {0}", driver.GetType().FullName));

            if (driver is IHasCapabilities hasCapabilities)
            {
                ICapabilities capabilities = hasCapabilities.Capabilities;
                Trace.WriteLine(string.Format(CultureInfo.InvariantCulture, "Browser Name: {0}", capabilities.BrowserName));
                Trace.WriteLine(string.Format(CultureInfo.InvariantCulture, "Browser Version: {0}", capabilities.Version));

                if (capabilities.Platform != null)
                {
                    Trace.WriteLine(string.Format(CultureInfo.InvariantCulture, "Browser Platform Type: {0}", capabilities.Platform.PlatformType));
                    Trace.WriteLine(string.Format(CultureInfo.InvariantCulture, "Browser Platform Major Version: {0}", capabilities.Platform.MajorVersion));
                    Trace.WriteLine(string.Format(CultureInfo.InvariantCulture, "Browser Platform Minor Version: {0}", capabilities.Platform.MinorVersion));
                }

                Trace.WriteLine(string.Format(CultureInfo.InvariantCulture, "Is JavaScript Enabled?: {0}", capabilities.IsJavaScriptEnabled));
            }
        }

        /// <summary>
        /// Waits for the page with the specified title to load.
        /// </summary>
        /// <param name="title">The title of the page to wait to load.</param>
        protected virtual void WaitForPageToLoad(string title)
        {
            WaitUntil((p) => string.Equals(p.Title, title, StringComparison.Ordinal));
        }

        /// <summary>
        /// Waits for the specified delegate to return <see langword="true"/>.
        /// </summary>
        /// <param name="condition">A delegate representing the condition to wait to be true.</param>
        protected virtual void WaitUntil(Func<bool> condition)
        {
            var wait = CreateWait();
            wait.Until((p) => condition());
        }

        /// <summary>
        /// Waits for the specified delegate to return <see langword="true"/>.
        /// </summary>
        /// <param name="condition">A delegate representing the condition to wait to be true which is passed an instance of <see cref="IWebDriver"/>.</param>
        protected virtual void WaitUntil(Func<IWebDriver, bool> condition)
        {
            var wait = CreateWait();
            wait.Until(condition);
        }

        /// <summary>
        /// Returns the build of this assembly.
        /// </summary>
        /// <returns>
        /// A string containing the build of this assembly.
        /// </returns>
        private static string GetAssemblyBuild()
        {
            var type = typeof(MvcApplication);

            string version = type.Assembly
                .GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute), false)
                .OfType<AssemblyInformationalVersionAttribute>()
                .Select((p) => p.InformationalVersion)
                .First();

            string configuration = type.Assembly
                .GetCustomAttributes(typeof(AssemblyMetadataAttribute), false)
                .OfType<AssemblyMetadataAttribute>()
                .Where((p) => string.Equals("BuildLabel", p.Key, StringComparison.OrdinalIgnoreCase))
                .Select((p) => p.Value)
                .FirstOrDefault();

            if (string.IsNullOrWhiteSpace(configuration))
            {
                configuration = type.Assembly
                    .GetCustomAttributes(typeof(AssemblyConfigurationAttribute), false)
                    .OfType<AssemblyConfigurationAttribute>()
                    .Select((p) => p.Configuration)
                    .FirstOrDefault();
            }

            return string.Format(
                CultureInfo.InvariantCulture,
                "{0} ({1})",
                version,
                string.IsNullOrEmpty(configuration) || string.Equals("LOCAL", configuration, StringComparison.OrdinalIgnoreCase) ? Environment.MachineName + "_" + DateTime.Now.ToString("yyyyMMdd_HHmm", CultureInfo.InvariantCulture) : configuration);
        }

        /// <summary>
        /// Ensures there is a <c>BrowserStack</c> Automate session available.
        /// </summary>
        private static void EnsureBrowserStackAutomateSessionAvailable()
        {
            var client = new BrowserStack.Automate.BrowserStackAutomateClient(
                BrowserStackUserName,
                BrowserStackApiKey);

            var status = client.GetStatusAsync().Result;

            if (status.MaximumAllowedParallelSessions < 1 ||
                status.MaximumAllowedParallelSessions == status.ParallelSessionsRunning)
            {
                Assert.Inconclusive(
                    "All {0:N0} BrowserStack Automate sessions available to {1} are in use.",
                    status.MaximumAllowedParallelSessions,
                    client.UserName);
            }
        }
    }
}
