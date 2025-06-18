// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FilterConfig.cs" company="http://www.martincostello.com">
//   Martin Costello (c) 2014-2015
// </copyright>
// <summary>
//   FilterConfig.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace MartinCostello
{
    using System.Web.Mvc;
    using MartinCostello.Filters;

    /// <summary>
    /// A class representing the filter configuration.
    /// </summary>
    internal static class FilterConfig
    {
        /// <summary>
        /// Registers the global filters.
        /// </summary>
        /// <param name="filters">The filter collection to register filters for.</param>
        internal static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new CustomHandleErrorAttribute());

#if !DEBUG
            filters.Add(new RequireHttpsAttribute());
#endif
        }
    }
}
