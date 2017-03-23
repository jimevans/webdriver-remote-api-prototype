using OpenQA.Selenium.Remote;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SessionStartupParametersDemo
{
    public class SeleniumGridRemoteSetting : RemoteSetting
    {
        public string InternetExplorerDriverServicePath { get; set; }
        public string GeckoDriverServicePath { get; set; }
        public string ChromeDriverServicePath { get; set; }

        protected override object GenerateSerializableObject()
        {
            Dictionary<string, object> settings = new Dictionary<string, object>();
            if (!string.IsNullOrEmpty(this.InternetExplorerDriverServicePath))
            {
                settings["webdriver.ie.driver"] = this.InternetExplorerDriverServicePath;
            }

            if (!string.IsNullOrEmpty(this.GeckoDriverServicePath))
            {
                settings["webdriver.gecko.driver"] = this.GeckoDriverServicePath;
            }

            if (!string.IsNullOrEmpty(this.ChromeDriverServicePath))
            {
                settings["webdriver.chrome.driver"] = this.ChromeDriverServicePath;
            }

            return settings;
        }
    }
}
