// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HashFormat.cs" company="http://www.martincostello.com">
//   Martin Costello (c) 2014
// </copyright>
// <summary>
//   HashFormat.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.ComponentModel.DataAnnotations;

namespace MartinCostello.Models
{
    /// <summary>
    /// An enumeration of hash formats.
    /// </summary>
    public enum HashFormat
    {
        /// <summary>
        /// Output the hash in hexadecimal format.
        /// </summary>
        [Display(Name = "Hexadecimal")]
        Hexadecimal = 0,

        /// <summary>
        /// Output the hash in base-64 format.
        /// </summary>
        [Display(Name = "Base64")]
        Base64,
    }
}