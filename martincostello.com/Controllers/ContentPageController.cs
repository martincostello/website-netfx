// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContentPageController.cs" company="http://www.martincostello.com">
//   Martin Costello (c) 2014
// </copyright>
// <summary>
//   ErrorController.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Web.Mvc;

namespace MartinCostello.Controllers
{
    /// <summary>
    /// A class representing the controller for the <c>ContentPage</c> views.
    /// </summary>
    public class ContentPageController : Controller
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ContentPageController"/> class.
        /// </summary>
        public ContentPageController()
            : base()
        {
        }

        /// <summary>
        /// Returns the <c>Blog</c> view.
        /// </summary>
        /// <returns>
        /// The action result of the view.
        /// </returns>
        public ActionResult Blog()
        {
            return RedirectToActionPermanent("Blog", "Home");
        }

        /// <summary>
        /// Returns the <c>Index</c> view.
        /// </summary>
        /// <returns>
        /// The action result of the view.
        /// </returns>
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// Returns the <c>Blog</c> view.
        /// </summary>
        /// <returns>
        /// The action result of the view.
        /// </returns>
        public ActionResult Twitter()
        {
            return RedirectPermanent("https://twitter.com/martin_costello");
        }
    }
}