// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectsController.cs" company="http://www.martincostello.com">
//   Martin Costello (c) 2014
// </copyright>
// <summary>
//   ProjectsController.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Web.Mvc;

namespace MartinCostello.Controllers
{
    /// <summary>
    /// A class representing the controller for the <c>Projects</c> views.
    /// </summary>
    public class ProjectsController : Controller
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectsController"/> class.
        /// </summary>
        public ProjectsController()
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