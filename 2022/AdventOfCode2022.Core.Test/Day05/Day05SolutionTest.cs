﻿using AdventOfCode2022.Core.Day05;
using Shouldly;

namespace AdventOfCode2022.Core.Test.Day05;

public class Day05SolutionTest
{
    [Fact]
    public void FirstSolution_Example_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day05Solution(input, _ => {});

        var actual = solution.FirstSolution().ToList();

        actual.Single().ShouldBe("CMZ");
    }

    [Fact]
    public void FirstSolution_Real_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day05Solution(input, _ => {});

        var actual = solution.FirstSolution().ToList();

        actual.Single().ShouldBe("RLFNRTNFB");
    }

    [Fact]
    public void SecondSolution_Example_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day05Solution(input, _ => {});

        var actual = solution.SecondSolution().ToList();

        actual.Single().ShouldBe("MCD");
    }

    [Fact]
    public void SecondSolution_Real_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day05Solution(input, _ => {});

        var actual = solution.SecondSolution().ToList();

        actual.Single().ShouldBe("MHQTLJRLB");
    }
}