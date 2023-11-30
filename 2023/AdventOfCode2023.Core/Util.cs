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
    
    public static long LowestCommonMultiple(int a, int b)
    {
        return a / GreatestCommonFactor(a, b) * b;
    }
}