﻿using AdventOfCode2022.Core.Day18;
using Shouldly;

namespace AdventOfCode2022.Core.Test.Day18;

public class Day18SolutionTest
{
    [Fact]
    public void FirstSolution_Example_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day18Solution(input);

        var actual = solution.FirstSolution().ToList();

        actual.Single().ShouldBe("64");
    }

    [Fact]
    public void FirstSolution_Real_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day18Solution(input);

        var actual = solution.FirstSolution().ToList();

        actual.Single().ShouldBe("4242");
    }

    [Fact]
    public void SecondSolution_Example_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day18Solution(input);

        var actual = solution.SecondSolution().ToList();

        actual.Single().ShouldBe("0");
    }

    [Fact]
    public void SecondSolution_Real_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day18Solution(input);

        var actual = solution.SecondSolution().ToList();

        actual.Single().ShouldBe("0");
    }
}