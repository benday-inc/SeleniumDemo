using System;

namespace Benday.SeleniumDemo.Api
{
    public interface IAnotherUsefulService
    {
        string ReturnValue { get; set; }
        string GetMessage();
    }
}
