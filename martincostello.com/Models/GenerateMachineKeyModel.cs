// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GenerateMachineKeyModel.cs" company="http://www.martincostello.com">
//   Martin Costello (c) 2014
// </copyright>
// <summary>
//   GenerateMachineKeyModel.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace MartinCostello.Models
{
    /// <summary>
    /// A class representing the view model for generating a machine key.
    /// </summary>
    public class GenerateMachineKeyModel
    {
        /// <summary>
        /// The default decryption hash algorithm types.
        /// </summary>
        private static readonly HashType[] DefaultDecryptionAlgorithms = new HashType[]
        {
            new HashType() { Text = "3DES", Value = "3DES" },
            new HashType() { Text = "AES (128 bits)", Value = "AES-128" },
            new HashType() { Text = "AES (192 bits)", Value = "AES-192" },
            new HashType() { Text = "AES (256 bits)", Value = "AES-256" },
            new HashType() { Text = "DES", Value = "DES" },
        };

        /// <summary>
        /// The default decryption hash algorithm types.
        /// </summary>
        private static readonly HashType[] DefaultValidationAlgorithms = new HashType[]
        {
            new HashType() { Text = "3DES", Value = "3DES" },
            new HashType() { Text = "AES", Value = "AES" },
            new HashType() { Text = "HMAC SHA-256", Value = "HMACSHA256" },
            new HashType() { Text = "HMAC SHA-384", Value = "HMACSHA384" },
            new HashType() { Text = "HMAC SHA-512", Value = "HMACSHA512" },
            new HashType() { Text = "MD5", Value = "MD5" },
            new HashType() { Text = "SHA-1", Value = "SHA1" },
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="GenerateMachineKeyModel"/> class.
        /// </summary>
        public GenerateMachineKeyModel()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GenerateMachineKeyModel"/> class.
        /// </summary>
        /// <param name="decryptionAlgorithmName">The name of the decryption algorithm.</param>
        /// <param name="validationAlgorithmName">The name of the validation algorithm.</param>
        public GenerateMachineKeyModel(string decryptionAlgorithmName, string validationAlgorithmName)
        {
            this.DecryptionAlgorithmName = decryptionAlgorithmName;
            this.ValidationAlgorithmName = validationAlgorithmName;
        }

        /// <summary>
        /// Gets or sets the decryption algorithm name.
        /// </summary>
        [Display(Name = "Decryption Algorithm")]
        public string DecryptionAlgorithmName { get; set; }

        /// <summary>
        /// Gets the available decryption algorithms
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "Required for use in the views.")]
        public IEnumerable<HashType> DecryptionAlgorithms
        {
            get { return DefaultDecryptionAlgorithms.ToArray(); }
        }

        /// <summary>
        /// Gets or sets the validation algorithm name.
        /// </summary>
        [Display(Name = "Validation Algorithm")]
        public string ValidationAlgorithmName { get; set; }

        /// <summary>
        /// Gets the available validation algorithms
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "Required for use in the views.")]
        public IEnumerable<HashType> ValidationAlgorithms
        {
            get { return DefaultValidationAlgorithms.ToArray(); }
        }
    }
}