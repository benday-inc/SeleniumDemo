using System;

namespace Benday.SeleniumDemo.Api
{
    public interface IAnotherUsefulService
    {
        string Prefix { get; set; }
        string GetMessage();
    }
}
