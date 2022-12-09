using System.Globalization;

namespace AdventOfCode2022.Core.Day08;

public record Day08Solution(IEnumerable<string> Input) : BaseSolution(Input)
{
    public override IEnumerable<string> FirstSolution()
    {
        var forest = new TreeGrid(Input);

        yield return forest.GetVisible().SelectMany(row => row.Values).Count(visible => visible).ToString();
    }
    
    public override IEnumerable<string> SecondSolution()
    {
        var forest = new TreeGrid(Input);

        yield return forest.GetHighestScenicRatio().ToString();
    }
}

internal class TreeGrid
{
    private readonly List<List<int>> _heights = new();
    public TreeGrid(IEnumerable<string> input)
    {
        var rows = input.Select(row => row.Select(c => int.Parse(c.ToString())).ToList()).ToList();
        foreach (var inputRow in rows)
        {
            _heights.Add(inputRow);
        }
    }

    public int GetHighestScenicRatio()
    {
        var maxScenery = 0;
        for (int rowIndex = 0; rowIndex < _heights.Count; rowIndex++)
        {
            for (var colIndex = 0; colIndex < _heights[0].Count; colIndex++)
            {
                var height = _heights[rowIndex][colIndex];
                var scenery = 1;
            
                // Left
                var count = 0;
                for (var col = colIndex - 1; col >= 0; col--)
                {
                    var heightInLine = _heights[rowIndex][col];
                    count++;
                    if (heightInLine >= height)
                    {
                        break;
                    }
                }

                scenery *= count;
                
                // Right
                count = 0;
                for (int col = colIndex + 1; col < _heights[rowIndex].Count; col++)
                {
                    var heightInLine = _heights[rowIndex][col];
                    count++;
                    if (heightInLine >= height)
                    {
                        break;
                    }
                }
                
                scenery *= count;
                
                // Up
                count = 0;
                var column = _heights.Select(row => row[colIndex]).ToArray();
                for (int row = rowIndex - 1; row >= 0; row--)
                {
                    var heightInLine = column[row];
                    count++;
                    if (heightInLine >= height)
                    {
                        break;
                    }
                }
                
                scenery *= count;
                
                // Down
                count = 0;
                for (int row = rowIndex + 1; row < column.Length; row++)
                {
                    var heightInLine = column[row];
                    count++;
                    if (heightInLine >= height)
                    {
                        break;
                    }
                }
                
                scenery *= count;

                maxScenery = scenery > maxScenery ? scenery : maxScenery;
            }
        }

        return maxScenery;
    }

    public List<Dictionary<int, bool>> GetVisible()
    {
        var visible = _heights.Select(row => new Dictionary<int, bool>()).ToList();

        for (var rowIndex = 0; rowIndex < _heights.Count; rowIndex++)
        {
            var comparer = new RowComparer(_heights[rowIndex].ToArray());
            foreach (var visibleColIndex in comparer.GetVisibleIndexes())
            {
                visible[rowIndex][visibleColIndex] = true;
            }
        }

        for (var colIndex = 0; colIndex < _heights[0].Count; colIndex++)
        {
            var column = _heights.Select(row => row[colIndex]).ToArray();
            var comparer = new RowComparer(column);
            foreach (var visibleRowIndex in comparer.GetVisibleIndexes())
            {
                visible[visibleRowIndex][colIndex] = true;
            }
        }
        
        return visible;
    }
}

internal class RowComparer
{
    private readonly int[] _row;

    public RowComparer(int[] row)
    {
        _row = row;
    }

    public IEnumerable<int> GetVisibleIndexes()
    {
        var highestLeft = int.MinValue;
        var highestRight = int.MinValue;

        var leftIndex = -1;
        var rightIndex = _row.Length;
        while (leftIndex < rightIndex)
        {
            if (highestLeft <= highestRight)
            {
                var leftHeight = _row[++leftIndex];
                if (highestLeft < leftHeight)
                {
                    yield return leftIndex;
                    highestLeft = leftHeight;
                }
            }
            else
            {
                var rightHeight = _row[--rightIndex];
                if (highestRight < rightHeight)
                {
                    yield return rightIndex;
                    highestRight = rightHeight;
                }
            }
        }
    }
}