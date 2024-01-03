using System.Drawing;

namespace Minesweeper;

public class Field
{
    public Field(Cell[,] cells)
    {
        Cells = cells;
    }

    public Cell[,] Cells { get; init; }
    public (int x, int y) Size => Cells.GetSize();

    public Cell this[int x, int y] => Cells[x, y];

    public (bool explode, bool win, bool effect) Open((int x, int y) pos)
    {
        if (Cells[pos.x, pos.y].flagged || Cells[pos.x, pos.y].opened)
            return (false, false, false);
        Cells[pos.x, pos.y].opened = true;

        bool explode = Cells[pos.x, pos.y].HasMine;

        if (explode) ShowMines();
        return (explode, CheckWin(), true);
    }
    public (bool win, bool effect) Flag((int x, int y) pos)
    {
        if (Cells[pos.x, pos.y].opened) return (false, false);
        Cells[pos.x, pos.y].flagged = !Cells[pos.x, pos.y].flagged;

        return (CheckWin(), true);
    }

    public void ShowMines()
    {
        for (int x = 0; x < Size.x; x++)
            for (int y = 0; y < Size.y; y++)
                Cells[x, y].opened = Cells[x, y].HasMine;
    }
    public bool CheckWin()
    {
        bool firstCheck = true;

        for (int x = 0; x < Size.x; x++)
        {
            for (int y = 0; y < Size.y; y++)
            {
                if (Cells[x, y].HasMine != Cells[x, y].flagged)
                {
                    firstCheck = false;
                    break;
                }
            }
            if (!firstCheck) break;
        }

        if (firstCheck) return true;

        for (int x = 0; x < Size.x; x++)
            for (int y = 0; y < Size.y; y++)
                if (!Cells[x, y].HasMine && !Cells[x, y].opened)
                    return false;

        return true;
    }

    public static Field Generate((int x, int y) size, int mineCount)
    {
        if (size.x * size.y < 1) throw new ArgumentOutOfRangeException(nameof(size), "Size is too small");

        Random r = new();
        Cell[,] cells = Const.FilledArray(size, new Cell());

        for (int i = 0; i < mineCount; i++)
        {
            int x = 0, y = 0;
            while (x == 0 && y == 0 || cells[x, y].HasMine)
            {
                (x, y) = (r.Next(0, size.x), r.Next(0, size.y));
            }

            cells[x, y] = new(true);
        }
        PlaceNumbers(ref cells);

        return new(cells);
    }
    public static void PlaceNumbers(ref Cell[,] cells)
    {
        for (int x = 0; x < cells.GetLength(0); x++)
        {
            for (int y = 0; y < cells.GetLength(1); y++)
            {
                if (cells[x, y].HasMine) continue;

                int number = 0;
                LookAround(cells, (x, y), (Cell cell) =>
                {
                    if (cell.HasMine) number++;
                });

                cells[x, y] = new(number);
            }
        }
    }

    public static void LookAround<T>(in T[,] array, (int x, int y) pos, Action<T> action)
    {
        if (array.Length < 2) throw new ArgumentException("Array is too small", nameof(array));
        if (!array.Contains(pos)) throw new ArgumentOutOfRangeException(nameof(pos), "Position is out of array bounds");

        for (int x = pos.x - 1; x <= pos.x + 1; x++)
        {
            for (int y = pos.y - 1; y <= pos.y + 1; y++)
            {
                if (x == pos.x && y == pos.y) continue;
                if (!array.Contains((x, y))) continue;

                action.Invoke(array[x, y]);
            }
        }
    }
}
