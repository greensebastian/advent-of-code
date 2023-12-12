namespace AdventOfCode2023.Core;

public static class Util
{
    // Modulus with negative numbers
    public static int Mod(int x, int m)
    {
        var r = x%m;
        return r < 0 ? r + m : r;
    }
    
    public static long GreatestCommonFactor(long a, long b)
    {
        while (b != 0)
        {
            var temp = b;
            b = a % b;
            a = temp;
        }
        return a;
    }
    
    public static long LowestCommonMultiple(long a, long b)
    {
        return a / GreatestCommonFactor(a, b) * b;
    }
}

public static class EnumerableExtensions
{
    public static IEnumerable<T[]> Batch<T>(this IEnumerable<T> source, int batchSize)
    {
        var batch = new List<T>();
        foreach (var item in source)
        {
            batch.Add(item);
            if (batch.Count >= batchSize)
            {
                yield return batch.ToArray();
                batch.Clear();
            }
        }

        if (batch.Count > 0)
            yield return batch.ToArray();
    }

    public static IEnumerable<int> Ints(this IEnumerable<char> source)
    {
        var currentNumber = string.Empty;
        foreach (var c in source)
        {
            if (char.IsNumber(c) || (currentNumber.Length == 0 && c == '-'))
            {
                currentNumber += c;
            }
            else
            {
                if (string.IsNullOrEmpty(currentNumber)) continue;
                
                yield return int.Parse(currentNumber);
                currentNumber = string.Empty;
            }
        }

        if (currentNumber.Length > 0) yield return int.Parse(currentNumber);
    }
    
    public static IEnumerable<long> Longs(this IEnumerable<char> source)
    {
        var currentNumber = string.Empty;
        foreach (var c in source)
        {
            if (char.IsNumber(c) || (currentNumber.Length == 0 && c == '-'))
            {
                currentNumber += c;
            }
            else
            {
                if (string.IsNullOrEmpty(currentNumber)) continue;
                
                yield return long.Parse(currentNumber);
                currentNumber = string.Empty;
            }
        }

        if (currentNumber.Length > 0) yield return long.Parse(currentNumber);
    }
    
    public static string ReplaceAt(this string str, int index, int length, string replace)
    {
        return string.Create(str.Length - length + replace.Length, (str, index, length, replace),
            (span, state) =>
            {
                state.str.AsSpan()[..state.index].CopyTo(span);
                state.replace.AsSpan().CopyTo(span[state.index..]);
                state.str.AsSpan()[(state.index + state.length)..].CopyTo(span[(state.index + state.replace.Length)..]);
            });
    }

    public static string Repeat(this string str, int count, string separator = "")
    {
        return string.Create(str.Length * count + separator.Length * (count - 1), (str, count, separator), (span, state) =>
        {
            var pos = 0;
            for (var i = 0; i < count; i++)
            {
                state.str.CopyTo(span[pos..]);
                pos += state.str.Length;
                if (i < count - 1)
                {
                    state.separator.CopyTo(span[pos..]);
                    pos += state.separator.Length;
                }
            }
        });
    }
}