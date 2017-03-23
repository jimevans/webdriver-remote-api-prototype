// <copyright file="Response.cs" company="WebDriver Committers">
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
using System.Globalization;

namespace OpenQA.Selenium.Remote
{
    /// <summary>
    /// Handles reponses from the browser
    /// </summary>
    public class Response
    {
        private object responseValue;
        private string responseSessionId;

        /// <summary>
        /// Initializes a new instance of the <see cref="Response"/> class
        /// </summary>
        public Response()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Response"/> class
        /// </summary>
        /// <param name="sessionId">Session ID in use</param>
        public Response(SessionId sessionId)
        {
            if (sessionId != null)
            {
                this.responseSessionId = sessionId.ToString();
            }
        }

        private Response(Dictionary<string, object> rawResponse)
        {
        }

        /// <summary>
        /// Gets or sets the value from JSON.
        /// </summary>
        public object Value
        {
            get { return this.responseValue; }
            set { this.responseValue = value; }
        }

        /// <summary>
        /// Gets or sets the session ID.
        /// </summary>
        public string SessionId
        {
            get { return this.responseSessionId; }
            set { this.responseSessionId = value; }
        }
    }
}
