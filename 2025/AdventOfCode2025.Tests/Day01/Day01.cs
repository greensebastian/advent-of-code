using Shouldly;

namespace AdventOfCode2025.Tests.Day01;

public class Day01
{
    private const string Example = """
                                   L68
                                   L30
                                   R48
                                   L5
                                   R60
                                   L55
                                   L1
                                   L99
                                   R14
                                   L82
                                   """;
    
    [Fact]
    public void Example_1()
    {
        var lines = Util.ReadRaw(Example);
        var dial = new Dial(lines);
        dial.CountsAt0.ShouldBe(3);
    }
    
    [Fact]
    public void Real_1()
    {
        var lines = Util.ReadFile("day01");
        var dial = new Dial(lines);
        dial.CountsAt0.ShouldBe(1165);
    }
    
    [Fact]
    public void Example_2()
    {
        var lines = Util.ReadRaw(Example);
        var dial = new Dial(lines);
        dial.CountsPassing0.ShouldBe(6);
    }
    
    [Fact]
    public void Real_2()
    {
        var lines = Util.ReadFile("day01");
        var dial = new Dial(lines);
        dial.CountsPassing0.ShouldBe(1165);
    }
}

public class Dial
{
    private int Position { get; set; } = 50;
    public int CountsAt0 { get; private set; } = 0;
    public int CountsPassing0 { get; private set; } = 0;
    public Dial(IReadOnlyList<string> input)
    {
        foreach (var action in input)
        {
            var goingUp = action[0] == 'R';
            var steps = int.Parse(action[1..]);

            for (var i = 0; i < steps; i++)
            {
                Position += goingUp ? 1 : -1;
                Position = (Position + 100) % 100;
                if (Position == 0) CountsPassing0++;
            }

            if (Position == 0) CountsAt0++;
        }
    }
}