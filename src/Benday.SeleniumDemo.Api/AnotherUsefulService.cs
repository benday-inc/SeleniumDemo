using System;

namespace Benday.SeleniumDemo.Api
{
    public class AnotherUsefulService : IAnotherUsefulService
    {

        public AnotherUsefulService()
        {
            ReturnValue = "bingbong";
        }

        public string ReturnValue { get; set; }

        public string GetMessage()
        {
            return $"{ReturnValue}";
        }
    }
}
