using System.Text;
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

        //var sum = SecondsUntilTree(Util.ReadRaw(Example), new Point(7, 11));
        var sum = SecondsUntilTree(Util.ReadFile("day14"), new Point(103, 101));
        sum.Should().Be(8270);
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
    
    private long SecondsUntilTree(string[] input, Point bounds)
    {
        var robots = input.Select(RobotPosition.FromInput).ToList();
        var room = new RobotRoom(robots, bounds);
        while (true)
        {
            room.MoveAllRobots();
            if (room.IsTree()) return room.SecondsPassed;
        }
    }

    private class RobotRoom(List<RobotPosition> robots, Point bounds)
    {
        private (long, long) _lowestError = (long.MaxValue, long.MaxValue);
        
        private long Error(long row)
        {
            var error = 0L;
            var allowedDistance = row;
            var lowerBound = bounds.Col / 2 - allowedDistance;
            var upperBound = bounds.Col / 2 + allowedDistance;
            foreach (var robot in robots.Where(r => r.Position.Row == row).DistinctBy(r => r.Position.Col))
            {
                if (robot.Position.Col < lowerBound)
                {
                    error += Math.Abs(robot.Position.Col - lowerBound);
                }
                if (robot.Position.Col > upperBound)
                {
                    error += Math.Abs(robot.Position.Col - upperBound);
                }
            }

            return error;
        }
        
        private bool CouldBeTree()
        {
            var error = 0L;
            for (var i = 0; i < bounds.Row - 35; i++)
            {
                error += Error(i);
            }

            if (error < _lowestError.Item1)
            {
                _lowestError = (error, SecondsPassed);
                if (error <= 507) return true; // Found from visual inspection
            }

            return false;
        }
        
        public bool IsTree()
        {
            
            var shouldPrint = CouldBeTree();
            if (shouldPrint)
            {
                Console.WriteLine(ToString());
            }

            var isTree = shouldPrint || SecondsPassed > 1_000_000_000;

            return isTree;
        }
        
        public override string ToString()
        {
            return ToString(p => robots.Find(r => r.Position == p) != null ? "#" : ".");
        }
    
        public string ToString(Func<Point, string> serialize)
        {
            var sb = new StringBuilder();
            for (var row = 0; row < bounds.Row; row++)
            {
                for (var col = 0; col <= bounds.Col; col++)
                {
                    sb.Append(serialize(new Point(row, col)));
                }

                sb.AppendLine();
            }

            return sb.ToString();
        }

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