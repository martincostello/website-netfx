// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NotificationsController.cs" company="http://www.martincostello.com">
//   Martin Costello (c) 2014
// </copyright>
// <summary>
//   NotificationsController.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Web.Mvc;

namespace MartinCostello.Controllers
{
    /// <summary>
    /// A class representing the controller for the <c>Notifications</c> views.
    /// </summary>
    public class NotificationsController : Controller
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationsController"/> class.
        /// </summary>
        public NotificationsController()
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
