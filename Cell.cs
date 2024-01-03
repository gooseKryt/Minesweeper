namespace Minesweeper;

public struct Cell
{
    public Cell() : this(false, 0) { }
    public Cell(bool hasMine) : this(hasMine, 0) { }
    public Cell(int number) : this(false, number) { }

    public Cell(bool hasMine, int number)
    {
        (HasMine, Number) = (hasMine, number);
    }

    public bool HasMine { get; init; }
    public int Number { get; init; }

    public bool opened = false;
    public bool flagged = false;
}
