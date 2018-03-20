// <copyright file="RemoteWebDriver.cs" company="WebDriver Committers">
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

using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text.RegularExpressions;

namespace OpenQA.Selenium.Remote
{
    /// <summary>
    /// Provides a way to use the driver through
    /// </summary>
    /// /// <example>
    /// <code>
    /// [TestFixture]
    /// public class Testing
    /// {
    ///     private IWebDriver driver;
    ///     <para></para>
    ///     [SetUp]
    ///     public void SetUp()
    ///     {
    ///         driver = new RemoteWebDriver(new Uri("http://127.0.0.1:4444/wd/hub"),DesiredCapabilities.InternetExplorer());
    ///     }
    ///     <para></para>
    ///     [Test]
    ///     public void TestGoogle()
    ///     {
    ///         driver.Navigate().GoToUrl("http://www.google.co.uk");
    ///         /*
    ///         *   Rest of the test
    ///         */
    ///     }
    ///     <para></para>
    ///     [TearDown]
    ///     public void TearDown()
    ///     {
    ///         driver.Quit();
    ///     }
    /// }
    /// </code>
    /// </example>
    public class RemoteWebDriver : IWebDriver
    {
        /// <summary>
        /// The default command timeout for HTTP requests in a RemoteWebDriver instance.
        /// </summary>
        protected static readonly TimeSpan DefaultCommandTimeout = TimeSpan.FromSeconds(60);
        private ICommandExecutor executor;

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteWebDriver"/> class.
        /// This constructor defaults proxy to http://127.0.0.1:4444/wd/hub
        /// </summary>
        /// <param name="remoteSettings">An <see cref="RemoteSessionOptions"/> object
        /// containing the desired capabilities of the browser.</param>
        public RemoteWebDriver(RemoteSessionOptions remoteSettings)
            : this(new Uri("http://127.0.0.1:4444/wd/hub"), remoteSettings)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteWebDriver"/> class using the
        /// specified remote address and desired capabilities.
        /// </summary>
        /// <param name="remoteAddress">URI containing the address of the WebDriver 
        /// remote server (e.g. http://127.0.0.1:4444/wd/hub ).</param>
        /// <param name="remoteSettings">An <see cref="RemoteSessionOptions"/> object
        /// containing the desired capabilities of the browser.</param>
        public RemoteWebDriver(Uri remoteAddress, RemoteSessionOptions remoteSettings)
            : this(remoteAddress, remoteSettings, RemoteWebDriver.DefaultCommandTimeout)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteWebDriver"/> class using the
        /// specified remote address and desired capabilities.
        /// </summary>
        /// <param name="remoteAddress">URI containing the address of the WebDriver 
        /// remote server (e.g. http://127.0.0.1:4444/wd/hub ).</param>
        /// <param name="browserOptions">An <see cref="DriverOptions"/> object
        /// containing the desired capabilities of the browser.</param>
        /// <remarks>
        /// This is a convenience constructor, designed to handle the most common 
        /// remote execution case. This is the case where the user wants to execute
        /// a specific browser, just perform the execution remotely.
        /// </remarks>
        public RemoteWebDriver(Uri remoteAddress, DriverOptions browserOptions)
            :this(remoteAddress, CreateRemoteSessionSettings(browserOptions))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteWebDriver"/> class using the
        /// specified remote address, desired capabilities, and command timeout.
        /// </summary>
        /// <param name="remoteAddress">URI containing the address of the WebDriver
        /// remote server (e.g. http://127.0.0.1:4444/wd/hub ).</param>
        /// <param name="remoteSettings">An <see cref="RemoteSessionOptions"/> object
        /// containing the desired capabilities of the browser.</param>
        /// <param name="commandTimeout">The maximum amount of time to wait for each command.</param>
        public RemoteWebDriver(Uri remoteAddress, RemoteSessionOptions remoteSettings, TimeSpan commandTimeout)
            : this(new HttpCommandExecutor(remoteAddress, commandTimeout), remoteSettings)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteWebDriver"/> class
        /// </summary>
        /// <param name="commandExecutor">An <see cref="ICommandExecutor"/> object
        /// which executes commands for the driver.</param>
        /// <param name="remoteSettings">An <see cref="RemoteSessionOptions"/> object
        /// containing the desired capabilities of the browser.</param>
        public RemoteWebDriver(ICommandExecutor commandExecutor, RemoteSessionOptions remoteSettings)
        {
            this.executor = commandExecutor;
            string serializedPayload = JsonConvert.SerializeObject(remoteSettings.ToDictionary(), Formatting.Indented);
            Console.WriteLine("The following payload would be sent across the wire in the new session command:\n{0}", serializedPayload);
        }

        private static RemoteSessionOptions CreateRemoteSessionSettings(DriverOptions browserOptions)
        {
            if (browserOptions == null)
            {
                throw new ArgumentNullException("browserOptions", "Options cannot be null");
            }

            RemoteSessionOptions remoteSettings = new RemoteSessionOptions();
            remoteSettings.AddFirstMatchDriverOption(browserOptions);
            return remoteSettings;
        }
    }
}
