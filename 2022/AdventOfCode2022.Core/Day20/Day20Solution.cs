using System.Collections;

namespace AdventOfCode2022.Core.Day20;

public record Day20Solution(IEnumerable<string> Input) : BaseSolution(Input)
{
    public override IEnumerable<string> FirstSolution(params string[] args)
    {
        var list = Mixer.Mix(Input);
        var result = list.CoordinateSum();

        yield return result.ToString();
    }
    
    public override IEnumerable<string> SecondSolution(params string[] args)
    {
        yield return "0";
    }
}

public static class Mixer
{
    public static MixerList Mix(IEnumerable<string> input)
    {
        var list = new MixerList(input);
        var nodes = list.ToArray();
        foreach (var node in nodes)
        {
            node.MoveUp(node.Value);
        }

        return list;
    }
}

public class MixerList : IEnumerable<MixerNode>
{
    public MixerNode First { get; set; }
    public int Length { get; private set; }
    public MixerList(IEnumerable<string> input)
    {
        var toAdd = input.Select(int.Parse).ToArray();
        
        First = new MixerNode(this, toAdd[0]);
        Length++;
        var cur = First;
        foreach (var nbrToAdd in toAdd[1..])
        {
            cur = new MixerNode(this, nbrToAdd, cur);
            Length++;
        }
    }

    public int CoordinateSum()
    {
        var postMix = this.ToArray();
        var startIndex = 0;
        while (postMix[startIndex].Value != 0)
        {
            startIndex++;
        }

        var coords = new[]
        {
            postMix[(startIndex + 1000) % postMix.Length],
            postMix[(startIndex + 2000) % postMix.Length],
            postMix[(startIndex + 3000) % postMix.Length]
        };

        var sum = coords.Select(n => n.Value).Sum();
        return sum;
    }

    public override string ToString()
    {
        return Length > 30 
            ? $"{Length}:\t[{First.Value}..{First.Prev.Value}]" 
            : $"({Length}):\t[{string.Join(", ", this.Select(n => n.Value))}]";
    }
    
    public IEnumerator<MixerNode> GetEnumerator()
    {
        var output = new List<MixerNode>();
        var cur = First.Next;
        output.Add(First);
        while (cur != First)
        {
            output.Add(cur);
            cur = cur.Next;
        }

        return output.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

public class MixerNode
{
    private MixerList List { get; }
    public int Value { get; }
    public MixerNode Prev { get; private set; }
    public MixerNode Next { get; private set; }
    
    public MixerNode(MixerList list, int value)
    {
        List = list;
        Value = value;
        Prev = this;
        Next = this;
    }
    
    public MixerNode(MixerList list, int value, MixerNode prev) : this(list, value)
    {
        InsertAfter(prev);
    }

    public void MoveUp(int steps)
    {
        if (steps == 0 || Math.Abs(steps) == List.Length - 1) return;
        
        var cur = this;

        if (steps > 0)
        {
            for (var moved = 0; moved < steps; moved++)
            {
                cur = cur.Next;
                if (cur == this && moved > 0) moved--;
            }
            MoveAfter(cur);
        }

        if (steps < 0)
        {
            steps *= -1;
            for (var moved = 0; moved < steps; moved++)
            {
                cur = cur.Prev;
                if (cur == this && moved > 0) moved--;
            }
            MoveAfter(cur.Prev);
        }
    }

    private void InsertAfter(MixerNode target)
    {
        // Target placements
        var first = target;
        var middle = this;
        var last = target.Next;

        // New linking
        first.Next = middle;
        middle.Prev = first;
        middle.Next = last;
        last.Prev = middle;
    }

    private void MoveAfter(MixerNode target)
    {
        if (this == List.First)
        {
            List.First = Next;
        }
        
        // Shift out of old spot
        var oldPrev = Prev;
        var oldNext = Next;
        oldPrev.Next = oldNext;
        oldNext.Prev = oldPrev;
        
        InsertAfter(target);
    }

    public override string ToString() => Value.ToString();
}