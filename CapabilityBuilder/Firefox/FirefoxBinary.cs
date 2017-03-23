// <copyright file="FirefoxBinary.cs" company="WebDriver Committers">
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
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Security.Permissions;
using System.Text;
using System.Threading;

namespace OpenQA.Selenium.Firefox
{
    /// <summary>
    /// Represents the binary associated with Firefox.
    /// </summary>
    /// <remarks>The <see cref="FirefoxBinary"/> class is responsible for instantiating the
    /// Firefox process, and the operating system environment in which it runs.</remarks>
    public class FirefoxBinary : IDisposable
    {
        private Process process;
        private bool isDisposed = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="FirefoxBinary"/> class.
        /// </summary>
        public FirefoxBinary()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FirefoxBinary"/> class located at a specific file location.
        /// </summary>
        /// <param name="pathToFirefoxBinary">Full path and file name to the Firefox executable.</param>
        public FirefoxBinary(string pathToFirefoxBinary)
        {
        }

        /// <summary>
        /// Releases all resources used by the <see cref="FirefoxBinary"/>.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="FirefoxBinary"/> and optionally
        /// releases the managed resources.
        /// </summary>
        /// <param name="disposing"><see langword="true"/> to release managed and resources;
        /// <see langword="false"/> to only release unmanaged resources.</param>
        [SecurityPermission(SecurityAction.Demand)]
        protected virtual void Dispose(bool disposing)
        {
            if (!this.isDisposed)
            {
                if (disposing)
                {
                    // Suicide watch: First,  a second to see if the process will die on
                    // it's own (we will likely have asked the process to kill itself just
                    // before calling this method).
                    if (this.process != null)
                    {
                        if (!this.process.HasExited)
                        {
                            System.Threading.Thread.Sleep(1000);
                        }

                        // Murder option: The process is still alive, so kill it.
                        if (!this.process.HasExited)
                        {
                            this.process.Kill();
                        }

                        this.process.Dispose();
                        this.process = null;
                    }
                }

                this.isDisposed = true;
            }
        }
    }
}
