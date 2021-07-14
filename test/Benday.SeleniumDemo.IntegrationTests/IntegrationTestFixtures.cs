using Benday.SeleniumDemo.WebUi;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Benday.SeleniumDemo.IntegrationTests
{
    [TestClass]
    public class IntegrationTestFixtures
    {
        [TestMethod]
        public async Task CallHomePage_UsingWebApplicationFactory()
        {
            var factory = new WebApplicationFactory<Startup>();

            var expectedText = "text that should always be there";

            var client = factory.CreateClient();

            var url = "home/index";

            var response = await client.GetAsync(url);

            Assert.IsTrue(response.IsSuccessStatusCode, "Request should have succeeded");

            var content = await response.Content.ReadAsStringAsync();

            Assert.IsNotNull(content, "content was null");

            Assert.IsTrue(content.Contains(expectedText), "content should contain expected text");
        }
    }
}
