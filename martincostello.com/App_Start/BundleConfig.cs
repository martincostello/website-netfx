// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BundleConfig.cs" company="http://www.martincostello.com">
//   Martin Costello (c) 2014-2015
// </copyright>
// <summary>
//   BundleConfig.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Web.Optimization;

namespace MartinCostello
{
    /// <summary>
    /// A class representing the bundle configuration.  This class cannot be inherited.
    /// </summary>
    internal static class BundleConfig
    {
        /// <summary>
        /// Registers the bundles.
        /// </summary>
        /// <param name="bundles">The collection to register bundles in.</param>
        internal static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include("~/Scripts/jquery-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include("~/Scripts/modernizr-*"));
            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include("~/Scripts/bootstrap.js", "~/Scripts/respond.js"));
            bundles.Add(new ScriptBundle("~/bundles/zeroclipboard").Include("~/Scripts/zeroclipboard*"));
            bundles.Add(new StyleBundle("~/Content/css").Include("~/Content/bootstrap-theme.css", "~/Content/site.css", "~/Content/font-awesome.css", "~/Content/bootstrap-social.css"));
            bundles.IgnoreList.Ignore("*.swf", OptimizationMode.Always);
        }
    }
}