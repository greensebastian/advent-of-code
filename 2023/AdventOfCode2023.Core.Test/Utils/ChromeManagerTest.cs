using Shouldly;

namespace AdventOfCode2023.Core.Test.Utils;

public class ChromeManagerTest
{
    [Fact]
    public void GetCookies_Works()
    {
        var hostname = "adventofcode.com";
        var cookies = ChromeManager.GetCookies(hostname);
        cookies.ShouldNotBeEmpty();
    }
}