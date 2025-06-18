// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HomeControllerTests.cs" company="http://www.martincostello.com">
//   Martin Costello (c) 2014
// </copyright>
// <summary>
//   HomeControllerTests.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MartinCostello.Controllers
{
    /// <summary>
    /// A class containing tests for the <see cref="HomeController"/> class.
    /// </summary>
    [TestClass]
    public class HomeControllerTests
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HomeControllerTests"/> class.
        /// </summary>
        public HomeControllerTests()
        {
        }

        [TestMethod]
        [Description("Tests Index() returns the correct view.")]
        public void HomeController_Index_Returns_Correct_Result()
        {
            // Arrange
            using (HomeController target = new HomeController())
            {
                // Act
                ActionResult result = target.Index();

                // Assert
                Assert.IsNotNull(result, "Index() returned null.");
                Assert.IsInstanceOfType(result, typeof(ViewResult), "Index() returned incorrect result.");
            }
        }
    }
}
