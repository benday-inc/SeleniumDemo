using Benday.SeleniumDemo.Api;
using System;

namespace Benday.SeleniumDemo.IntegrationTests
{
    public class FakeUsefulService : IUsefulService
    {

        public FakeUsefulService()
        {
            UsefulValue = $"FAKE VALUE";
        }

        public string UsefulValue { get; set; }
    }
}
