﻿// <copyright file="DesiredCapabilities.cs" company="WebDriver Committers">
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

using System.Collections.Generic;
using System.Globalization;

namespace OpenQA.Selenium.Remote
{
    /// <summary>
    /// Class to Create the capabilities of the browser you require for <see cref="IWebDriver"/>.
    /// If you wish to use default values use the static methods
    /// </summary>
    public class DesiredCapabilities : ICapabilities
    {
        private readonly Dictionary<string, object> capabilities = new Dictionary<string, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="DesiredCapabilities"/> class
        /// </summary>
        /// <param name="browser">Name of the browser e.g. firefox, internet explorer, safari</param>
        /// <param name="version">Version of the browser</param>
        /// <param name="platform">The platform it works on</param>
        public DesiredCapabilities(string browser)
        {
            this.SetCapability(CapabilityType.BrowserName, browser);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DesiredCapabilities"/> class
        /// </summary>
        public DesiredCapabilities()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DesiredCapabilities"/> class
        /// </summary>
        /// <param name="rawMap">Dictionary of items for the remote driver</param>
        /// <example>
        /// <code>
        /// DesiredCapabilities capabilities = new DesiredCapabilities(new Dictionary<![CDATA[<string,object>]]>(){["browserName","firefox"],["version",string.Empty],["javaScript",true]});
        /// </code>
        /// </example>
        public DesiredCapabilities(Dictionary<string, object> rawMap)
        {
            if (rawMap != null)
            {
                foreach (string key in rawMap.Keys)
                {
                    this.SetCapability(key, rawMap[key]);
                }
            }
        }

        /// <summary>
        /// Gets the browser name
        /// </summary>
        public string BrowserName
        {
            get
            {
                string name = string.Empty;
                object capabilityValue = this.GetCapability(CapabilityType.BrowserName);
                if (capabilityValue != null)
                {
                    name = capabilityValue.ToString();
                }

                return name;
            }
        }

        /// <summary>
        /// Gets the internal capabilities dictionary.
        /// </summary>
        internal Dictionary<string, object> CapabilitiesDictionary
        {
            get { return this.capabilities; }
        }

        /// <summary>
        /// Method to return a new DesiredCapabilities using defaults
        /// </summary>
        /// <returns>New instance of DesiredCapabilities for use with Firefox</returns>
        public static DesiredCapabilities Firefox()
        {
            return new DesiredCapabilities("firefox");
        }

        /// <summary>
        /// Method to return a new DesiredCapabilities using defaults
        /// </summary>
        /// <returns>New instance of DesiredCapabilities for use with Firefox</returns>
        public static DesiredCapabilities PhantomJS()
        {
            return new DesiredCapabilities("phantomjs");
        }

        /// <summary>
        /// Method to return a new DesiredCapabilities using defaults
        /// </summary>
        /// <returns>New instance of DesiredCapabilities for use with Internet Explorer</returns>
        public static DesiredCapabilities InternetExplorer()
        {
            return new DesiredCapabilities("internet explorer");
        }

        /// <summary>
        /// Method to return a new DesiredCapabilities using defaults
        /// </summary>
        /// <returns>New instance of DesiredCapabilities for use with Microsoft Edge</returns>
        public static DesiredCapabilities Edge()
        {
            return new DesiredCapabilities("MicrosoftEdge");
        }

        /// <summary>
        /// Method to return a new DesiredCapabilities using defaults
        /// </summary>
        /// <returns>New instance of DesiredCapabilities for use with Chrome</returns>
        public static DesiredCapabilities Chrome()
        {
            // This is strangely inconsistent.
            DesiredCapabilities dc = new DesiredCapabilities("chrome");
            return dc;
        }

        /// <summary>
        /// Method to return a new DesiredCapabilities using defaults
        /// </summary>
        /// <returns>New instance of DesiredCapabilities for use with Opera</returns>
        public static DesiredCapabilities Opera()
        {
            return new DesiredCapabilities("opera");
        }

        /// <summary>
        /// Method to return a new DesiredCapabilities using defaults
        /// </summary>
        /// <returns>New instance of DesiredCapabilities for use with Safari</returns>
        public static DesiredCapabilities Safari()
        {
            return new DesiredCapabilities("safari");
        }

        /// <summary>
        /// Gets a value indicating whether the browser has a given capability.
        /// </summary>
        /// <param name="capability">The capability to get.</param>
        /// <returns>Returns <see langword="true"/> if the browser has the capability; otherwise, <see langword="false"/>.</returns>
        public bool HasCapability(string capability)
        {
            return this.capabilities.ContainsKey(capability);
        }

        /// <summary>
        /// Gets a capability of the browser.
        /// </summary>
        /// <param name="capability">The capability to get.</param>
        /// <returns>An object associated with the capability, or <see langword="null"/>
        /// if the capability is not set on the browser.</returns>
        public object GetCapability(string capability)
        {
            object capabilityValue = null;
            if (this.capabilities.ContainsKey(capability))
            {
                capabilityValue = this.capabilities[capability];
                string capabilityValueString = capabilityValue as string;
            }

            return capabilityValue;
        }

        /// <summary>
        /// Sets a capability of the browser.
        /// </summary>
        /// <param name="capability">The capability to get.</param>
        /// <param name="capabilityValue">The value for the capability.</param>
        public void SetCapability(string capability, object capabilityValue)
        {
            this.capabilities[capability] = capabilityValue;
        }

        /// <summary>
        /// Converts the <see cref="ICapabilities"/> object to a <see cref="Dictionary{TKey, TValue}"/>.
        /// </summary>
        /// <returns>The <see cref="Dictionary{TKey, TValue}"/> containing the capabilities.</returns>
        public Dictionary<string, object> ToDictionary()
        {
            // CONSIDER: Instead of returning the raw internal member,
            // we might want to copy/clone it instead.
            return this.capabilities;
        }

        /// <summary>
        /// Return HashCode for the DesiredCapabilities that has been created
        /// </summary>
        /// <returns>Integer of HashCode generated</returns>
        public override int GetHashCode()
        {
            int result;
            result = this.BrowserName != null ? this.BrowserName.GetHashCode() : 0;
            return result;
        }

        /// <summary>
        /// Return a string of capabilities being used
        /// </summary>
        /// <returns>String of capabilities being used</returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "Capabilities [BrowserName={0}", this.BrowserName);
        }

        /// <summary>
        /// Compare two DesiredCapabilities and will return either true or false
        /// </summary>
        /// <param name="obj">DesiredCapabilities you wish to compare</param>
        /// <returns>true if they are the same or false if they are not</returns>
        public override bool Equals(object obj)
        {
            if (this == obj)
            {
                return true;
            }

            DesiredCapabilities other = obj as DesiredCapabilities;
            if (other == null)
            {
                return false;
            }

            return true;
        }
    }
}
