﻿using AdventOfCode2021.Core.Day05;
using Shouldly;

namespace AdventOfCode2021.Core.Test.Day05;

public class Day05SolutionTest
{
    [Fact]
    public void FirstSolution_Example_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day05Solution(input);

        var actual = solution.FirstSolution().ToList();
        
        actual.Single().ShouldBe("5");
    }

    [Fact]
    public void FirstSolution_Real_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day05Solution(input);

        var actual = solution.FirstSolution().ToList();
        
        actual.Single().ShouldBe("8111");
    }
    
    [Fact]
    public void SecondSolution_Example_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day05Solution(input);

        var actual = solution.SecondSolution().ToList();
        
        actual.Single().ShouldBe("12");
    }

    [Fact]
    public void SecondSolution_Real_Solves()
    {
        var input = Util.ReadFromFile("input");

        var solution = new Day05Solution(input);

        var actual = solution.SecondSolution().ToList();
        
        actual.Single().ShouldBe("22088");
    }
}