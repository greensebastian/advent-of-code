using FluentAssertions;

namespace AdventOfCode2024.Tests.Solutions;

public class Day14 : ISolution
{
    private const string Example = """
                                   p=0,4 v=3,-3
                                   p=6,3 v=-1,-3
                                   p=10,3 v=-1,2
                                   p=2,0 v=2,-1
                                   p=0,0 v=1,3
                                   p=3,0 v=-2,-2
                                   p=7,6 v=-1,-3
                                   p=3,0 v=-1,-2
                                   p=9,3 v=2,3
                                   p=7,3 v=-1,2
                                   p=2,4 v=2,-3
                                   p=9,5 v=-3,-3
                                   """;
    
    [Fact]
    public void Solution1()
    {
        //var input = Util.ReadRaw(Example);
        //var input = Util.ReadFile("day13");

        //var sum = SafetyFactor(Util.ReadRaw(Example), new Point(7, 11), 100);
        var sum = SafetyFactor(Util.ReadFile("day14"), new Point(103, 101), 100);
        sum.Should().Be(230436441L);
    }

    [Fact]
    public void Solution2()
    {
        //var input = Util.ReadRaw(Example);
        //var input = Util.ReadFile("day13");

        var sum = SafetyFactor(Util.ReadRaw(Example), new Point(7, 11), 100);
        //var sum = SafetyFactor(Util.ReadFile("day14"), new Point(103, 101), 100);
        sum.Should().Be(12);
    }

    private long SafetyFactor(string[] input, Point bounds, long endTime)
    {
        var robots = input.Select(RobotPosition.FromInput).ToList();
        var room = new RobotRoom(robots, bounds);
        for (var i = 0; i < endTime; i++)
        {
            room.MoveAllRobots();
        }
        return room.SafetyFactor();
    }

    private class RobotRoom(List<RobotPosition> robots, Point bounds)
    {
        public long SecondsPassed { get; private set; } = 0;
        public void MoveAllRobots()
        {
            for (var i = 0; i < robots.Count; i++)
            {
                var bot = robots[i].Move();
                
                bot = bot with
                {
                    Position = new Point((bot.Position.Row + bounds.Row) % bounds.Row, (bot.Position.Col + bounds.Col) % bounds.Col)
                };
                
                robots[i] = bot;
            }

            SecondsPassed++;
        }

        public long SafetyFactor()
        {
            var quadrantSums = new long[] { 0, 0, 0, 0 };
            var mid = new Point(bounds.Row / 2, bounds.Col / 2);
            foreach (var robot in robots)
            {
                if (robot.Position.Row == mid.Row || robot.Position.Col == mid.Col) continue;
                var quadrantIndex = (robot.Position.Row < mid.Row, robot.Position.Col < mid.Col) switch
                {
                    (true, true) => 0,
                    (true, false) => 1,
                    (false, true) => 2,
                    (false, false) => 3
                };
                quadrantSums[quadrantIndex]++;
            }

            return quadrantSums.Aggregate(1L, (product, current) => product * current);
        }
    }

    private record RobotPosition(Point Position, Point Velocity)
    {
        public static RobotPosition FromInput(string input)
        {
            var numbers = input.PlusMinusLongs();
            return new RobotPosition(new Point(numbers[1], numbers[0]), new Point(numbers[3], numbers[2]));
        }

        public RobotPosition Move() => this with { Position = Position + Velocity };
    }
}