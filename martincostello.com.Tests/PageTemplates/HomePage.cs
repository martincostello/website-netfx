// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HomePage.cs" company="http://www.martincostello.com">
//   Martin Costello (c) 2014
// </copyright>
// <summary>
//   HomePage.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using OpenQA.Selenium;

namespace MartinCostello.PageTemplates
{
    /// <summary>
    /// A class representing the page template for the <c>/</c> page.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "HomePage", Justification = "Name as it's the Page for 'Home'.")]
    public sealed class HomePage : PageBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HomePage"/> class.
        /// </summary>
        /// <param name="driver">The driver.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="driver"/> is <see langword="null"/>.
        /// </exception>
        public HomePage(IWebDriver driver)
            : base(driver)
        {
        }
    }
}