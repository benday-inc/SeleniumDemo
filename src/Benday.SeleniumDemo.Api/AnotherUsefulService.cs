using System;

namespace Benday.SeleniumDemo.Api
{
    public class AnotherUsefulService : IAnotherUsefulService
    {

        public AnotherUsefulService()
        {
            Prefix = "bingbong";
        }

        public string Prefix { get; set; }

        public string GetMessage()
        {
            return $"{Prefix} - {DateTime.Now.ToString()}";
        }
    }
}
