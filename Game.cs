using static System.Console;

namespace Minesweeper;

public static class Game
{
    private static Field field = null!;
    public static (int x, int y) fieldSize;
    public static readonly (int x, int y)
        defaultSize = (16, 9),
        maxSize = (64, 32);
    public static int mineCount;

    public static bool explosion = false;
    public static bool win = false;

    private static Cell SelectedCell => field[cursor.x, cursor.y];
    public static(int x, int y) cursor = (0, 0);

    private static readonly Dictionary<ConsoleKey, Action> Actions = new()
    {
        { ConsoleKey.LeftArrow, ()
            => { cursor.x--; ClampCursor(); RenderCursor(); } },
        { ConsoleKey.RightArrow, ()
            => { cursor.x++; ClampCursor(); RenderCursor(); } },
        { ConsoleKey.UpArrow, ()
            => { cursor.y--; ClampCursor(); RenderCursor(); } },
        { ConsoleKey.DownArrow, ()
            => { cursor.y++; ClampCursor(); RenderCursor(); } },

        { ConsoleKey.Spacebar, () =>
            {
                (bool explode, bool win, bool effect) = field.Open(cursor);
                if (effect) Audio.sBlip1!.Play();
                RenderCell();

                if (explode) Explode();
                else if (win) Win();
            }
        },
        { ConsoleKey.E, () =>
            {
                (bool win, bool effect) = field.Flag(cursor);
                if (effect) Audio.sBlip2!.Play();
                RenderCell();

                if (win) Win();
            }
        }
    };

    private static readonly Dictionary<int, ConsoleColor> NumberColors = new()
    {
        { 0, ConsoleColor.DarkGray },
        { 1, ConsoleColor.Blue },
        { 2, ConsoleColor.DarkGreen },
        { 3, ConsoleColor.Red },
        { 4, ConsoleColor.DarkBlue },
        { 5, ConsoleColor.DarkRed },
        { 6, ConsoleColor.DarkCyan },
        { 7, ConsoleColor.White },
        { 8, ConsoleColor.Gray }
    };

    private const char ClosedCell = '=';
    private const char EmptyCell = ' ';

    private const char FlagChar = 'P';
    private const ConsoleColor FlagColor = ConsoleColor.White;
    private const char MineChar = '*';
    private const ConsoleColor MineColor = ConsoleColor.DarkRed;

    public static DateTime startTimestamp, stopTimestamp;

    public static void MainLoop()
    {
        CursorVisible = false;
        Clear();
        Program.SetWindow((fieldSize.x * 2 + 2, fieldSize.y + 1));

        field = Field.Generate(fieldSize, mineCount);

        ForegroundColor = NumberColors[0];
        for (int y = 0; y < fieldSize.y; y++)
        {
            Write(" ");
            for (int x = 0; x < fieldSize.x; x++)
            {
                Write($"{ClosedCell} ");
            }
            WriteLine();
        }
        ResetColor();

        Audio.sStart.Play();

        RenderCursor(false);
        startTimestamp = DateTime.Now;

        while (true)
        {
            Actions.TryGetValue(ReadKey(true).Key, out Action? action);
            if (action == null) continue;
            action();

            if (explosion)
            {
                Program.LoseMenu(); break;
            }
            else if (win)
            {
                Program.WinMenu(); break;
            }
        }

        Program.Menu();
    }

    private static void Win()
    {
        win = true;
        stopTimestamp = DateTime.Now;

        Audio.sWin.Play();

        Program.Sleep(TimeSpan.FromSeconds(1.5d));
    }
    private static void Explode()
    {
        explosion = true;
        stopTimestamp = DateTime.Now;

        for (int x = 0; x < fieldSize.x; x++)
        {
            for (int y = 0; y < fieldSize.y; y++)
            {
                if (field[x, y].HasMine) RenderCell((x, y));
                else continue;
            }
        }

        Audio.sExplosion.Play();

        Program.Sleep(TimeSpan.FromSeconds(1.5d));
    }

    private static void ClampCursor()
    {
        if (cursor.x < 0) cursor.x = 0;
        if (cursor.x >= fieldSize.x) cursor.x = fieldSize.x - 1;

        if (cursor.y < 0) cursor.y = 0;
        if (cursor.y >= fieldSize.y) cursor.y = fieldSize.y - 1;
    }

    private static void RenderCell()
        => RenderCell(cursor);
    private static void RenderCell((int x, int y) pos)
    {
        SetCursor(pos);
        Cell cell = SelectedCell;

        char c;
        ConsoleColor color;

        if (cell.flagged)
        {
            c = FlagChar;
            color = FlagColor;
        }
        else if (!cell.opened)
        {
            c = ClosedCell;
            color = NumberColors[0];
        }
        else if (cell.HasMine)
        {
            c = MineChar;
            color = MineColor;
        }

        else if (cell.Number == 0)
        {
            c = EmptyCell;
            color = NumberColors[0];
        }
        else
        {
            c = cell.Number.ToString()[0];
            color = NumberColors[cell.Number];
        }

        ForegroundColor = color;
        Write(c);
        CursorLeft += 1;
        ResetColor();
    }
    private static void RenderCursor(bool reRender = true)
    {
        ForegroundColor = ConsoleColor.DarkGray;

        if (reRender)
        {
            CursorLeft -= 1;
            Write(" ");
            CursorLeft -= 3;
            Write(" ");
        }

        SetCursor(false);

        Write("[");
        CursorLeft += 1;
        Write("]");

        ResetColor();
    }

    private static void SetCursor(bool center = true)
    {
        CursorLeft = cursor.x * 2 + (center ? 1 : 0);
        CursorTop = cursor.y;
    }
    private static void SetCursor((int x, int y) pos)
    {
        CursorLeft = pos.x * 2 + 1;
        CursorTop = pos.y;
    }
}
