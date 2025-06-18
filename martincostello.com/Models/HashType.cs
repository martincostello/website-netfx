// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HashType.cs" company="http://www.martincostello.com">
//   Martin Costello (c) 2014
// </copyright>
// <summary>
//   HashType.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace MartinCostello.Models
{
    /// <summary>
    /// A class representing the type of a hash.
    /// </summary>
    public class HashType
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HashType"/> class.
        /// </summary>
        public HashType()
        {
        }

        /// <summary>
        /// Gets or sets the text for the hash type.
        /// </summary>
        public string Text
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the value of the hash type.
        /// </summary>
        public string Value
        {
            get;
            set;
        }
    }
}