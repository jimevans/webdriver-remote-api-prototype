﻿// <copyright file="FirefoxExtension.cs" company="WebDriver Committers">
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
using System.IO.Compression;
using System.Xml;

namespace OpenQA.Selenium.Firefox
{
    /// <summary>
    /// Provides the ability to install extensions into a <see cref="FirefoxProfile"/>.
    /// </summary>
    public class FirefoxExtension
    {
        private const string EmNamespaceUri = "http://www.mozilla.org/2004/em-rdf#";

        private string extensionFileName;
        private string extensionResourceId;

        /// <summary>
        /// Initializes a new instance of the <see cref="FirefoxExtension"/> class.
        /// </summary>
        /// <param name="fileName">The name of the file containing the Firefox extension.</param>
        /// <remarks>WebDriver attempts to resolve the <paramref name="fileName"/> parameter
        /// by looking first for the specified file in the directory of the calling assembly,
        /// then using the full path to the file, if a full path is provided.</remarks>
        public FirefoxExtension(string fileName)
            : this(fileName, string.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FirefoxExtension"/> class.
        /// </summary>
        /// <param name="fileName">The name of the file containing the Firefox extension.</param>
        /// <param name="resourceId">The ID of the resource within the assembly containing the extension
        /// if the file is not present in the file system.</param>
        /// <remarks>WebDriver attempts to resolve the <paramref name="fileName"/> parameter
        /// by looking first for the specified file in the directory of the calling assembly,
        /// then using the full path to the file, if a full path is provided. If the file is
        /// not found in the file system, WebDriver attempts to locate a resource in the
        /// executing assembly with the name specified by the <paramref name="resourceId"/>
        /// parameter.</remarks>
        internal FirefoxExtension(string fileName, string resourceId)
        {
            this.extensionFileName = fileName;
            this.extensionResourceId = resourceId;
        }

        /// <summary>
        /// Installs the extension into a profile directory.
        /// </summary>
        /// <param name="profileDirectory">The Firefox profile directory into which to install the extension.</param>
        public void Install(string profileDirectory)
        {
        }

        private static string ReadIdFromInstallRdf(string root)
        {
            string id = null;
            try
            {
                string installRdf = Path.Combine(root, "install.rdf");
                XmlDocument rdfXmlDocument = new XmlDocument();
                rdfXmlDocument.Load(installRdf);

                XmlNamespaceManager rdfNamespaceManager = new XmlNamespaceManager(rdfXmlDocument.NameTable);
                rdfNamespaceManager.AddNamespace("em", EmNamespaceUri);
                rdfNamespaceManager.AddNamespace("RDF", "http://www.w3.org/1999/02/22-rdf-syntax-ns#");

                XmlNode node = rdfXmlDocument.SelectSingleNode("//em:id", rdfNamespaceManager);
                if (node == null)
                {
                    XmlNode descriptionNode = rdfXmlDocument.SelectSingleNode("//RDF:Description", rdfNamespaceManager);
                    XmlAttribute attribute = descriptionNode.Attributes["id", EmNamespaceUri];
                    if (attribute == null)
                    {
                        throw new WebDriverException("Cannot locate node containing extension id: " + installRdf);
                    }

                    id = attribute.Value;
                }
                else
                {
                    id = node.InnerText;
                }

                if (string.IsNullOrEmpty(id))
                {
                    throw new FileNotFoundException("Cannot install extension with ID: " + id);
                }
            }
            catch (Exception e)
            {
                throw new WebDriverException("Error installing extension", e);
            }

            return id;
        }
    }
}
