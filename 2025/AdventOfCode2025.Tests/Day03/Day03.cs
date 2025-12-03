using Shouldly;

namespace AdventOfCode2025.Tests.Day03;

public class Day03
{
    private const string Example = """
                                   987654321111111
                                   811111111111119
                                   234234234234278
                                   818181911112111
                                   """;
    
    [Fact]
    public void Example_1()
    {
        var lines = Util.ReadRaw(Example);
        var ranges = new BatteryBanks(lines);
        ranges.HighestVoltageSum().ShouldBe(357);
    }
    
    [Fact]
    public void Real_1()
    {
        var lines = Util.ReadFile("day03");
        var ranges = new BatteryBanks(lines);
        ranges.HighestVoltageSum().ShouldBe(17263);
    }
    
    [Fact]
    public void Example_2()
    {
        var lines = Util.ReadRaw(Example);
        var ranges = new BatteryBanks(lines);
        ranges.Highest12DigitVoltageSum().ShouldBe(3121910778619);
    }
    
    [Fact]
    public void Real_2()
    {
        var lines = Util.ReadFile("day03");
        var ranges = new BatteryBanks(lines);
        ranges.Highest12DigitVoltageSum().ShouldBe(170731717900423L);
    }
}

public class BatteryBanks(IReadOnlyList<string> input)
{
    public int HighestVoltageSum()
    {
        var sum = 0;
        foreach (var bank in input)
        {
            var highest = 0;
            for (var i = 0; i < bank.Length - 1; i++)
            {
                var left = bank[i] - '0';
                var right = bank[(i + 1)..].Max(c => c - '0');
                var tot = left * 10 + right;
                highest = Math.Max(highest, tot);
            }

            sum += highest;
        }

        return sum;
    }
    
    public long Highest12DigitVoltageSum()
    {
        var sum = 0L;
        foreach (var bankText in input)
        {
            var bankVoltage = 0L;
            var bank = bankText.Select(c => c - '0').ToArray();

            var offset = 0;
            for (var i = 0; i < 12; i++)
            {
                var remainingBank = bank.Skip(offset).SkipLast(12 - i - 1).ToArray();
                var val = remainingBank.Max();
                offset = remainingBank.IndexOf(val) + offset + 1;
                bankVoltage *= 10;
                bankVoltage += val;
            }

            sum += bankVoltage;
        }

        return sum;
    }
}