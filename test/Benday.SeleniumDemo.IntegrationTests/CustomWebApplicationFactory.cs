using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using System;
using System.Linq;

namespace Benday.SeleniumDemo.IntegrationTests
{
    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup>
    where TStartup : class
    {
        private readonly string _baseAddress = "https://localhost";
        private IWebHost _webHost;
        private Action<IWebHostBuilder> _addDevelopmentConfigs;

        public CustomWebApplicationFactory(Action<IWebHostBuilder> addDevelopmentConfigurations = null)
        {
            Console.WriteLine($"CustomWebApplicationFactory.ctor starting...");

            if (addDevelopmentConfigurations != null)
            {
                _addDevelopmentConfigs = addDevelopmentConfigurations;
            }

            ClientOptions.BaseAddress = new Uri(_baseAddress);

            Console.WriteLine($"CustomWebApplicationFactory.ctor calling CreateServer()...");

            CreateServer(CreateWebHostBuilder());

            Console.WriteLine($"CustomWebApplicationFactory.ctor exiting...");
        }

        public string GetServerAddress()
        {
            var serverAddresses = _webHost.ServerFeatures.Get<IServerAddressesFeature>();

            if (serverAddresses == null)
            {
                throw new InvalidOperationException($"Could not get instance of IServerAddressFeature.");
            }

            var addresses = serverAddresses.Addresses;

            var returnValue = addresses.FirstOrDefault();

            return returnValue;
        }

        public string GetServerAddressForRelativeUrl(string url)
        {
            var baseAddr = GetServerAddress();

            return $"{baseAddr}/{url}";
        }

        public TestServer TestServer { get; private set; }

        protected override TestServer CreateServer(IWebHostBuilder builder)
        {
            Console.WriteLine($"CustomWebApplicationFactory.CreateServer() starting...");

            Console.WriteLine($"CustomWebApplicationFactory.CreateServer() calling builder.Build()...");
            _webHost = builder.Build();

            Console.WriteLine($"CustomWebApplicationFactory.CreateServer() calling webhost.start()...");
            _webHost.Start();

            TestServer returnValue = InitializeTestServer();

            Console.WriteLine($"CustomWebApplicationFactory.CreateServer() exiting...");

            return returnValue;
        }

        private TestServer InitializeTestServer()
        {
            Console.WriteLine($"CustomWebApplicationFactory.InitializeTestServer() creating instance of test server...");

            var builder = new WebHostBuilder().UseStartup<TStartup>();

            Console.WriteLine($"CustomWebApplicationFactory.InitializeTestServer() invoking add dev config...");
            _addDevelopmentConfigs?.Invoke(builder);
            Console.WriteLine($"CustomWebApplicationFactory.InitializeTestServer() invoked add dev config...");

            var returnValue = new TestServer(builder);
            // var returnValue = new TestServer(builder.UseStartup<TStartup>());

            TestServer = returnValue;
            return returnValue;
        }

        protected override IWebHostBuilder CreateWebHostBuilder()
        {
            Console.WriteLine($"CustomWebApplicationFactory.CreateWebHostBuilder() starting...");

            var builder = WebHost.CreateDefaultBuilder(Array.Empty<string>());
            builder.UseStartup<TStartup>();

            Console.WriteLine($"CustomWebApplicationFactory.CreateWebHostBuilder() invoking add dev config...");
            _addDevelopmentConfigs?.Invoke(builder);
            Console.WriteLine($"CustomWebApplicationFactory.CreateWebHostBuilder() invoked add dev config...");

            Console.WriteLine($"CustomWebApplicationFactory.CreateWebHostBuilder() starting...");

            return builder;
        }        

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                _webHost?.Dispose();
            }
        }
    }
}
