using OpenQA.Selenium.Remote;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SessionStartupParametersDemo
{
    public class CloudProviderRemoteSetting : RemoteSetting
    {
        public string AccountUserName { get; set; }
        public string AccountAccessToken { get; set; }
        public string VirtualMachineOperatingSystem { get; set; }

        protected override object GenerateSerializableObject()
        {
            Dictionary<string, object> authenticationSettings = new Dictionary<string, object>();
            if (!string.IsNullOrEmpty(this.AccountUserName))
            {
                authenticationSettings["userName"] = this.AccountUserName;
            }

            if (!string.IsNullOrEmpty(this.AccountAccessToken))
            {
                authenticationSettings["token"] = this.AccountAccessToken;
            }

            if (!string.IsNullOrEmpty(this.VirtualMachineOperatingSystem))
            {
                authenticationSettings["platform"] = this.VirtualMachineOperatingSystem;
            }

            return authenticationSettings;
        }
    }
}
