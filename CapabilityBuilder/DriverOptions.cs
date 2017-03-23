// <copyright file="DriverOptions.cs" company="WebDriver Committers">
// Licensed to the Software Freedom Conservancy (SFC) under one
// or more contributor license agreements. See the NOTICE file
// distributed with this work for additional information
// regarding copyright ownership. The SFC licenses this file
// to you under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>

using OpenQA.Selenium.Remote;
using System;
using System.Collections.Generic;

namespace OpenQA.Selenium
{

    /// <summary>
    /// Specifies the behavior of handling unexpected alerts in the IE driver.
    /// </summary>
    public enum UnhandledPromptBehavior
    {
        /// <summary>
        /// Indicates the behavior is not set.
        /// </summary>
        Default,

        /// <summary>
        /// Ignore unexpected alerts, such that the user must handle them.
        /// </summary>
        Ignore,

        /// <summary>
        /// Accept unexpected alerts.
        /// </summary>
        Accept,

        /// <summary>
        /// Dismiss unexpected alerts.
        /// </summary>
        Dismiss
    }

    /// <summary>
    /// Specifies the behavior of waiting for page loads in the IE driver.
    /// </summary>
    public enum PageLoadStrategy
    {
        /// <summary>
        /// Indicates the behavior is not set.
        /// </summary>
        Default,

        /// <summary>
        /// Waits for pages to load and ready state to be 'complete'.
        /// </summary>
        Normal,

        /// <summary>
        /// Waits for pages to load and for ready state to be 'interactive' or 'complete'.
        /// </summary>
        Eager,

        /// <summary>
        /// Does not wait for pages to load, returning immediately.
        /// </summary>
        None
    }

    /// <summary>
    /// Base class for managing options specific to a browser driver.
    /// </summary>
    public class DriverOptions
    {
        private Dictionary<string, LogLevel> loggingPreferences = new Dictionary<string, LogLevel>();
        private string browserName;
        private string browserVersion;
        private string platformName;
        private Proxy proxy;
        private bool? acceptInsecureCertificates;
        private UnhandledPromptBehavior unhandledPromptBehavior = UnhandledPromptBehavior.Default;
        private PageLoadStrategy pageLoadStrategy = PageLoadStrategy.Default;
        private Dictionary<string, object> additionalCapabilities = new Dictionary<string, object>();

        public string BrowserName
        {
            get { return this.browserName; }
            protected set { this.browserName = value; }
        }

        public string BrowserVersion
        {
            get { return this.browserVersion; }
            set { this.browserVersion = value; }
        }

        public string PlatformName
        {
            get { return this.platformName; }
            set { this.platformName = value; }
        }

        public Proxy Proxy
        {
            get { return this.proxy; }
            set { this.proxy = value; }
        }

        public bool? AcceptInsecureCertificates
        {
            get { return this.acceptInsecureCertificates; }
            set { this.acceptInsecureCertificates = value; }
        }

        public UnhandledPromptBehavior UnhandledPromptBehavior
        {
            get { return this.unhandledPromptBehavior; }
            set { this.unhandledPromptBehavior = value; }
        }

        public PageLoadStrategy PageLoadStrategy
        {
            get { return this.pageLoadStrategy; }
            set { this.pageLoadStrategy = value; }
        }

        /// <summary>
        /// Provides a means to add additional capabilities not yet added as type safe options
        /// for the specific browser driver.
        /// </summary>
        /// <param name="capabilityName">The name of the capability to add.</param>
        /// <param name="capabilityValue">The value of the capability to add.</param>
        /// <exception cref="ArgumentException">
        /// thrown when attempting to add a capability for which there is already a type safe option, or
        /// when <paramref name="capabilityName"/> is <see langword="null"/> or the empty string.
        /// </exception>
        /// <remarks>Calling <see cref="AddAdditionalCapability(string, object)"/>
        /// where <paramref name="capabilityName"/> has already been added will overwrite the
        /// existing value with the new value in <paramref name="capabilityValue"/>.
        /// </remarks>
        public virtual void AddAdditionalCapability(string capabilityName, object capabilityValue)
        {
            if (string.IsNullOrEmpty(capabilityName))
            {
                throw new ArgumentException("Capability name may not be null an empty string.", "capabilityName");
            }

            this.additionalCapabilities[capabilityName] = capabilityValue;
        }

        /// <summary>
        /// Returns DesiredCapabilities for the specific browser driver with these
        /// options included ascapabilities. This does not copy the options. Further
        /// changes will be reflected in the returned capabilities.
        /// </summary>
        /// <returns>The DesiredCapabilities for browser driver with these options.</returns>
        public virtual ICapabilities ToCapabilities()
        {
            DesiredCapabilities capabilities = new DesiredCapabilities();
            if (!string.IsNullOrEmpty(this.browserName))
            {
                capabilities.SetCapability(CapabilityType.BrowserName, this.browserName);
            }

            if (!string.IsNullOrEmpty(this.browserVersion))
            {
                capabilities.SetCapability(CapabilityType.BrowserVersion, this.browserVersion);
            }

            if (!string.IsNullOrEmpty(this.platformName))
            {
                capabilities.SetCapability(CapabilityType.PlatformName, this.platformName);
            }

            if (this.acceptInsecureCertificates.HasValue)
            {
                capabilities.SetCapability(CapabilityType.AcceptInsecureCertificates, this.acceptInsecureCertificates);
            }

            if (this.pageLoadStrategy != PageLoadStrategy.Default)
            {
                string pageLoadStrategySetting = "normal";
                switch (this.pageLoadStrategy)
                {
                    case PageLoadStrategy.Eager:
                        pageLoadStrategySetting = "eager";
                        break;

                    case PageLoadStrategy.None:
                        pageLoadStrategySetting = "none";
                        break;
                }

                capabilities.SetCapability(CapabilityType.PageLoadStrategy, pageLoadStrategySetting);
            }

            if (this.unhandledPromptBehavior != UnhandledPromptBehavior.Default)
            {
                string unexpectedAlertBehaviorSetting = "dismiss";
                switch (this.unhandledPromptBehavior)
                {
                    case UnhandledPromptBehavior.Ignore:
                        unexpectedAlertBehaviorSetting = "ignore";
                        break;

                    case UnhandledPromptBehavior.Accept:
                        unexpectedAlertBehaviorSetting = "accept";
                        break;
                }

                capabilities.SetCapability(CapabilityType.UnhandledPromptBehavior, unexpectedAlertBehaviorSetting);
            }

            if (this.proxy != null)
            {
                capabilities.SetCapability(CapabilityType.Proxy, this.Proxy);
            }

            foreach (KeyValuePair<string, object> pair in this.additionalCapabilities)
            {
                capabilities.SetCapability(pair.Key, pair.Value);
            }

            return capabilities;

        }

        /// <summary>
        /// Compares this <see cref="DriverOptions"/> object with another to see if there
        /// are merge conflicts between them.
        /// </summary>
        /// <param name="other">The <see cref="DriverOptions"/> object to compare with.</param>
        /// <returns><see langword="true"/>If there is a merge conflict; otherwise <see langword="false"/>.</returns>
        public virtual DriverOptionsMergeResult TryMerge(DriverOptions other)
        {
            DriverOptionsMergeResult result = new DriverOptionsMergeResult();
            if (this.browserName != null && other.BrowserName != null)
            {
                result.IsMergeConflict = true;
                result.MergeConflictOptionName = "BrowserName";
                return result;
            }

            if (this.browserVersion != null && other.BrowserVersion != null)
            {
                result.IsMergeConflict = true;
                result.MergeConflictOptionName = "BrowserVersion";
                return result;
            }

            if (this.platformName != null && other.PlatformName != null)
            {
                result.IsMergeConflict = true;
                result.MergeConflictOptionName = "PlatformName";
                return result;
            }

            if (this.proxy != null && other.Proxy != null)
            {
                result.IsMergeConflict = true;
                result.MergeConflictOptionName = "Proxy";
                return result;
            }

            if (this.unhandledPromptBehavior != UnhandledPromptBehavior.Default && other.UnhandledPromptBehavior != UnhandledPromptBehavior)
            {
                result.IsMergeConflict = true;
                result.MergeConflictOptionName = "UnhandledPromptBehavior";
                return result;
            }

            if (this.pageLoadStrategy != PageLoadStrategy.Default && other.PageLoadStrategy != PageLoadStrategy.Default)
            {
                result.IsMergeConflict = true;
                result.MergeConflictOptionName = "PageLoadStrategy";
                return result;
            }

            return result;
        }

        /// <summary>
        /// Sets the logging preferences for this driver.
        /// </summary>
        /// <param name="logType">The type of log for which to set the preference.
        /// Known log types can be found in the <see cref="LogType"/> class.</param>
        /// <param name="logLevel">The <see cref="LogLevel"/> value to which to set the log level.</param>
        public void SetLoggingPreference(string logType, LogLevel logLevel)
        {
            this.loggingPreferences[logType] = logLevel;
        }

        /// <summary>
        /// Generates the logging preferences dictionary for transmission as a desired capability.
        /// </summary>
        /// <returns>The dictionary containing the logging preferences.</returns>
        protected Dictionary<string, object> GenerateLoggingPreferencesDictionary()
        {
            if (this.loggingPreferences.Count == 0)
            {
                return null;
            }

            Dictionary<string, object> loggingPreferenceCapability = new Dictionary<string, object>();
            foreach (string logType in this.loggingPreferences.Keys)
            {
                loggingPreferenceCapability[logType] = this.loggingPreferences[logType].ToString().ToUpperInvariant();
            }

            return loggingPreferenceCapability;
        }
    }
}
