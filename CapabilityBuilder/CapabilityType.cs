﻿// <copyright file="CapabilityType.cs" company="WebDriver Committers">
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

namespace OpenQA.Selenium.Remote
{
    /// <summary>
    /// Provides types of capabilities for the DesiredCapabilities object.
    /// </summary>
    public static class CapabilityType
    {
        /// <summary>
        /// Capability name used for the browser name.
        /// </summary>
        public static readonly string BrowserName = "browserName";

        /// <summary>
        /// Capability name used for the browser version.
        /// </summary>
        public static readonly string BrowserVersion = "browserVersion";

        /// <summary>
        /// Capability name used for the platform name.
        /// </summary>
        public static readonly string PlatformName = "platformName";

        /// <summary>
        /// Capability name used to indicate whether the browser accepts SSL certificates.
        /// </summary>
        public static readonly string AcceptInsecureCertificates = "acceptInsecureCerts";

        /// <summary>
        /// Capability name used to indicate how the browser handles unexpected alerts.
        /// </summary>
        public static readonly string UnhandledPromptBehavior = "unhandledPromptBehavior";

        /// <summary>
        /// Capability name used to indicate the page load strategy for the browser.
        /// </summary>
        public static readonly string PageLoadStrategy = "pageLoadStrategy";

        /// <summary>
        /// Capability name used to indicate the proxy for the browser.
        /// </summary>
        public static readonly string Proxy = "proxy";

        /// <summary>
        /// Capability name used to indicate the logging preferences for the session.
        /// </summary>
        public static readonly string LoggingPreferences = "loggingPrefs";
    }
}
