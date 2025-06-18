// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WebDriverFactoryOptions.cs" company="http://www.martincostello.com">
//   Martin Costello (c) 2014
// </copyright>
// <summary>
//   WebDriverFactoryOptions.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace MartinCostello
{
    /// <summary>
    /// A class containing options to use to create instances of <see cref="IWebDriver"/>
    /// using the <see cref="IWebDriverFactory.Create"/> method.
    /// </summary>
    public class WebDriverFactoryOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WebDriverFactoryOptions"/> class.
        /// </summary>
        public WebDriverFactoryOptions()
        {
        }

        /// <summary>
        /// Gets or sets the browser to create the driver for.
        /// </summary>
        public WebBrowserType Browser
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the <c>BrowserStack</c> options to use, if any.
        /// </summary>
        public BrowserStackOptions BrowserStackOptions
        {
            get;
            set;
        }
    }
}