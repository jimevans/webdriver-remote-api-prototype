using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenQA.Selenium.Remote
{
    public class RemoteSessionSettings
    {
        private bool includeLegacyCapabilities = true;
        private DriverOptions requiredOptions;
        private List<DriverOptions> desiredOptions = new List<DriverOptions>();
        private Dictionary<string, RemoteSetting> additionalRemoteSettings = new Dictionary<string, RemoteSetting>();

        public RemoteSessionSettings()
        {
        }

        public RemoteSessionSettings(DriverOptions requiredDriverOptions)
        {
            this.requiredOptions = requiredDriverOptions;
        }

        public bool IncludeLegacyCapabilities
        {
            get { return this.includeLegacyCapabilities; }
            set { this.includeLegacyCapabilities = value; }
        }

        public void AddDriverOptions(DriverOptions options)
        {
            if (requiredOptions != null)
            {
                DriverOptionsMergeResult mergeResult = requiredOptions.TryMerge(options);
                if (mergeResult.IsMergeConflict)
                {
                    string msg = string.Format(CultureInfo.InvariantCulture, "You cannot request the same capability in both required and desired capabilities. You are attempting to add a desired driver options object that defines a capability, '{0}', that is already defined in the required driver options.", mergeResult.MergeConflictOptionName);
                    throw new ArgumentException(msg, "options");
                }
            }

            desiredOptions.Add(options);
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

        public Dictionary<string, object> ToPayload()
        {
            Dictionary<string, object> payload = new Dictionary<string, object>();
            Dictionary<string, object> capabilitiesDictionary = new Dictionary<string, object>();

            if (this.requiredOptions != null)
            {
                DesiredCapabilities requiredCapabilities = this.requiredOptions.ToCapabilities() as DesiredCapabilities;
                if (this.includeLegacyCapabilities)
                {
                    payload["requiredCapabilities"] = requiredCapabilities.CapabilitiesDictionary;
                    if (this.desiredOptions.Count == 0)
                    {
                        payload["desiredCapabilities"] = requiredCapabilities.CapabilitiesDictionary;
                    }
                }

                capabilitiesDictionary["alwaysMatch"] = requiredCapabilities.CapabilitiesDictionary;
            }

            if (this.desiredOptions.Count > 0)
            {
                List<object> optionsMatches = new List<object>();
                foreach (DriverOptions options in this.desiredOptions)
                {
                    DesiredCapabilities capabilities = options.ToCapabilities() as DesiredCapabilities;
                    optionsMatches.Add(capabilities.CapabilitiesDictionary);
                }

                if (this.includeLegacyCapabilities)
                {
                    DesiredCapabilities desiredCapabilities = this.desiredOptions[0].ToCapabilities() as DesiredCapabilities;
                    payload["desiredCapabilities"] = desiredCapabilities.CapabilitiesDictionary;
                }

                capabilitiesDictionary["firstMatch"] = optionsMatches;
            }

            foreach (KeyValuePair<string, RemoteSetting> additionalSetting in this.additionalRemoteSettings)
            {
                payload[additionalSetting.Key] = additionalSetting.Value.SerializableObject;
            }

            payload["capabilities"] = capabilitiesDictionary;

            return payload;
        }
    }
}
