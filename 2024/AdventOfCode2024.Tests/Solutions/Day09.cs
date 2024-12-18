using FluentAssertions;

namespace AdventOfCode2024.Tests.Solutions;

public class Day09 : ISolution
{
    private const string Example = """
                                   2333133121414131402
                                   """;
    
    [Fact]
    public void Solution1()
    {
        //var input = Util.ReadRaw(Example);
        var input = Util.ReadFile("day09");
        
        var answer = CompactedChecksum(input.Single());
        answer.Should().Be(6359213660505L);
    }

    [Fact]
    public void Solution2()
    {
        //var input = Util.ReadRaw(Example);
        var input = Util.ReadFile("day08");

        var answer = CompactedChecksum(input.Single());
        answer.Should().Be(951);
    }

    private long CompactedChecksum(string input)
    {
        var data = new List<long?>(input.Length);
        var fileIndex = 0;
        for (var inputIndex = 0; inputIndex < input.Length; inputIndex++)
        {
            var isFile = inputIndex % 2 == 0;
            var toAdd = isFile ? fileIndex : (int?)null;
            var length = int.Parse(input[inputIndex].ToString());
            for (var sizeInc = 0; sizeInc < length; sizeInc++)
            {
                data.Add(toAdd);
            }

            if (isFile) fileIndex++;
        }

        var freeIndex = data.FindIndex(i => !i.HasValue);
        var toMoveIndex = data.Count - 1;
        while (freeIndex < toMoveIndex)
        {
            while (!data[toMoveIndex].HasValue) toMoveIndex--;
            //Console.WriteLine(string.Join("", data.Select(n => n.HasValue ? n.Value.ToString() : ".")));
            if (!data[freeIndex].HasValue)
            {
                data[freeIndex] = data[toMoveIndex];
                data.RemoveAt(toMoveIndex);
                toMoveIndex--;
            }

            freeIndex++;
        }

        return data.Select((n, i) => n.HasValue ? n.Value * i : 0).Sum();
    }
}