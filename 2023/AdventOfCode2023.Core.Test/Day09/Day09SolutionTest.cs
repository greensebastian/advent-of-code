﻿using AdventOfCode2023.Core.Day09;
using Shouldly;

namespace AdventOfCode2023.Core.Test.Day09;

public class Day09SolutionTest
{
    [Fact]
    public void FirstSolution_Example_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day09Solution(input, _ => {});

        var actual = solution.FirstSolution().ToList();

        actual.Single().ShouldBe("114");
    }

    [Fact]
    public async Task FirstSolution_Real_Solves()
    {
        var input = await Util.GetInput(2023, 9);

        var solution = new Day09Solution(input, _ => {});

        var actual = solution.FirstSolution().ToList();

        // 2356385560 Too high
        actual.Single().ShouldBe("1995001648");
    }

    [Fact]
    public void SecondSolution_Example_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day09Solution(input, _ => {});

        var actual = solution.SecondSolution().ToList();

        actual.Single().ShouldBe("2");
    }

    [Fact]
    public async Task SecondSolution_Real_Solves()
    {
        var input = await Util.GetInput(2023, 9);

        var solution = new Day09Solution(input, _ => {});

        var actual = solution.SecondSolution().ToList();

        actual.Single().ShouldBe("988");
    }
}