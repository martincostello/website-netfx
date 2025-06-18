// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AboutPage.cs" company="http://www.martincostello.com">
//   Martin Costello (c) 2014
// </copyright>
// <summary>
//   AboutPage.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using OpenQA.Selenium;

namespace MartinCostello.PageTemplates
{
    /// <summary>
    /// A class representing the page template for the <c>/About</c> page.
    /// </summary>
    public sealed class AboutPage : PageBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AboutPage"/> class.
        /// </summary>
        /// <param name="driver">The driver.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="driver"/> is <see langword="null"/>.
        /// </exception>
        public AboutPage(IWebDriver driver)
            : base(driver)
        {
        }
    }
}