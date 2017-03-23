﻿// <copyright file="HttpCommandExecutor.cs" company="WebDriver Committers">
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
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;

namespace OpenQA.Selenium.Remote
{
    /// <summary>
    /// Provides a way of executing Commands over HTTP
    /// </summary>
    internal class HttpCommandExecutor : ICommandExecutor
    {
        private const string JsonMimeType = "application/json";
        private const string ContentTypeHeader = JsonMimeType + ";charset=utf-8";
        private const string RequestAcceptHeader = JsonMimeType + ", image/png";
        private Uri remoteServerUri;
        private TimeSpan serverResponseTimeout;
        private bool enableKeepAlive;
        private CommandInfoRepository commandInfoRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpCommandExecutor"/> class
        /// </summary>
        /// <param name="addressOfRemoteServer">Address of the WebDriver Server</param>
        /// <param name="timeout">The timeout within which the server must respond.</param>
        public HttpCommandExecutor(Uri addressOfRemoteServer, TimeSpan timeout)
            : this(addressOfRemoteServer, timeout, true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpCommandExecutor"/> class
        /// </summary>
        /// <param name="addressOfRemoteServer">Address of the WebDriver Server</param>
        /// <param name="timeout">The timeout within which the server must respond.</param>
        /// <param name="enableKeepAlive"><see langword="true"/> if the KeepAlive header should be sent
        /// with HTTP requests; otherwise, <see langword="false"/>.</param>
        public HttpCommandExecutor(Uri addressOfRemoteServer, TimeSpan timeout, bool enableKeepAlive)
        {
            if (addressOfRemoteServer == null)
            {
                throw new ArgumentNullException("addressOfRemoteServer", "You must specify a remote address to connect to");
            }

            if (!addressOfRemoteServer.AbsoluteUri.EndsWith("/", StringComparison.OrdinalIgnoreCase))
            {
                addressOfRemoteServer = new Uri(addressOfRemoteServer.ToString() + "/");
            }

            this.remoteServerUri = addressOfRemoteServer;
            this.serverResponseTimeout = timeout;
            this.enableKeepAlive = enableKeepAlive;

            ServicePointManager.Expect100Continue = false;

            // In the .NET Framework, HttpWebRequest responses with an error code are limited
            // to 64k by default. Since the remote server error responses include a screenshot,
            // they can frequently exceed this size. This only applies to the .NET Framework;
            // Mono does not implement the property.
            if (Type.GetType("Mono.Runtime", false, true) == null)
            {
                HttpWebRequest.DefaultMaximumErrorResponseLength = -1;
            }
        }

        /// <summary>
        /// Gets the repository of objects containin information about commands.
        /// </summary>
        public CommandInfoRepository CommandInfoRepository
        {
            get { return this.commandInfoRepository; }
        }

        /// <summary>
        /// Executes a command
        /// </summary>
        /// <param name="commandToExecute">The command you wish to execute</param>
        /// <returns>A response from the browser</returns>
        public virtual Response Execute(Command commandToExecute)
        {
            if (commandToExecute == null)
            {
                throw new ArgumentNullException("commandToExecute", "commandToExecute cannot be null");
            }

            CommandInfo info = this.commandInfoRepository.GetCommandInfo(commandToExecute.Name);
            HttpWebRequest request = info.CreateWebRequest(this.remoteServerUri, commandToExecute);
            request.Timeout = (int)this.serverResponseTimeout.TotalMilliseconds;
            request.Accept = RequestAcceptHeader;
            request.KeepAlive = this.enableKeepAlive;
            request.ServicePoint.ConnectionLimit = 2000;
            if (request.Method == CommandInfo.PostCommand)
            {
                string payload = commandToExecute.ParametersAsJsonString;
                byte[] data = Encoding.UTF8.GetBytes(payload);
                request.ContentType = ContentTypeHeader;
                Stream requestStream = request.GetRequestStream();
                requestStream.Write(data, 0, data.Length);
                requestStream.Close();
            }

            Response toReturn = this.CreateResponse(request);
            return toReturn;
        }

        private static string GetTextOfWebResponse(HttpWebResponse webResponse)
        {
            // StreamReader.Close also closes the underlying stream.
            Stream responseStream = webResponse.GetResponseStream();
            StreamReader responseStreamReader = new StreamReader(responseStream, Encoding.UTF8);
            string responseString = responseStreamReader.ReadToEnd();
            responseStreamReader.Close();

            // The response string from the Java remote server has trailing null
            // characters. This is due to the fix for issue 288.
            if (responseString.IndexOf('\0') >= 0)
            {
                responseString = responseString.Substring(0, responseString.IndexOf('\0'));
            }

            return responseString;
        }

        private Response CreateResponse(WebRequest request)
        {
            Response commandResponse = new Response();
            return commandResponse;
        }
    }
}
