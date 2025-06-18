// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HomeController.cs" company="http://www.martincostello.com">
//   Martin Costello (c) 2014
// </copyright>
// <summary>
//   HomeController.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Web.Mvc;

namespace MartinCostello.Controllers
{
    /// <summary>
    /// A class representing the controller for the <c>Home</c> views.
    /// </summary>
    public class HomeController : Controller
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HomeController"/> class.
        /// </summary>
        public HomeController()
            : base()
        {
        }

        /// <summary>
        /// Returns the <c>About</c> view.
        /// </summary>
        /// <returns>
        /// The action result of the view.
        /// </returns>
        public ActionResult About()
        {
            return View();
        }

        /// <summary>
        /// Returns the <c>Blog</c> view.
        /// </summary>
        /// <returns>
        /// The action result of the view.
        /// </returns>
        public ActionResult Blog()
        {
            return RedirectPermanent("https://blog.martincostello.com");
        }

        /// <summary>
        /// Returns the <c>Index</c> view.
        /// </summary>
        /// <returns>
        /// The action result of the view.
        /// </returns>
        public ActionResult Index()
        {
            return View();
        }
    }
}