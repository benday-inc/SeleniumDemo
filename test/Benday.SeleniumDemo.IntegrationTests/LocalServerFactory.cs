using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using System;
using System.Linq;

namespace Benday.SeleniumDemo.IntegrationTests
{
    public class LocalServerFactory<TStartup> : WebApplicationFactory<TStartup>
    where TStartup : class
    {
        private readonly string _baseAddress = "https://localhost";
        private IWebHost _webHost;

        public LocalServerFactory()
        {
            ClientOptions.BaseAddress = new Uri(_baseAddress);
            
            CreateServer(CreateWebHostBuilder());
        }
        public string RootUri { get; private set; }
        protected override TestServer CreateServer(IWebHostBuilder builder)
        {
            _webHost = builder.Build();
            _webHost.Start();
            RootUri = _webHost.ServerFeatures.Get<IServerAddressesFeature>().Addresses.LastOrDefault();
            // not used but needed in the CreateServer method logic
            return new TestServer(new WebHostBuilder().UseStartup<TStartup>());
        }
        protected override IWebHostBuilder CreateWebHostBuilder()
        {
            var builder = WebHost.CreateDefaultBuilder(Array.Empty<string>());
            builder.UseStartup<TStartup>();
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
