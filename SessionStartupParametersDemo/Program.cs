using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Remote;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SessionStartupParametersDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            CreateDefaultBrowserSpecificDriver();
            CreateBrowserSpecificDriverWithOptions();
            CreateBrowserSpecificDriverWithExperimentalOptions();
            SimpleRemoteCase();
            ComplexRemoteCase();
            MergeFailureCase();
            MixedRequiredAndDesiredRemoteCase();
            CloudProviderExample();
            Console.WriteLine("Press <Enter> to exit");
            Console.ReadLine();
        }

        private static void CreateDefaultBrowserSpecificDriver()
        {
            Console.WriteLine("Creating default IE driver");
            IWebDriver driver = new InternetExplorerDriver();
        }

        private static void CreateBrowserSpecificDriverWithOptions()
        {
            Console.WriteLine("Creating IE driver with specific options");
            InternetExplorerOptions options = new InternetExplorerOptions();
            options.PageLoadStrategy = PageLoadStrategy.Eager;
            options.EnsureCleanSession = true;
            options.RequireWindowFocus = true;
            IWebDriver driver = new InternetExplorerDriver(options);
        }

        private static void CreateBrowserSpecificDriverWithExperimentalOptions()
        {
            Console.WriteLine("Creating IE driver with specific options and experimental, new capability");
            InternetExplorerOptions options = new InternetExplorerOptions();
            options.PageLoadStrategy = PageLoadStrategy.Eager;
            options.EnsureCleanSession = true;
            options.RequireWindowFocus = true;
            options.AddAdditionalCapability("myNewBrowserSpecificCapability", "hello world");
            IWebDriver driver = new InternetExplorerDriver(options);
        }

        private static void SimpleRemoteCase()
        {
            Console.WriteLine("Creating a simple remote instance for a single browser capability set");
            ChromeOptions options = new ChromeOptions();
            options.AddArgument("--ignore-browser-security");
            IWebDriver driver = new RemoteWebDriver(new Uri("http://path.to.remote.server:4444/wd/hub"), options);
        }

        private static void ComplexRemoteCase()
        {
            Console.WriteLine("Creating a complex remote instance for matching one of several browser capability sets");

            InternetExplorerOptions ieOptions = new InternetExplorerOptions();
            ieOptions.EnsureCleanSession = true;

            FirefoxOptions firefoxOptions = new FirefoxOptions();
            firefoxOptions.BrowserVersion = "52";
            firefoxOptions.SetPreference("my.firefox.preference", 1);

            ChromeOptions chromeOptions = new ChromeOptions();
            chromeOptions.AddArgument("--ignore-browser-security");

            RemoteSessionOptions settings = new RemoteSessionOptions();
            settings.AddFirstMatchDriverOption(ieOptions);
            settings.AddFirstMatchDriverOption(firefoxOptions);
            settings.AddFirstMatchDriverOption(chromeOptions);
            IWebDriver driver = new RemoteWebDriver(new Uri("http://path.to.remote.server:4444/wd/hub"), settings);
        }

        private static void MergeFailureCase()
        {
            Console.WriteLine("Try creating incompatible remote settings.");
            InternetExplorerOptions ieOptions = new InternetExplorerOptions();
            ieOptions.EnsureCleanSession = true;

            FirefoxOptions firefoxOptions = new FirefoxOptions();
            firefoxOptions.BrowserVersion = "52";
            firefoxOptions.SetPreference("my.firefox.preference", 1);
            RemoteSessionOptions settings = new RemoteSessionOptions(ieOptions);

            try
            {
                settings.AddFirstMatchDriverOption(firefoxOptions);
            }
            catch (ArgumentException e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private static void MixedRequiredAndDesiredRemoteCase()
        {
            Console.WriteLine("Creating a complex remote instance for matching some required and the first of several desired browser capability sets");

            Proxy proxy = new Proxy();
            proxy.HttpProxy = "http://proxylocation:8080";

            DriverOptions requiredOptions = new DriverOptions();
            requiredOptions.PlatformName = "windows";
            requiredOptions.Proxy = proxy;

            InternetExplorerOptions ieOptions = new InternetExplorerOptions();
            ieOptions.RequireWindowFocus = true;
            ieOptions.AddAdditionalCapability("experimentalTopLevel", "capability value", true);
            ieOptions.PlatformName = null; // clear PlatformName capability so it can be merged.

            FirefoxOptions firefoxOptions = new FirefoxOptions();
            firefoxOptions.BrowserVersion = "52";
            firefoxOptions.AcceptInsecureCertificates = true;
            firefoxOptions.SetPreference("my.firefox.preference", "hello there");

            ChromeOptions chromeOptions = new ChromeOptions();
            chromeOptions.BinaryLocation = @"C:\path\to\custom\chrome.exe";

            RemoteSessionOptions settings = new RemoteSessionOptions(requiredOptions);
            settings.AddFirstMatchDriverOption(chromeOptions);
            settings.AddFirstMatchDriverOption(firefoxOptions);
            settings.AddFirstMatchDriverOption(ieOptions);
            IWebDriver driver = new RemoteWebDriver(new Uri("http://path.to.remote.server:4444/wd/hub"), settings);
        }

        private static void CloudProviderExample()
        {
            Console.WriteLine("Sample showing how to construct a remote setting using a fictional cloud provider");
            FirefoxOptions firefoxOptions = new FirefoxOptions();
            firefoxOptions.PageLoadStrategy = PageLoadStrategy.None;

            // Simulate using a cloud provider. Cloud providers should publish
            // their own classes that extend RemoteSetting for .NET users.
            CloudProviderRemoteSetting cloudSettings = new CloudProviderRemoteSetting();
            
            // Authentication to cloud provider account
            cloudSettings.AccountUserName = "MyUserName";
            cloudSettings.AccountAccessToken = "a1430ef01937dc20371f";

            // Different from WebDriver's "platformName" because the cloud
            // provider may offer more finely grained OS info on the VM spun up
            // including specific versions of OS.
            cloudSettings.VirtualMachineOperatingSystem = "Windows 7";

            RemoteSessionOptions sessionSettings = new RemoteSessionOptions();
            sessionSettings.AddFirstMatchDriverOption(firefoxOptions);

            // This name is an example only. A reputable cloud provider would
            // name this setting something meaningful.
            sessionSettings.AddRemoteSetting("cloud:myCloudSettings", cloudSettings);
            IWebDriver driver = new RemoteWebDriver(new Uri("http://example.cloudprovider.com:4444/wd/hub"), sessionSettings);
        }
    }
}
