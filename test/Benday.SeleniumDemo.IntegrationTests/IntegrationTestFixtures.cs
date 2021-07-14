using Benday.SeleniumDemo.WebUi;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Edge.SeleniumTools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace Benday.SeleniumDemo.IntegrationTests
{
    [TestClass]
    public class IntegrationTestFixtures
    {
        [TestInitialize]
        public void OnTestInitialize()
        {
            _SystemUnderTest = null;
        }

        private WebApplicationFactory<Startup> _SystemUnderTest;
        public WebApplicationFactory<Startup> SystemUnderTest
        {
            get
            {
                if (_SystemUnderTest == null)
                {
                    _SystemUnderTest = new WebApplicationFactory<Startup>();
                }

                return _SystemUnderTest;
            }
        }

        [TestMethod]
        public async Task CallHomePage_UsingWebApplicationFactory()
        {
            var expectedText = "text that should always be there";

            var client = SystemUnderTest.CreateClient();

            var url = "home/index";

            var response = await client.GetAsync(url);

            Assert.IsTrue(response.IsSuccessStatusCode, "Request should have succeeded");

            var content = await response.Content.ReadAsStringAsync();

            Assert.IsNotNull(content, "content was null");

            Assert.IsTrue(content.Contains(expectedText), "content should contain expected text");
        }
        
        [TestMethod]
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task CallHomePage_UsingSeleniumAndWebApplicationFactory()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {    
            var expectedText = "text that should always be there";

            var client = SystemUnderTest.CreateClient();

            var url = "home/index";
            var fullyQualifiedUrl = GetFullUrl(url);

            var driverOptions = new EdgeOptions();
            driverOptions.UseChromium = true;
            
            // if there is an issue with the run in CI, comment the headless part
            // driverOptions.AddArguments("--headless", "--ignore-certificate-errors");

            // using var driver = new EdgeDriver(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), driverOptions);
            using var driver = new EdgeDriver(driverOptions);

            // act
            driver.Navigate().GoToUrl(fullyQualifiedUrl);

            // assert
            var divThatShouldExistAlways = driver.FindElement(By.Id("divTextThatIsAlwaysThere"));

            Assert.IsNotNull(divThatShouldExistAlways, "div should not be null");
            Assert.IsTrue(divThatShouldExistAlways.Displayed, "div should be displayed");
            Assert.IsTrue(divThatShouldExistAlways.Enabled, "div should be enabled");

            Assert.IsTrue(divThatShouldExistAlways.Text.Contains(expectedText), "div should contain expected text");            
        }

        private string GetFullUrl(string url)
        {
            var baseAddr = SystemUnderTest.Server.BaseAddress;
            var uri = new Uri(baseAddr, url);

            return uri.AbsoluteUri;
        }
    }
}
