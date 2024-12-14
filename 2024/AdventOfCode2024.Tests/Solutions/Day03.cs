using System.Text.RegularExpressions;
using FluentAssertions;

namespace AdventOfCode2024.Tests.Solutions;

public class Day03 : ISolution
{
    [Fact]
    public void Solution1()
    {
        //var input = Util.ReadRaw("xmul(2,4)%&mul[3,7]!@^do_not_mul(5,5)+mul(32,64]then(mul(11,8)mul(8,5))");
        var input = string.Join('\n', Util.ReadFile("day03"));
        
        var regex = new Regex("mul\\((\\d{1,3}),(\\d{1,3})\\)");

        var sum = regex.Matches(input)
            .Select(match => long.Parse(match.Groups[1].Value) * long.Parse(match.Groups[2].Value)).Sum();

        sum.Should().Be(162813399L);
        
        // 28434750 too low
    }


    [Fact]
    public void Solution2()
    {
        //var input = Util.ReadRaw("xmul(2,4)%&mul[3,7]!@^do_not_mul(5,5)+mul(32,64]then(mul(11,8)mul(8,5))");
        var input = string.Join('\n', Util.ReadFile("day03"));
        
        var regex = new Regex("(mul\\((\\d{1,3}),(\\d{1,3})\\))|do\\(\\)|don't\\(\\)");

        var sum = 0L;
        var enabled = true;
        foreach (Match match in regex.Matches(input))
        {
            if (match.Value == "do()") enabled = true;
            if (match.Value == "don't()") enabled = false;
            if (enabled && match.Value.Contains("mul")) sum += long.Parse(match.Groups[2].Value) * long.Parse(match.Groups[3].Value);
        }
        sum.Should().Be(53783319L);
    }
}