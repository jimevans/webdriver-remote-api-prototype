// <copyright file="FirefoxDriver.cs" company="WebDriver Committers">
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

using System;
using System.Collections.Generic;
using System.IO;
using OpenQA.Selenium.Remote;

namespace OpenQA.Selenium.Firefox
{
    /// <summary>
    /// Provides a way to access Firefox to run tests.
    /// </summary>
    /// <remarks>
    /// When the FirefoxDriver object has been instantiated the browser will load. The test can then navigate to the URL under test and
    /// start your test.
    /// <para>
    /// In the case of the FirefoxDriver, you can specify a named profile to be used, or you can let the
    /// driver create a temporary, anonymous profile. A custom extension allowing the driver to communicate
    /// to the browser will be installed into the profile.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// [TestFixture]
    /// public class Testing
    /// {
    ///     private IWebDriver driver;
    ///     <para></para>
    ///     [SetUp]
    ///     public void SetUp()
    ///     {
    ///         driver = new FirefoxDriver();
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
    public class FirefoxDriver : RemoteWebDriver
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FirefoxDriver"/> class.
        /// </summary>
        public FirefoxDriver()
            : this(new FirefoxOptions(null, null))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FirefoxDriver"/> class for a given profile.
        /// </summary>
        /// <param name="profile">A <see cref="FirefoxProfile"/> object representing the profile settings
        /// to be used in starting Firefox.</param>
        public FirefoxDriver(FirefoxProfile profile)
            : this(new FirefoxOptions(profile, null))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FirefoxDriver"/> class using the specified options. Uses the Mozilla-provided Marionette driver implementation.
        /// </summary>
        /// <param name="options">The <see cref="FirefoxOptions"/> to be used with the Firefox driver.</param>
        public FirefoxDriver(FirefoxOptions options)
            : this(FirefoxDriverService.CreateDefaultService(), options, RemoteWebDriver.DefaultCommandTimeout)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FirefoxDriver"/> class using the specified driver service. Uses the Mozilla-provided Marionette driver implementation.
        /// </summary>
        /// <param name="service">The <see cref="FirefoxDriverService"/> used to initialize the driver.</param>
        public FirefoxDriver(FirefoxDriverService service)
            : this(service, new FirefoxOptions(), RemoteWebDriver.DefaultCommandTimeout)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FirefoxDriver"/> class using the specified options, driver service, and timeout. Uses the Mozilla-provided Marionette driver implementation.
        /// </summary>
        /// <param name="service">The <see cref="FirefoxDriverService"/> to use.</param>
        /// <param name="options">The <see cref="FirefoxOptions"/> to be used with the Firefox driver.</param>
        /// <param name="commandTimeout">The maximum amount of time to wait for each command.</param>
        public FirefoxDriver(FirefoxDriverService service, FirefoxOptions options, TimeSpan commandTimeout)
            : base(CreateExecutor(service, options, commandTimeout), ConvertOptionsToCapabilities(options))
        {
        }

        private static ICommandExecutor CreateExecutor(FirefoxDriverService service, FirefoxOptions options, TimeSpan commandTimeout)
        {
            if (service == null)
            {
                throw new ArgumentNullException("service", "You requested a service-based implementation, but passed in a null service object.");
            }

            return new DriverServiceCommandExecutor(service, commandTimeout);
        }

        private static RemoteSessionSettings ConvertOptionsToCapabilities(FirefoxOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException("options", "options must not be null");
            }

            return new RemoteSessionSettings(options);
        }
    }
}