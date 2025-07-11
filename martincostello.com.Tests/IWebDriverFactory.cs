﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IWebDriverFactory.cs" company="http://www.martincostello.com">
//   Martin Costello (c) 2014
// </copyright>
// <summary>
//   IWebDriverFactory.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;

namespace MartinCostello
{
    /// <summary>
    /// Defines a method for creating instances of <see cref="IWebDriver"/>.
    /// </summary>
    public interface IWebDriverFactory
    {
        /// <summary>
        /// Creates a new instance of <see cref="IWebDriver"/>.
        /// </summary>
        /// <param name="options">The options to use to create the instance.</param>
        /// <param name="context">The <see cref="TestContext"/> associated with the current test.</param>
        /// <returns>
        /// The created instance of <see cref="IWebDriver"/>.
        /// </returns>
        IWebDriver Create(WebDriverFactoryOptions options, TestContext context);
    }
}