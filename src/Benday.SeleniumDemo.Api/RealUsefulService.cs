using System;

namespace Benday.SeleniumDemo.Api
{
    public class RealUsefulService : IUsefulService
    {
        public RealUsefulService()
        {
            var guid = Guid.NewGuid();
            UsefulValue = $"{guid}:(not set)";
        }
        public string UsefulValue { get; set; }
    }
}
