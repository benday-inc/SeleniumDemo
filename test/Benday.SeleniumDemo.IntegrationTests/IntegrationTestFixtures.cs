using Benday.SeleniumDemo.Api;
using Benday.SeleniumDemo.WebUi;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Edge.SeleniumTools;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Linq;

namespace Benday.SeleniumDemo.IntegrationTests
{
    [TestClass]
    public class IntegrationTestFixtures
    {
        [TestInitialize]
        public void OnTestInitialize()
        {
            _systemUnderTest = null;
        }

        [TestCleanup]
        public void OnTestCleanup()
        {
            _systemUnderTest?.Dispose();
        }

        private CustomWebApplicationFactory<Startup> _systemUnderTest;
        public CustomWebApplicationFactory<Startup> SystemUnderTest
        {
            get
            {
                if (_systemUnderTest == null)
                {
                    _systemUnderTest = new CustomWebApplicationFactory<Startup>();
                }

                return _systemUnderTest;
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
            var fullyQualifiedUrl = 
                SystemUnderTest.GetServerAddressForRelativeUrl(url);

            var driverOptions = new EdgeOptions();
            driverOptions.UseChromium = true;

            driverOptions.AddArgument("headless");

            // using var driver = new EdgeDriver(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), driverOptions);
            using var driver = new EdgeDriver(driverOptions);

            // act
            Console.WriteLine($"Navigating to '{fullyQualifiedUrl}...'");
            driver.Navigate().GoToUrl(fullyQualifiedUrl);

            // assert
            AssertDivExistsAndContainsText(expectedText, driver, "divTextThatIsAlwaysThere");            
        }

        [TestMethod]
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task CallHomePage_UsingSeleniumAndWebApplicationFactory_WithTypeReplacements()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            InitializeWithTypeReplacements();

            var expectedMessage = "testing";
            var service = SystemUnderTest.CreateInstance<IAnotherUsefulService>();
            service.ReturnValue = expectedMessage;

            var expectedText = "text that should always be there";

            var client = SystemUnderTest.CreateClient();

            var url = "home/index";
            var fullyQualifiedUrl =
                SystemUnderTest.GetServerAddressForRelativeUrl(url);

            var driverOptions = new EdgeOptions();
            driverOptions.UseChromium = true;

            driverOptions.AddArgument("headless");

            // using var driver = new EdgeDriver(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), driverOptions);
            using var driver = new EdgeDriver(driverOptions);

            // act
            Console.WriteLine($"Navigating to '{fullyQualifiedUrl}...'");
            driver.Navigate().GoToUrl(fullyQualifiedUrl);

            // assert
            AssertDivExistsAndContainsText(expectedText, driver, "divTextThatIsAlwaysThere");
            AssertDivExistsAndContainsText("FAKE VALUE", driver, "divUsefulServiceValue");
            AssertDivExistsAndContainsText(expectedMessage, driver, "divAnotherUsefulServiceValue");
        }        

        private AnotherUsefulService _instance;

        private AnotherUsefulService GetInstanceOfAnotherUsefulService(IServiceProvider arg)
        {
            if (_instance == null)
            {
                _instance = new AnotherUsefulService();
            }

            return _instance;
        }

        private void InitializeWithTypeReplacements()
        {
            _systemUnderTest = new CustomWebApplicationFactory<Startup>(addDevelopmentConfigurations: builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddTransient<IUsefulService, FakeUsefulService>();
                    AssertTypeIsRegistered<IAnotherUsefulService>(services);

                    services.RemoveAll<IAnotherUsefulService>();
                    services.RemoveAll<AnotherUsefulService>();

                    AssertTypeIsNotRegistered<IAnotherUsefulService>(services);

                    services.AddSingleton<IAnotherUsefulService, AnotherUsefulService>(GetInstanceOfAnotherUsefulService);
                });
            });

            Console.WriteLine($"InitializeWithTypeReplacements(): TestServer is null = {(_systemUnderTest.TestServer == null)}");
        }

        private static void AssertTypeIsRegistered<T>(IServiceCollection services)
        {
            var asServiceCollection = services as ServiceCollection;

            if (asServiceCollection != null)
            {
                var match = (from temp in asServiceCollection
                             where temp.ServiceType == typeof(T)
                             select temp).FirstOrDefault();

                Assert.IsNotNull(match, "Type should be registered.");
            }
        }

        private static void AssertTypeIsNotRegistered<T>(IServiceCollection services)
        {
            var asServiceCollection = services as ServiceCollection;

            if (asServiceCollection != null)
            {
                var match = (from temp in asServiceCollection
                             where temp.ServiceType == typeof(T)
                             select temp).FirstOrDefault();

                Assert.IsNull(match, "Type should not be registered.");
            }
        }

        private static void AssertDivExistsAndContainsText(string expectedText, EdgeDriver driver, string id)
        {
            var element = driver.FindElement(By.Id(id));

            Assert.IsNotNull(element, $"element '{id}' should not be null");
            Assert.IsTrue(element.Displayed, $"element '{id}' should be displayed");
            Assert.IsTrue(element.Enabled, $"element '{id}' should be enabled");

            Assert.IsTrue(element.Text.Contains(expectedText), 
                $"element '{id}' should contain expected text. Actual: '{element.Text}'");
        }        
    }
}
