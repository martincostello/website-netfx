// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ManageLoginsViewModel.cs" company="http://www.martincostello.com">
//   Martin Costello (c) 2015
// </copyright>
// <summary>
//   ManageLoginsViewModel.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;

namespace MartinCostello.Models
{
    /// <summary>
    /// A class representing the view model for the <c>/manage/managelogins</c> view. This class cannot be inherited.
    /// </summary>
    public sealed class ManageLoginsViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ManageLoginsViewModel"/> class.
        /// </summary>
        public ManageLoginsViewModel()
        {
            this.CurrentLogins = new List<UserLoginInfo>();
            this.OtherLogins = new List<AuthenticationDescription>();
        }

        /// <summary>
        /// Gets an <see cref="IList{T}"/> containing the current logins.
        /// </summary>
        public IList<UserLoginInfo> CurrentLogins
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets an <see cref="IList{T}"/> containing the other logins.
        /// </summary>
        public IList<AuthenticationDescription> OtherLogins
        {
            get;
            internal set;
        }
    }
}