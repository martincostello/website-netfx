// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MapsController.cs" company="http://www.martincostello.com">
//   Martin Costello (c) 2014
// </copyright>
// <summary>
//   SiteMapController.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Web.Mvc;

namespace MartinCostello.Controllers
{
    /// <summary>
    /// A class representing the controller for the <c>Maps</c> views.
    /// </summary>
    public class MapsController : Controller
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MapsController"/> class.
        /// </summary>
        public MapsController()
            : base()
        {
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