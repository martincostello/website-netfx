// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ToolsController.cs" company="http://www.martincostello.com">
//   Martin Costello (c) 2014
// </copyright>
// <summary>
//   ToolsController.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using log4net;
using MartinCostello.Models;

namespace MartinCostello.Controllers
{
    /// <summary>
    /// A class representing the controller for the <c>Tools</c> views.
    /// </summary>
    public class ToolsController : Controller
    {
        /// <summary>
        /// The <see cref="ILog"/> to use. This field is read-only.
        /// </summary>
        private static readonly ILog ToolsLogger = LogManager.GetLogger(typeof(ToolsController));

        /// <summary>
        /// An <see cref="IDictionary{K, V}"/> containing the sizes of the decryption and validation hashes for machine keys.
        /// </summary>
        private static readonly IDictionary<string, int> HashSizes = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
        {
            { "3DES-D", 24 },
            { "3DES-V", 24 },
            { "AES-128-D", 16 },
            { "AES-192-D", 24 },
            { "AES-256-D", 32 },
            { "AES-V", 32 },
            { "DES-D", 32 },
            { "MD5-V", 16 },
            { "HMACSHA256-V", 32 },
            { "HMACSHA384-V", 48 },
            { "HMACSHA512-V", 64 },
            { "SHA1-V", 64 },
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="ToolsController"/> class.
        /// </summary>
        public ToolsController()
            : base()
        {
        }

        /// <summary>
        /// Returns the <c>_GenerateGuid</c> view.
        /// </summary>
        /// <returns>
        /// The action result of the view.
        /// </returns>
        [HttpGet]
        [ActionName("_GenerateGuid")]
        public ActionResult GenerateGuid()
        {
            return PartialView();
        }

        /// <summary>
        /// Returns the <c>_GenerateGuid</c> view.
        /// </summary>
        /// <param name="format">The GUID format string.</param>
        /// <param name="uppercase">Whether to generate the GUID in upper case.</param>
        /// <returns>
        /// The action result of the view.
        /// </returns>
        [HttpPost]
        [ActionName("_GenerateGuid")]
        public ActionResult GenerateGuid(string format, bool uppercase)
        {
            ToolsLogger.Info("Generating a new GUID.");
            ValidateAntiForgeryTokenAjax();

            string data = Guid.NewGuid().ToString(format ?? "D", CultureInfo.InvariantCulture);

            if (uppercase)
            {
                data = data.ToUpperInvariant();
            }

            return Json(new { guid = data });
        }

        /// <summary>
        /// Returns the <c>_GenerateHash</c> view.
        /// </summary>
        /// <returns>
        /// The action result of the view.
        /// </returns>
        [HttpGet]
        [ActionName("_GenerateHash")]
        public ActionResult GenerateHash()
        {
            GenerateHashModel model = new GenerateHashModel()
            {
                Format = HashFormat.Hexadecimal,
                HashName = "SHA256",
                Plaintext = string.Empty,
            };

            return PartialView(model);
        }

        /// <summary>
        /// Returns the <c>_GenerateHash</c> view.
        /// </summary>
        /// <param name="model">The view model to use to generate the hash.</param>
        /// <returns>
        /// The action result of the view.
        /// </returns>
        /// <exception cref="HttpException">
        /// <paramref name="model"/> is invalid.
        /// </exception>
        [HttpPost]
        [ActionName("_GenerateHash")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Usage",
            "CA2202:Do not dispose objects multiple times",
            Justification = "It is not disposed multiple times.")]
        public ActionResult GenerateHash(GenerateHashModel model)
        {
            ToolsLogger.Info("Generating a new hash.");
            ValidateAntiForgeryTokenAjax();

            if (model == null)
            {
                throw new HttpException(400, "No hash data specified.");
            }

            const int MaxPlaintextLength = 4096;

            if (model.Plaintext != null &&
                model.Plaintext.Length > MaxPlaintextLength)
            {
                string message = string.Format(CultureInfo.InvariantCulture, "The plaintext to hash cannot be more than {0} characters in length.", MaxPlaintextLength);
                throw new HttpException(400, message);
            }

            if (string.IsNullOrEmpty(model.HashName))
            {
                model.HashName = "SHA256";
            }

            byte[] hash;

            using (Stream stream = new MemoryStream())
            {
                using (TextWriter writer = new StreamWriter(stream, Encoding.ASCII, MaxPlaintextLength, true))
                {
                    writer.Write(model.Plaintext ?? string.Empty);
                    writer.Flush();
                }

                stream.Seek(0, SeekOrigin.Begin);

                using (HashAlgorithm algorithm = HashAlgorithm.Create(model.HashName))
                {
                    hash = algorithm.ComputeHash(stream);
                }
            }

            string result;

            switch (model.Format)
            {
                case HashFormat.Base64:
                    result = Convert.ToBase64String(hash, Base64FormattingOptions.None);
                    break;

                case HashFormat.Hexadecimal:
                default:
                    result = BytesToHexString(hash);
                    break;
            }

            return Json(new { hash = result });
        }

        /// <summary>
        /// Returns the <c>_GenerateMachineKey</c> view.
        /// </summary>
        /// <returns>
        /// The action result of the view.
        /// </returns>
        [HttpGet]
        [ActionName("_GenerateMachineKey")]
        public ActionResult GenerateMachineKey()
        {
            return PartialView();
        }

        /// <summary>
        /// Returns the <c>_GenerateMachineKey</c> view.
        /// </summary>
        /// <param name="model">The view model to use to generate the machine key.</param>
        /// <returns>
        /// The action result of the view.
        /// </returns>
        [HttpPost]
        [ActionName("_GenerateMachineKey")]
        public ActionResult GenerateMachineKey(GenerateMachineKeyModel model)
        {
            ToolsLogger.Info("Generating a new machine key.");
            ValidateAntiForgeryTokenAjax();

            if (model == null)
            {
                throw new HttpException(400, "No machine key data specified.");
            }

            const string TagFormat =
@"<machineKey validationKey=""{0}""
            decryptionKey=""{1}""
            validation=""{2}""
            decryption=""{3}"" />";

            int decryptionKeyLength;
            int validationKeyLength;

            if (string.IsNullOrEmpty(model.DecryptionAlgorithmName) ||
                !HashSizes.TryGetValue(model.DecryptionAlgorithmName + "-D", out decryptionKeyLength))
            {
                throw new HttpException(400, "The specified decryption algorithm name is invalid.");
            }

            if (string.IsNullOrEmpty(model.ValidationAlgorithmName) ||
                !HashSizes.TryGetValue(model.ValidationAlgorithmName + "-V", out validationKeyLength))
            {
                throw new HttpException(400, "The specified validation algorithm name is invalid.");
            }

            byte[] decryptionKeyBytes = new byte[decryptionKeyLength];
            byte[] validationKeyBytes = new byte[validationKeyLength];

            string tag;

            try
            {
                using (RandomNumberGenerator random = RandomNumberGenerator.Create())
                {
                    random.GetBytes(decryptionKeyBytes);
                }

                using (RandomNumberGenerator random = RandomNumberGenerator.Create())
                {
                    random.GetBytes(validationKeyBytes);
                }

                string decryptionAlgorithmName = model.DecryptionAlgorithmName;

                int indexOfDash = decryptionAlgorithmName.LastIndexOf('-');

                if (indexOfDash > -1)
                {
                    decryptionAlgorithmName = decryptionAlgorithmName.Substring(0, indexOfDash);
                }

                tag = string.Format(
                    CultureInfo.InvariantCulture,
                    TagFormat,
                    BytesToHexString(validationKeyBytes).ToUpperInvariant(),
                    BytesToHexString(decryptionKeyBytes).ToUpperInvariant(),
                    model.ValidationAlgorithmName,
                    decryptionAlgorithmName);
            }
            finally
            {
                Array.Clear(decryptionKeyBytes, 0, decryptionKeyBytes.Length);
                Array.Clear(validationKeyBytes, 0, validationKeyBytes.Length);
            }

            object data = new
            {
                tag = tag,
            };

            return Json(data);
        }

        /// <summary>
        /// Returns the <c>Index</c> view.
        /// </summary>
        /// <returns>
        /// The action result of the view.
        /// </returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Returns a <see cref="string"/> containing a hexadecimal representation of the specified <see cref="Array"/> of bytes.
        /// </summary>
        /// <param name="buffer">The buffer to generate the hash string for.</param>
        /// <returns>
        /// A <see cref="string"/> containing the hexadecimal representation of <paramref name="buffer"/>.
        /// </returns>
        private static string BytesToHexString(byte[] buffer)
        {
            return string.Concat(buffer.Select((p) => p.ToString("x2", CultureInfo.InvariantCulture)));
        }

        /// <summary>
        /// Validates the anti-forgery token for the current HTTP request
        /// when submitted from an AJAX POST request.
        /// </summary>
        private void ValidateAntiForgeryTokenAjax()
        {
            string cookieToken = string.Empty;
            string formToken = string.Empty;

            string tokenHeaders = this.Request.Headers[SecurityHelpers.AjaxAntiForgeryTokenName];

            if (!string.IsNullOrEmpty(tokenHeaders))
            {
                string[] split = tokenHeaders.Split(':');

                if (split.Length == 2)
                {
                    cookieToken = split[0].Trim();
                    formToken = split[1].Trim();
                }
            }

            AntiForgery.Validate(cookieToken, formToken);
        }
    }
}