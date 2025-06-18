// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ManageMessageId.cs" company="http://www.martincostello.com">
//   Martin Costello (c) 2015
// </copyright>
// <summary>
//   ManageMessageId.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace MartinCostello.Models
{
    /// <summary>
    /// An enumeration of message Ids for the <c>/manage</c> views.
    /// </summary>
    public enum ManageMessageId
    {
        /// <summary>
        /// A <c>login</c> was removed successfully.
        /// </summary>
        RemoveLogOnSuccess = 0,

        /// <summary>
        /// An error occurred.
        /// </summary>
        Error,
    }
}