namespace AdventOfCode2021.Core.Day04;

public record Day04Solution(IEnumerable<string> Input) : BaseSolution(Input)
{
    public override IEnumerable<string> FirstSolution()
    {
        var inputs = new Queue<string>(Input);
        var randomNumbers = inputs.Dequeue().Split(",").Select(int.Parse);

        var boards = LoadBoards(inputs);

        foreach (var pulledNumber in randomNumbers)
        {
            foreach (var board in boards)
            {
                if (board.Mark(pulledNumber))
                {
                    return new[] { (board.SumOfUnmarked * pulledNumber).ToString() };
                }
            }
        }

        return Array.Empty<string>();
    }

    private static List<BingoBoard> LoadBoards(Queue<string> inputs)
    {
        var boards = new List<BingoBoard>();
        while (inputs.Count > 0)
        {
            var line = inputs.Dequeue();
            if (string.IsNullOrWhiteSpace(line))
            {
                boards.Add(new BingoBoard());
                continue;
            }

            var board = boards.Last();
            board.AddRow(line);
        }

        return boards;
    }

    public override IEnumerable<string> SecondSolution()
    {
        var inputs = new Queue<string>(Input);
        var randomNumbers = inputs.Dequeue().Split(",").Select(int.Parse);

        var boards = LoadBoards(inputs);

        BingoBoard? lastWon = null;
        var finalPull = 0;
        
        foreach (var pulledNumber in randomNumbers)
        {
            for (var boardIndex = 0; boardIndex < boards.Count; boardIndex++)
            {
                var board = boards[boardIndex];
                if (board.Mark(pulledNumber))
                {
                    boards.Remove(board);
                    lastWon = board;
                    finalPull = pulledNumber;
                    boardIndex--;
                }
            }
        }
        
        if (lastWon is not null)
            return new[] { (lastWon.SumOfUnmarked * finalPull).ToString() };

        return Array.Empty<string>();
    }
}

class BingoBoard
{
    private readonly List<List<BingoSlot>> _rows = new();

    public void AddRow(string serializedRow)
    {
        _rows.Add(serializedRow
            .Split(" ")
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .Select(s => new BingoSlot
            {
                Number = int.Parse(s)
            })
            .ToList());
    }

    public int SumOfUnmarked => _rows
        .SelectMany(row => row.Where(cell => !cell.Marked))
        .Sum(cell => cell.Number);

    public bool Mark(int number)
    {
        if (!TryMark(number, out var hitRow, out var hitCol)) return false;
        
        // Check row win
        if (_rows[hitRow].All(cell => cell.Marked))
            return true;

        // Check column win
        if (_rows.Select(row => row[hitCol]).All(cell => cell.Marked))
            return true;

        return false;
    }
    
    private bool TryMark(int number, out int hitRow, out int hitCol)
    {
        for (var rowIndex = 0; rowIndex < _rows.Count; rowIndex++)
        {
            var row = _rows[rowIndex];
            for (var colIndex = 0; colIndex < row.Count; colIndex++)
            {
                var cell = row[colIndex];
                if (cell.Number != number) continue;
                
                hitRow = rowIndex;
                hitCol = colIndex;
                cell.Marked = true;
                return true;
            }
        }

        hitRow = -1;
        hitCol = -1;
        return false;
    }
}

internal class BingoSlot
{
    public required int Number { get; init; }
    public bool Marked { get; set; } = false;
}