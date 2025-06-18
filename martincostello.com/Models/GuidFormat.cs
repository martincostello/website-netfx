// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GuidFormat.cs" company="http://www.martincostello.com">
//   Martin Costello (c) 2014
// </copyright>
// <summary>
//   GuidFormat.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace MartinCostello.Models
{
    /// <summary>
    /// A class representing a GUID format.
    /// </summary>
    public class GuidFormat
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GuidFormat"/> class.
        /// </summary>
        public GuidFormat()
        {
        }

        /// <summary>
        /// Gets or sets the text of the GUID format.
        /// </summary>
        public string Text
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the value of the GUID format.
        /// </summary>
        public string Value
        {
            get;
            set;
        }
    }
}