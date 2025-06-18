// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GenerateHashModel.cs" company="http://www.martincostello.com">
//   Martin Costello (c) 2014
// </copyright>
// <summary>
//   GenerateHashModel.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace MartinCostello.Models
{
    /// <summary>
    /// A class representing the view model for generating a hash.
    /// </summary>
    public class GenerateHashModel
    {
        /// <summary>
        /// The default hash format types.
        /// </summary>
        private static readonly HashType[] DefaultFormats = new HashType[]
        {
            new HashType() { Text = "MD5", Value = "MD5" },
            new HashType() { Text = "SHA-1", Value = "SHA1" },
            new HashType() { Text = "SHA-256", Value = "SHA256" },
            new HashType() { Text = "SHA-384", Value = "SHA384" },
            new HashType() { Text = "SHA-512", Value = "SHA512" },
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="GenerateHashModel"/> class.
        /// </summary>
        public GenerateHashModel()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GenerateHashModel"/> class.
        /// </summary>
        /// <param name="hashName">The name of the hash.</param>
        public GenerateHashModel(string hashName)
        {
            this.HashName = hashName;
        }

        /// <summary>
        /// Gets or sets the hash format.
        /// </summary>
        [Display(Name = "Format")]
        public HashFormat Format { get; set; }

        /// <summary>
        /// Gets or sets the hash name.
        /// </summary>
        [Display(Name = "Algorithm")]
        public string HashName { get; set; }

        /// <summary>
        /// Gets the available hash types.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "Required for use in the views.")]
        public IEnumerable<HashType> HashTypes
        {
            get { return DefaultFormats.ToArray(); }
        }

        /// <summary>
        /// Gets or sets the plaintext to hash.
        /// </summary>
        [Display(Name = "Plaintext")]
        public string Plaintext { get; set; }
    }
}