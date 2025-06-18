// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ToolsPage.cs" company="http://www.martincostello.com">
//   Martin Costello (c) 2014
// </copyright>
// <summary>
//   ToolsPage.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;
using OpenQA.Selenium.Support.UI;

namespace MartinCostello.PageTemplates
{
    /// <summary>
    /// A class representing the page template for the <c>/Tools</c> page.
    /// </summary>
    public sealed class ToolsPage : PageBase
    {
        /// <summary>
        /// The button to copy a hash to the clipboard.
        /// </summary>
        [FindsBy(How = How.Id, Using = "button-copy-hash")]
        private IWebElement _hashCopyButton = null;

        /// <summary>
        /// The select list to select the format of the generated hash.
        /// </summary>
        [FindsBy(How = How.Id, Using = "dropdown-hash-format")]
        private IWebElement _hashFormatSelect = null;

        /// <summary>
        /// The select list to select the name of the generated hash.
        /// </summary>
        [FindsBy(How = How.Id, Using = "dropdown-hash-name")]
        private IWebElement _hashNameSelect = null;

        /// <summary>
        /// The button to generate a GUID.
        /// </summary>
        [FindsBy(How = How.Id, Using = "button-generate-hash")]
        private IWebElement _hashGenerateButton = null;

        /// <summary>
        /// The text area for entering the plaintext to hash.
        /// </summary>
        [FindsBy(How = How.Id, Using = "textarea-plaintext")]
        private IWebElement _hashPlaintext = null;

        /// <summary>
        /// The textbox containing the generated GUID.
        /// </summary>
        [FindsBy(How = How.Id, Using = "text-hash")]
        private IWebElement _hashTextbox = null;

        /// <summary>
        /// The checkbox to change the casing of the generated GUID.
        /// </summary>
        [FindsBy(How = How.Id, Using = "checkbox-guid-uppercase")]
        private IWebElement _guidCasingCheckbox = null;

        /// <summary>
        /// The button to copy a GUID to the clipboard.
        /// </summary>
        [FindsBy(How = How.Id, Using = "button-copy-guid")]
        private IWebElement _guidCopyButton = null;

        /// <summary>
        /// The select list to select the format of the generated GUID.
        /// </summary>
        [FindsBy(How = How.Id, Using = "dropdown-guid-format")]
        private IWebElement _guidFormatSelect = null;

        /// <summary>
        /// The button to generate a GUID.
        /// </summary>
        [FindsBy(How = How.Id, Using = "button-generate-guid")]
        private IWebElement _guidGenerateButton = null;

        /// <summary>
        /// The textbox containing the generated GUID.
        /// </summary>
        [FindsBy(How = How.Id, Using = "text-guid")]
        private IWebElement _guidTextbox = null;

        /// <summary>
        /// The element containing the generated machine key.
        /// </summary>
        [FindsBy(How = How.Id, Using = "code-machine-key")]
        private IWebElement _machineKeyCode = null;

        /// <summary>
        /// The button to copy a machine key to the clipboard.
        /// </summary>
        [FindsBy(How = How.Id, Using = "link-copy-machine-key")]
        private IWebElement _machineKeyCopyButton = null;

        /// <summary>
        /// The select list to select the machine key decryption algorithm.
        /// </summary>
        [FindsBy(How = How.Id, Using = "dropdown-decryption")]
        private IWebElement _machineKeyDecryptionSelect = null;

        /// <summary>
        /// The button to generate a machine key.
        /// </summary>
        [FindsBy(How = How.Id, Using = "button-generate-machine-key")]
        private IWebElement _machineKeyGenerateButton = null;

        /// <summary>
        /// The select list to select the machine key validation algorithm.
        /// </summary>
        [FindsBy(How = How.Id, Using = "dropdown-validation")]
        private IWebElement _machineKeyValidationSelect = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="ToolsPage"/> class.
        /// </summary>
        /// <param name="driver">The driver.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="driver"/> is <see langword="null"/>.
        /// </exception>
        public ToolsPage(IWebDriver driver)
            : base(driver)
        {
        }

        /// <summary>
        /// Gets a value indicating whether uppercase GUIDs should be generated.
        /// </summary>
        public bool GenerateUppercaseGuid
        {
            get { return _guidCasingCheckbox.Selected; }
        }

        /// <summary>
        /// Gets the current value of the generated GUID.
        /// </summary>
        public string GeneratedGuid
        {
            get { return _guidTextbox.GetAttribute("value"); }
        }

        /// <summary>
        /// Gets the current value of the generated hash.
        /// </summary>
        public string GeneratedHash
        {
            get { return _hashTextbox.GetAttribute("value"); }
        }

        /// <summary>
        /// Gets the current value of the generated machine key.
        /// </summary>
        public string GeneratedMachineKey
        {
            get { return _machineKeyCode.Text; }
        }

        /// <summary>
        /// Gets the format to generate GUIDs in.
        /// </summary>
        public string GuidFormat
        {
            get
            {
                SelectElement select = new SelectElement(_guidFormatSelect);
                return select.SelectedOption.Text;
            }
        }

        /// <summary>
        /// Gets the format to generate hashes in.
        /// </summary>
        public string HashFormat
        {
            get
            {
                SelectElement select = new SelectElement(_hashFormatSelect);
                return select.SelectedOption.Text;
            }
        }

        /// <summary>
        /// Gets the name of the hash algorithm to generate hashes in.
        /// </summary>
        public string HashName
        {
            get
            {
                SelectElement select = new SelectElement(_hashNameSelect);
                return select.SelectedOption.Text;
            }
        }

        /// <summary>
        /// Gets the decryption algorithm to use for machine keys.
        /// </summary>
        public string MachineKeyDecryptionAlgorithm
        {
            get
            {
                SelectElement select = new SelectElement(_machineKeyDecryptionSelect);
                return select.SelectedOption.Text;
            }
        }

        /// <summary>
        /// Gets the validation algorithm to use for machine keys.
        /// </summary>
        public string MachineKeyValidationAlgorithm
        {
            get
            {
                SelectElement select = new SelectElement(_machineKeyValidationSelect);
                return select.SelectedOption.Text;
            }
        }

        /// <summary>
        /// Gets the current value of the hash plaintext.
        /// </summary>
        public string HashPlaintext
        {
            get { return _hashPlaintext.GetAttribute("value"); }
        }

        /// <summary>
        /// Changes the case to generate GUIDs in.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if GUIDs are now being generated in uppercase; otherwise <see langword="false"/>.
        /// </returns>
        public bool ChangeGuidCase()
        {
            _guidCasingCheckbox.Click();
            return _guidCasingCheckbox.Selected;
        }

        /// <summary>
        /// Clicks the button to copy the generated GUID to the clipboard.
        /// </summary>
        public void CopyGuidToClipboard()
        {
            _guidCopyButton.Click();
        }

        /// <summary>
        /// Clicks the button to copy the generated hash to the clipboard.
        /// </summary>
        public void CopyHashToClipboard()
        {
            _hashCopyButton.Click();
        }

        /// <summary>
        /// Clicks the button to copy the generated machine key to the clipboard.
        /// </summary>
        public void CopyMachineKeyToClipboard()
        {
            _machineKeyCopyButton.Click();
        }

        /// <summary>
        /// Enters the specified plaintext to hash into the form.
        /// </summary>
        /// <param name="text">The text to enter.</param>
        public void EnterHashPlaintext(string text)
        {
            _hashPlaintext.Clear();
            _hashPlaintext.SendKeys(text);
        }

        /// <summary>
        /// Clicks the button to generate a new hash.
        /// </summary>
        public void GenerateHash()
        {
            _hashGenerateButton.Click();
        }

        /// <summary>
        /// Clicks the button to generate a new machine key.
        /// </summary>
        public void GenerateMachineKey()
        {
            _machineKeyGenerateButton.Click();
        }

        /// <summary>
        /// Clicks the button to generate a new GUID.
        /// </summary>
        public void GenerateNewGuid()
        {
            _guidGenerateButton.Click();
        }

        /// <summary>
        /// Selects the GUID format with the specified text.
        /// </summary>
        /// <param name="text">The text to select the GUID format for.</param>
        /// <returns>
        /// The currently selected GUID generation format.
        /// </returns>
        public string SelectGuidFormat(string text)
        {
            return SelectValueByText(_guidFormatSelect, text);
        }

        /// <summary>
        /// Selects the hash format with the specified text.
        /// </summary>
        /// <param name="text">The text to select the hash format for.</param>
        /// <returns>
        /// The currently selected hash generation format.
        /// </returns>
        public string SelectHashFormat(string text)
        {
            return SelectValueByText(_hashFormatSelect, text);
        }

        /// <summary>
        /// Selects the hash algorithm name with the specified text.
        /// </summary>
        /// <param name="text">The text to select the hash name for.</param>
        /// <returns>
        /// The currently selected hash name.
        /// </returns>
        public string SelectHashName(string text)
        {
            return SelectValueByText(_hashNameSelect, text);
        }

        /// <summary>
        /// Selects the machine key decryption algorithm name with the specified text.
        /// </summary>
        /// <param name="text">The text to select the decryption algorithm for.</param>
        /// <returns>
        /// The currently selected decryption algorithm.
        /// </returns>
        public string SelectDecryptionAlgorithm(string text)
        {
            return SelectValueByText(_machineKeyDecryptionSelect, text);
        }

        /// <summary>
        /// Selects the machine key validation algorithm name with the specified text.
        /// </summary>
        /// <param name="text">The text to select the validation algorithm for.</param>
        /// <returns>
        /// The currently selected validation algorithm.
        /// </returns>
        public string SelectValidationAlgorithm(string text)
        {
            return SelectValueByText(_machineKeyValidationSelect, text);
        }

        /// <summary>
        /// Selects the value with the specified text from the specified <see cref="IWebElement"/>.
        /// </summary>
        /// <param name="element">The element to select the value in.</param>
        /// <param name="text">The text of the value to select.</param>
        /// <returns>
        /// The currently selected value.
        /// </returns>
        private static string SelectValueByText(IWebElement element, string text)
        {
            SelectElement select = new SelectElement(element);
            select.SelectByText(text);

            return select.SelectedOption.Text;
        }
    }
}