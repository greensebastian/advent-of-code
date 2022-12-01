﻿using AdventOfCode2022.Core.Day01;
using AdventOfCode2022.Core.Day02;
using Shouldly;

namespace AdventOfCode2022.Core.Test.Day02;

public class Day02SolutionTest
{
    [Fact]
    public void FirstSolution_Example_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day02Solution(input);

        var actual = solution.FirstSolution().ToList();
        
        actual.Single().ShouldBe("0");
    }

    [Fact]
    public void FirstSolution_Real_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day02Solution(input);

        var actual = solution.FirstSolution().ToList();
        
        actual.Single().ShouldBe("0");
    }
    
    [Fact]
    public void SecondSolution_Example_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day02Solution(input);

        var actual = solution.SecondSolution().ToList();
        
        actual.Single().ShouldBe("0");
    }

    [Fact]
    public void SecondSolution_Real_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day02Solution(input);

        var actual = solution.SecondSolution().ToList();
        
        actual.Single().ShouldBe("0");
    }
}