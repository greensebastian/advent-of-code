using System.Text;
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
        var input = Util.ReadFile("day09");

        var answer = CompactedWholeFileChecksum(input.Single());
        answer.Should().Be(6381624803796L);
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
    
    private long CompactedWholeFileChecksum(string input)
    {
        var data = new List<MemoryBlock>(input.Length);
        var fileIndex = 0;
        var startIndex = 0;
        for (var inputIndex = 0; inputIndex < input.Length; inputIndex++)
        {
            var isFile = inputIndex % 2 == 0;
            var value = isFile ? fileIndex : (int?)null;
            var length = int.Parse(input[inputIndex].ToString());
            data.Add(new MemoryBlock(startIndex, length, value));
            startIndex += length;
            if (isFile) fileIndex++;
        }

        data.RemoveAll(mb => mb.Size == 0);

        for (var indexToMove = data.Count; indexToMove > 0; indexToMove--)
        {
            if (indexToMove > data.Count - 1) continue;
            var toMove = data[indexToMove];
            if (toMove.IsFree()) continue;
            var indexToFree = data.FindIndex(mb => mb.StartIndex < toMove.StartIndex && mb.CanFit(toMove));
            if (indexToFree < 0 || indexToFree >= indexToMove) continue;
            var toFree = data[indexToFree];
            var newBlocks = toFree.ReplaceWith(toMove).ToArray();
            
            data[indexToMove] = toMove with { FileIndex = null };
            data.RemoveAt(indexToFree);
            data.InsertRange(indexToFree, newBlocks);
            indexToMove++;
        }

        return data.Sum(mb => mb.Checksum());
    }

    private void Print(IEnumerable<MemoryBlock> mbs)
    {
        var sb = new StringBuilder();
        foreach (var mb in mbs)
        {
            for (var i = 0; i < mb.Size; i++)
            {
                sb.Append(mb.IsFree() ? "." : mb.FileIndex.ToString());
            }
        }
        Console.WriteLine(sb.ToString());
    }

    private readonly record struct MemoryBlock(int StartIndex, int Size, int? FileIndex)
    {
        public bool IsFree() => !FileIndex.HasValue;
        public bool CanFit(MemoryBlock other) => IsFree() && other.Size <= Size;

        public IEnumerable<MemoryBlock> ReplaceWith(MemoryBlock other)
        {
            if (!IsFree()) throw new InvalidOperationException();
            yield return other with { StartIndex = StartIndex };
            var remaining = Size - other.Size;
            if (remaining > 0)
                yield return this with { StartIndex = StartIndex + other.Size, Size = Size - other.Size };
        }

        public long Checksum()
        {
            var sum = 0L;
            for (var i = 0; i < Size; i++)
            {
                sum += (FileIndex ?? 0) * (StartIndex + i);
            }

            return sum;
        }
    }
}