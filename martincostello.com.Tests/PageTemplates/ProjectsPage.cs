// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectsPage.cs" company="http://www.martincostello.com">
//   Martin Costello (c) 2014
// </copyright>
// <summary>
//   ProjectsPage.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using OpenQA.Selenium;

namespace MartinCostello.PageTemplates
{
    /// <summary>
    /// A class representing the page template for the <c>/Projects</c> page.
    /// </summary>
    public sealed class ProjectsPage : PageBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectsPage"/> class.
        /// </summary>
        /// <param name="driver">The driver.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="driver"/> is <see langword="null"/>.
        /// </exception>
        public ProjectsPage(IWebDriver driver)
            : base(driver)
        {
        }
    }
}