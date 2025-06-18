// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PageBase.cs" company="http://www.martincostello.com">
//   Martin Costello (c) 2014
// </copyright>
// <summary>
//   PageBase.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;

namespace MartinCostello.PageTemplates
{
    /// <summary>
    /// A class representing the base class for page templates.
    /// </summary>
    public abstract class PageBase
    {
        /// <summary>
        /// The link to the the <c>/About</c> page.
        /// </summary>
        [FindsBy(How = How.Id, Using = "link-about")]
        private IWebElement _aboutLink = null;

        /// <summary>
        /// The link to the blog.
        /// </summary>
        [FindsBy(How = How.Id, Using = "link-blog")]
        private IWebElement _blogLink = null;

        /// <summary>
        /// The link to the <c>/Projects</c> page.
        /// </summary>
        [FindsBy(How = How.Id, Using = "link-projects")]
        private IWebElement _projectsLink = null;

        /// <summary>
        /// The link to the root of the website.
        /// </summary>
        [FindsBy(How = How.Id, Using = "link-home")]
        private IWebElement _homeLink = null;

        /// <summary>
        /// The link to the <c>/Tools</c> page.
        /// </summary>
        [FindsBy(How = How.Id, Using = "link-tools")]
        private IWebElement _toolsLink = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="PageBase"/> class.
        /// </summary>
        /// <param name="driver">The driver.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="driver"/> is <see langword="null"/>.
        /// </exception>
        protected PageBase(IWebDriver driver)
        {
            if (driver == null)
            {
                throw new ArgumentNullException("driver");
            }

            PageFactory.InitElements(driver, this);

            this.Driver = driver;
        }

        /// <summary>
        /// Gets the <see cref="IWebDriver"/> associated with the page.
        /// </summary>
        public IWebDriver Driver
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the page's current title.
        /// </summary>
        public string Title
        {
            get { return this.Driver.Title; }
        }

        /// <summary>
        /// Gets the page's current URL.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Design",
            "CA1056:UriPropertiesShouldNotBeStrings",
            Justification = "Fits the Selenium API design.")]
        public string Url
        {
            get { return this.Driver.Url; }
        }

        /// <summary>
        /// Returns a new page of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of the new page.</typeparam>
        /// <returns>
        /// The page as the type specified by <typeparamref name="T"/>.
        /// </returns>
        public T As<T>()
            where T : PageBase
        {
            if (this.GetType() == typeof(T))
            {
                return this as T;
            }
            else
            {
                return Activator.CreateInstance(typeof(T), this.Driver) as T;
            }
        }

        /// <summary>
        /// Navigates to the about page.
        /// </summary>
        /// <returns>
        /// The page navigated to.
        /// </returns>
        public PageBase About()
        {
            _aboutLink.Click();
            return As<AboutPage>();
        }

        /// <summary>
        /// Navigates to the blog page.
        /// </summary>
        public void Blog()
        {
            _blogLink.Click();
        }

        /// <summary>
        /// Navigates to the home page.
        /// </summary>
        /// <returns>
        /// The page navigated to.
        /// </returns>
        public PageBase Home()
        {
            _homeLink.Click();
            return As<HomePage>();
        }

        /// <summary>
        /// Navigates to the projects page.
        /// </summary>
        /// <returns>
        /// The page navigated to.
        /// </returns>
        public PageBase Projects()
        {
            _projectsLink.Click();
            return As<ProjectsPage>();
        }

        /// <summary>
        /// Navigates to the tools page.
        /// </summary>
        /// <returns>
        /// The page navigated to.
        /// </returns>
        public PageBase Tools()
        {
            _toolsLink.Click();
            return As<ToolsPage>();
        }
    }
}