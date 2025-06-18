// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GenerateGuidModel.cs" company="http://www.martincostello.com">
//   Martin Costello (c) 2014
// </copyright>
// <summary>
//   GenerateGuidModel.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace MartinCostello.Models
{
    /// <summary>
    /// A class representing the view model for generating a GUID.
    /// </summary>
    public class GenerateGuidModel
    {
        /// <summary>
        /// The default GUID formats.
        /// </summary>
        private static readonly GuidFormat[] DefaultFormats = new GuidFormat[]
        {
            new GuidFormat() { Text = "Default", Value = "D" },
            new GuidFormat() { Text = "Hexadecimal with braces", Value = "X" },
            new GuidFormat() { Text = "Numeric", Value = "N" },
            new GuidFormat() { Text = "With braces", Value = "B" },
            new GuidFormat() { Text = "With parentheses", Value = "P" },
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="GenerateGuidModel"/> class.
        /// </summary>
        public GenerateGuidModel()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GenerateGuidModel"/> class.
        /// </summary>
        /// <param name="format">The GUID format.</param>
        public GenerateGuidModel(string format)
        {
            Format = format;
        }

        /// <summary>
        /// Gets or sets the GUID format.
        /// </summary>
        [Display(Name = "Format")]
        public string Format
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the default GUID formats.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "Required for use in the views.")]
        public IEnumerable<GuidFormat> Formats
        {
            get { return DefaultFormats.ToArray(); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to generate a GUID with upper case characters.
        /// </summary>
        [Display(Name = "Upper case?")]
        public bool Uppercase
        {
            get;
            set;
        }
    }
}
