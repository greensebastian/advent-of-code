using System.Text;
using FluentAssertions;

namespace AdventOfCode2024.Tests.Solutions;

public class Day18 : ISolution
{
    private const string Example = """
                                   5,4
                                   4,2
                                   4,5
                                   3,0
                                   2,1
                                   6,3
                                   2,4
                                   1,5
                                   0,6
                                   3,3
                                   2,6
                                   5,1
                                   1,2
                                   5,5
                                   2,5
                                   6,5
                                   1,4
                                   0,4
                                   6,4
                                   1,1
                                   6,1
                                   1,0
                                   0,5
                                   1,6
                                   2,0
                                   """;
    
    [Fact]
    public void Solution1()
    {
        //var input = Util.ReadRaw(Example);
        //var input = Util.ReadFile("day18");

        //var result = new MemoryMaze(Util.ReadRaw(Example), new Point(6, 6)).MinimumSteps(12);
        var result = new MemoryMaze(Util.ReadFile("day18"), new Point(70, 70)).MinimumSteps(1024);
        result.Should().Be(22);
    }

    [Fact]
    public void Solution2()
    {
        var input = Util.ReadRaw(Example);
        //var input = Util.ReadFile("day18");

        var result = new MemoryMaze(Util.ReadRaw(Example), new Point(6, 6)).MinimumSteps(12);
        //var result = new MemoryMaze(input, new Point(70, 70)).MinimumSteps(1024);
        result.Should().Be(22);
    }

    public class MemoryMaze
    {
        private readonly Point _bounds;
        private readonly List<HashSet<Point>> _availableSpaces;
        
        public MemoryMaze(string[] input, Point bounds)
        {
            _bounds = bounds;
            _availableSpaces = new();
            var free = Enumerable.Range(0, (int)bounds.Row + 1).SelectMany(row => Enumerable.Range(0, (int)bounds.Col + 1).Select(col => new Point(row, col))).ToHashSet();
            _availableSpaces.Add(free);
            foreach (var line in input)
            {
                var falling = new Point(line.PlusMinusLongs()[1], line.PlusMinusLongs()[0]);
                var next = free.Where(p => p != falling).ToHashSet();
                _availableSpaces.Add(next);
                free = next;
                //Print(_availableSpaces.Count - 1);
            }
        }

        public void Print(int time)
        {
            var sb = new StringBuilder();
            for (int row = 0; row <= _bounds.Row; row++)
            {
                for (int col = 0; col <= _bounds.Col; col++)
                {
                    sb.Append(_availableSpaces[time].Contains(new Point(row, col)) ? '.' : '#');
                }

                sb.AppendLine();
            }
            Console.WriteLine(sb.ToString());
        }

        public int MinimumSteps(int limit)
        {
            var result = new Node<Point>(Point.Origin, null).Dijkstra(0,
                node => node.Value.ClockwiseOrthogonalNeighbours()
                    .Where(n => _availableSpaces[limit].Contains(n)).Select(n => n), node => 1,
                node => node.Value == _bounds, (node, dist) => false).First();
            return (int)result.Dist;
        }
    }
    
    private long Solve(string[] input) => throw new NotImplementedException();
}