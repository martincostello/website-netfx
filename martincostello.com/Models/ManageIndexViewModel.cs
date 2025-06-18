// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ManageIndexViewModel.cs" company="http://www.martincostello.com">
//   Martin Costello (c) 2015
// </copyright>
// <summary>
//   ManageIndexViewModel.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.AspNet.Identity;

namespace MartinCostello.Models
{
    /// <summary>
    /// A class representing the view model for the <c>/manage/index</c> view. This class cannot be inherited.
    /// </summary>
    public sealed class ManageIndexViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ManageIndexViewModel"/> class.
        /// </summary>
        public ManageIndexViewModel()
        {
            this.Logins = new List<UserLoginInfo>();
        }

        /// <summary>
        /// Gets an <see cref="IList{T}"/> containing the user logins.
        /// </summary>
        public IList<UserLoginInfo> Logins
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets a value indicating whether the log in is remembered by the browser.
        /// </summary>
        public bool BrowserRemembered
        {
            get;
            internal set;
        }
    }
}