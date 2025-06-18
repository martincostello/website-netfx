// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ToolsPageTest.cs" company="http://www.martincostello.com">
//   Martin Costello (c) 2014
// </copyright>
// <summary>
//   ToolsPageTest.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Xml;
using MartinCostello.PageTemplates;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MartinCostello
{
    /// <summary>
    /// A class containing UI tests for the <c>/tools</c> page.
    /// </summary>
    [TestClass]
    public class ToolsPageTest : UITestBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ToolsPageTest"/> class.
        /// </summary>
        public ToolsPageTest()
            : base()
        {
        }

        [Ignore]
        [TestMethod]
        [TestCategory("UI")]
        [Description("Tests that the /tools/ page can generate GUIDs.")]
        [DataSource(ProviderName, BrowsersDataFileName, BrowserTypeTableName, DataAccessMethod.Sequential)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "MartinCostello.PageTemplates.ToolsPage.SelectGuidFormat(System.String)", Justification = "Not an issue for tests.")]
        public void Tools_Page_Can_Generate_Guids()
        {
            // Arrange
            // Act
            ToolsPage page = GoToUrl<ToolsPage>("/tools/");

            // Assert
            Assert.AreEqual("Default", page.GuidFormat, "The default GUID format is incorrect.");
            Assert.IsFalse(page.GenerateUppercaseGuid, "The default case for generated GUIDs is incorrect.");

            string lastGuid = AssertGuidGeneration(page, "D", false, string.Empty);

            // Act
            page.GenerateNewGuid();

            // Assert
            lastGuid = AssertGuidGeneration(page, "D", false, lastGuid);

            // Act
            page.ChangeGuidCase();

            // Assert
            lastGuid = AssertGuidGeneration(page, "D", true, lastGuid);

            // Act
            page.ChangeGuidCase();

            // Assert
            lastGuid = AssertGuidGeneration(page, "D", false, lastGuid);

            // Act
            page.SelectGuidFormat("Hexadecimal with braces");

            // Asset
            lastGuid = AssertGuidGeneration(page, "X", false, lastGuid);

            // Act
            page.SelectGuidFormat("Numeric");

            // Asset
            lastGuid = AssertGuidGeneration(page, "N", false, lastGuid);

            // Act
            page.SelectGuidFormat("With braces");

            // Asset
            lastGuid = AssertGuidGeneration(page, "B", false, lastGuid);

            // Act
            page.SelectGuidFormat("With parentheses");

            // Asset
            lastGuid = AssertGuidGeneration(page, "P", false, lastGuid);

            // Act
            page.SelectGuidFormat("Default");

            // Asset
            lastGuid = AssertGuidGeneration(page, "D", false, lastGuid);

            // Act and Assert
            for (int i = 0; i < 5; i++)
            {
                page.GenerateNewGuid();
                lastGuid = AssertGuidGeneration(page, "D", false, lastGuid);
            }

            /*// Act
            page.CopyGuidToClipboard();

            // Assert
            Assert.AreEqual(page.GeneratedGuid, Clipboard.GetText(), "An incorrect GUID was copied to the clipboard.");*/
        }

        [Ignore]
        [TestMethod]
        [TestCategory("UI")]
        [Description("Tests that the /tools/ page can generate hashes.")]
        [DataSource(ProviderName, BrowsersDataFileName, BrowserTypeTableName, DataAccessMethod.Sequential)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "MartinCostello.PageTemplates.ToolsPage.SelectGuidFormat(System.String)", Justification = "Not an issue for tests.")]
        public void Tools_Page_Can_Generate_Hashes()
        {
            // Arrange
            var testCases = new Tuple<string, string, string, string>[]
            {
                Tuple.Create("MD5", "Hexadecimal", string.Empty, "d41d8cd98f00b204e9800998ecf8427e"),
                Tuple.Create("SHA-1", "Hexadecimal", string.Empty, "da39a3ee5e6b4b0d3255bfef95601890afd80709"),
                Tuple.Create("SHA-256", "Hexadecimal", string.Empty, "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855"),
                Tuple.Create("SHA-384", "Hexadecimal", string.Empty, "38b060a751ac96384cd9327eb1b1e36a21fdb71114be07434c0cc7bf63f6e1da274edebfe76f65fbd51ad2f14898b95b"),
                Tuple.Create("SHA-512", "Hexadecimal", string.Empty, "cf83e1357eefb8bdf1542850d66d8007d620e4050b5715dc83f4a921d36ce9ce47d0d13c5d85f2b0ff8318d2877eec2f63b931bd47417a81a538327af927da3e"),
                Tuple.Create("MD5", "Base64", string.Empty, "1B2M2Y8AsgTpgAmY7PhCfg=="),
                Tuple.Create("SHA-1", "Base64", string.Empty, "2jmj7l5rSw0yVb/vlWAYkK/YBwk="),
                Tuple.Create("SHA-256", "Base64", string.Empty, "47DEQpj8HBSa+/TImW+5JCeuQeRkm5NMpJWZG3hSuFU="),
                Tuple.Create("SHA-384", "Base64", string.Empty, "OLBgp1GsljhM2TJ+sbHjaiH9txEUvgdDTAzHv2P24donTt6/529l+9Ua0vFImLlb"),
                Tuple.Create("SHA-512", "Base64", string.Empty, "z4PhNX7vuL3xVChQ1m2AB9Yg5AULVxXcg/SpIdNs6c5H0NE8XYXysP+DGNKHfuwvY7kxvUdBeoGlODJ6+SfaPg=="),
                Tuple.Create("MD5", "Hexadecimal", "martincostello.com", "e6c3105bdb8e6466f9db1dab47a85131"),
                Tuple.Create("SHA-1", "Hexadecimal", "martincostello.com", "7fbd8e8cf806e5282af895396f5268483bf6af1b"),
                Tuple.Create("SHA-256", "Hexadecimal", "martincostello.com", "3b8143aa8119eaf0910aef5cade45dd0e6bb7b70e8d1c8c057bf3fc125248642"),
                Tuple.Create("SHA-384", "Hexadecimal", "martincostello.com", "5c0e892a9348c184df255f46ab7282eb5792d552c896eb6893d90f36c7202540a9942c80ce5812616d29c08331c60510"),
                Tuple.Create("SHA-512", "Hexadecimal", "martincostello.com", "3be0167275455dcf1e34f8818d48b7ae4a61fb8549153f42d0d035464fdccee97022d663549eb249d4796956e4016ad83d5e64ba766fb751c8fb2c03b2b4eb9a"),
            };

            // Act
            ToolsPage page = GoToUrl<ToolsPage>("/tools/");

            // Assert
            Assert.AreEqual("Hexadecimal", page.HashFormat, "The default hash format is incorrect.");
            Assert.AreEqual("SHA-256", page.HashName, "The default hash name is incorrect.");
            Assert.AreEqual(string.Empty, page.HashPlaintext, "The default hash plaintext is incorrect.");

            foreach (var testCase in testCases)
            {
                // Act
                page.SelectHashName(testCase.Item1);
                page.SelectHashFormat(testCase.Item2);
                page.EnterHashPlaintext(testCase.Item3);
                page.GenerateHash();

                // Assert
                WaitUntil(() => testCase.Item4 == page.GeneratedHash);
            }
        }

        [Ignore]
        [TestMethod]
        [TestCategory("UI")]
        [Description("Tests that the /tools/ page can generate machine keys.")]
        [DataSource(ProviderName, BrowsersDataFileName, BrowserTypeTableName, DataAccessMethod.Sequential)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "MartinCostello.PageTemplates.ToolsPage.SelectGuidFormat(System.String)", Justification = "Not an issue for tests.")]
        public void Tools_Page_Can_Generate_Machine_Keys()
        {
            // Arrange
            var testCases = new Tuple<string, string, string, string, int, int>[]
            {
                Tuple.Create("AES (256 bits)", "SHA-1", "AES", "SHA1", 64, 128),
                Tuple.Create("3DES", "3DES", "3DES", "3DES", 48, 48),
                Tuple.Create("AES (128 bits)", "AES", "AES", "AES", 32, 64),
                Tuple.Create("AES (192 bits)", "HMAC SHA-256", "AES", "HMACSHA256", 48, 64),
                Tuple.Create("AES (256 bits)", "HMAC SHA-384", "AES", "HMACSHA384", 64, 96),
                Tuple.Create("DES", "HMAC SHA-512", "DES", "HMACSHA512", 64, 128),
                Tuple.Create("DES", "MD5", "DES", "MD5", 64, 32),
                Tuple.Create("DES", "SHA-1", "DES", "SHA1", 64, 128),
            };

            // Act
            ToolsPage page = GoToUrl<ToolsPage>("/tools/");

            // Assert
            Assert.AreEqual("AES (256 bits)", page.MachineKeyDecryptionAlgorithm, "The default machine key decryption algorithm is incorrect.");
            Assert.AreEqual("SHA-1", page.MachineKeyValidationAlgorithm, "The default machine key validation algorithm is incorrect.");

            string lastValue = string.Empty;

            foreach (var testCase in testCases)
            {
                // Act
                page.SelectDecryptionAlgorithm(testCase.Item1);
                page.SelectValidationAlgorithm(testCase.Item2);

                page.GenerateMachineKey();

                WaitUntil(() => lastValue != page.GeneratedMachineKey);

                XmlDocument document = new XmlDocument();
                document.LoadXml(page.GeneratedMachineKey);

                var node = document.SelectSingleNode("/machineKey/@decryption");
                Assert.IsNotNull(node, "The decryption attribute was not found.");
                Assert.AreEqual(testCase.Item3, node.Value, "The decryption attribute value is incorrect.");

                node = document.SelectSingleNode("/machineKey/@validation");
                Assert.IsNotNull(node, "The validation attribute was not found.");
                Assert.AreEqual(testCase.Item4, node.Value, "The validation attribute value is incorrect.");

                node = document.SelectSingleNode("/machineKey/@decryptionKey");
                Assert.IsNotNull(node, "The decryptionKey attribute was not found.");
                Assert.AreEqual(testCase.Item5, node.Value.Length, "The decryptionKey attribute value's length is incorrect.");

                node = document.SelectSingleNode("/machineKey/@validationKey");
                Assert.IsNotNull(node, "The validationKey attribute was not found.");
                Assert.AreEqual(testCase.Item6, node.Value.Length, "The validationKey attribute value's length is incorrect.");

                lastValue = page.GeneratedMachineKey;
            }
        }

        /// <summary>
        /// Asserts that the specified page has generated a valid GUID.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="expectedFormat">The expected format of the generated GUID.</param>
        /// <param name="isUppercase">Whether the generated GUID should be uppercase.</param>
        /// <param name="generatedGuids">The GUIDs already generated.</param>
        /// <param name="lastGuid">The last GUID that was generated.</param>
        /// <returns>
        /// The GUID that was generated.
        /// </returns>
        private string AssertGuidGeneration(ToolsPage page, string expectedFormat, bool isUppercase, string lastGuid)
        {
            WaitUntil(() => page.GeneratedGuid != lastGuid);

            string guidString = page.GeneratedGuid;

            Assert.IsTrue(
                Guid.TryParseExact(guidString, expectedFormat, out var guid),
                "The generated GUID '{0}' is not in the expected format '{1}'.",
                guidString,
                expectedFormat);

            Assert.AreEqual(
                isUppercase,
                guidString.Where((p) => char.IsLetter(p)).All((p) => char.IsUpper(p)),
                "The case of the generated GUID is incorrect. Expected uppercase: {0}. Generated GUID: '{1}'.",
                isUppercase,
                guidString);

            return guidString;
        }
    }
}
