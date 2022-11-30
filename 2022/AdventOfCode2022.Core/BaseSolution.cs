﻿namespace AdventOfCode2022.Core;

public abstract record BaseSolution(IEnumerable<string> Input)
{
    public abstract IEnumerable<string> Solve();
}