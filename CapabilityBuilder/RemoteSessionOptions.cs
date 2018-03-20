using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenQA.Selenium.Remote
{
    public class RemoteSessionOptions
    {
        protected static readonly TimeSpan DefaultCommandTimeout = TimeSpan.FromSeconds(60);
        private static readonly string DefaultRemoteUri = "http://127.0.0.1:4444/wd/hub";

        // This field is only used for demo purposes. It would not exist in
        // production code. It is only used to filter out the legacy capabilities
        // when printing out the information going over the wire.
        private bool includeLegacyCapabilities = true;

        private Uri remoteUri = new Uri(DefaultRemoteUri);
        private TimeSpan commandTimeout = DefaultCommandTimeout;

        private DriverOptions mustMatchDriverOptions;
        private List<DriverOptions> firstMatchOptions = new List<DriverOptions>();
        private Dictionary<string, RemoteSetting> additionalRemoteSettings = new Dictionary<string, RemoteSetting>();

        public RemoteSessionOptions()
        {
        }

        public RemoteSessionOptions(DriverOptions mustMatchDriverOptions, params DriverOptions[] firstMatchDriverOptions)
        {
            this.mustMatchDriverOptions = mustMatchDriverOptions;
            foreach (DriverOptions firstMatchOption in firstMatchDriverOptions)
            {
                this.AddFirstMatchDriverOption(firstMatchOption);
            }
        }

        public bool IncludeLegacyCapabilities
        {
            get { return this.includeLegacyCapabilities; }
            set { this.includeLegacyCapabilities = value; }
        }

        public void AddFirstMatchDriverOption(DriverOptions options)
        {
            if (mustMatchDriverOptions != null)
            {
                DriverOptionsMergeResult mergeResult = mustMatchDriverOptions.TryMerge(options);
                if (mergeResult.IsMergeConflict)
                {
                    string msg = string.Format(CultureInfo.InvariantCulture, "You cannot request the same capability in both must-match and first-match capabilities. You are attempting to add a first-match driver options object that defines a capability, '{0}', that is already defined in the must-match driver options.", mergeResult.MergeConflictOptionName);
                    throw new ArgumentException(msg, "options");
                }
            }

            firstMatchOptions.Add(options);
        }

        public void SetMustMatchDriverOptions(DriverOptions options)
        {
            if (this.firstMatchOptions.Count > 0)
            {
                int driverOptionIndex = 0;
                foreach (DriverOptions firstMatchOption in this.firstMatchOptions)
                {
                    DriverOptionsMergeResult mergeResult = firstMatchOption.TryMerge(options);
                    if (mergeResult.IsMergeConflict)
                    {
                        string msg = string.Format(CultureInfo.InvariantCulture, "You cannot request the same capability in both must-match and first-match capabilities. You are attempting to add a must-match driver options object that defines a capability, '{0}', that is already defined in the first-match driver options with index {1}.", mergeResult.MergeConflictOptionName, driverOptionIndex);
                        throw new ArgumentException(msg, "options");
                    }

                    driverOptionIndex++;
                }
            }

            this.mustMatchDriverOptions = options;
        }

        public void AddRemoteSetting(string settingName, RemoteSetting setting)
        {
            if (string.IsNullOrEmpty(settingName))
            {
                throw new ArgumentNullException("settingName", "Setting name must not be null or empty.");
            }

            if (setting == null)
            {
                throw new ArgumentNullException("setting", "Setting must not be null.");
            }

            if (!setting.IsValid)
            {
                throw new ArgumentException("Remote setting cannot be serialized to JSON payload.", "setting");
            }

            additionalRemoteSettings[settingName] = setting;
        }

        public Dictionary<string, object> ToDictionary()
        {
            Dictionary<string, object> payloadDictionary = new Dictionary<string, object>();
            Dictionary<string, object> capabilitiesDictionary = new Dictionary<string, object>();

            if (this.mustMatchDriverOptions != null)
            {
                DesiredCapabilities requiredCapabilities = this.mustMatchDriverOptions.ToCapabilities() as DesiredCapabilities;
                if (this.includeLegacyCapabilities)
                {
                    if (this.firstMatchOptions.Count == 0)
                    {
                        payloadDictionary["desiredCapabilities"] = requiredCapabilities.CapabilitiesDictionary;
                    }
                }

                capabilitiesDictionary["alwaysMatch"] = requiredCapabilities.CapabilitiesDictionary;
            }

            if (this.firstMatchOptions.Count > 0)
            {
                List<object> optionsMatches = new List<object>();
                foreach (DriverOptions options in this.firstMatchOptions)
                {
                    DesiredCapabilities capabilities = options.ToCapabilities() as DesiredCapabilities;
                    optionsMatches.Add(capabilities.CapabilitiesDictionary);
                }

                if (this.includeLegacyCapabilities)
                {
                    DesiredCapabilities desiredCapabilities = this.firstMatchOptions[0].ToCapabilities() as DesiredCapabilities;
                    payloadDictionary["desiredCapabilities"] = desiredCapabilities.CapabilitiesDictionary;
                }

                capabilitiesDictionary["firstMatch"] = optionsMatches;
            }

            foreach (KeyValuePair<string, RemoteSetting> additionalSetting in this.additionalRemoteSettings)
            {
                payloadDictionary[additionalSetting.Key] = additionalSetting.Value.SerializableObject;
            }

            payloadDictionary["capabilities"] = capabilitiesDictionary;

            return payloadDictionary;
        }
    }
}
