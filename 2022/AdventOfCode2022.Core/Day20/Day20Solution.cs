using System.Collections;
using System.Diagnostics;

namespace AdventOfCode2022.Core.Day20;

public record Day20Solution(IEnumerable<string> Input, Action<string> Log) : BaseSolution(Input, Log)
{
    public override IEnumerable<string> FirstSolution(params string[] args)
    {
        var list = new MixerList(Input, 1);
        Mixer.Mix(list, 1, Log);
        var result = list.CoordinateSum();

        yield return result.ToString();
    }
    
    public override IEnumerable<string> SecondSolution(params string[] args)
    {
        var decryptionKey = long.Parse(args[0]);
        var iterations = long.Parse(args[1]);
        
        var list = new MixerList(Input, decryptionKey);
        Mixer.Mix(list, iterations, Log);
        var result = list.CoordinateSum();

        yield return result.ToString();
    }
}

public static class Mixer
{
    public static void Mix(MixerList list, long iterations, Action<string> log)
    {
        log.Invoke("Starting mixing..");
        var sw = Stopwatch.StartNew();
        var nodes = list.ToArray();
        for (var i = 0; i < iterations; i++)
        {
            log.Invoke($"Starting iteration {i} at {sw.Elapsed}");
            foreach (var node in nodes)
            {
                node.MoveUp(node.Value);
            }
            log.Invoke($"Iteration {i} done at {sw.Elapsed}");
            log.Invoke(list.ToString());
        }
        log.Invoke($"Done mixing after {sw.Elapsed}");
    }
}

public class MixerList : IEnumerable<MixerNode>
{
    public MixerNode First { get; set; }
    public int Length { get; private set; }
    public MixerList(IEnumerable<string> input, long decryptionKey)
    {
        var toAdd = input.Select(line => long.Parse(line) * decryptionKey).ToArray();
        
        First = new MixerNode(this, toAdd[0]);
        Length++;
        var cur = First;
        foreach (var nbrToAdd in toAdd[1..])
        {
            cur = new MixerNode(this, nbrToAdd, cur);
            Length++;
        }
    }

    public long CoordinateSum()
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
    public long Value { get; }
    public MixerNode Prev { get; private set; }
    public MixerNode Next { get; private set; }
    
    public MixerNode(MixerList list, long value)
    {
        List = list;
        Value = value;
        Prev = this;
        Next = this;
    }
    
    public MixerNode(MixerList list, long value, MixerNode prev) : this(list, value)
    {
        InsertAfter(prev);
    }

    private long NormalizeSteps(long steps)
    {
        if (steps < 0) steps *= -1;
        steps %= List.Length - 1;
        //if (steps >= List.Length) return steps % List.Length;
        return steps;
    }

    public void MoveUp(long steps)
    {
        if (steps == 0) return;

        var cur = this;

        if (steps > 0)
        {
            steps = NormalizeSteps(steps);
            for (var moved = 0; moved < steps; moved++)
            {
                cur = cur.Next;
                if (cur == this && moved > 0) moved--;
            }
            MoveAfter(cur);
        }

        if (steps < 0)
        {
            steps = NormalizeSteps(steps);
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
        if (first == this) first = first.Prev;
        var middle = this;
        var last = target.Next;
        if (last == this) last = last.Next;

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