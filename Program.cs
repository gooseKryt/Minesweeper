using System.Media;
using static System.Console;
using static Minesweeper.Game;

namespace Minesweeper;

public static class Program
{
    public static readonly (int x, int y) menuWindowSize = (56, 14);

    private static readonly SoundPlayer[] soundPlayers = new SoundPlayer[]
        { Audio.sStart, Audio.sWin, Audio.sExplosion, Audio.sIntermission, Audio.sBlip1, Audio.sBlip2, Audio.sBlip3 };

    private static void Main()
    {
        InputEncoding = System.Text.Encoding.UTF8;
        OutputEncoding = System.Text.Encoding.Unicode;

        Title = "│▀ Minesweeper";
        SetWindow(menuWindowSize);

        foreach (SoundPlayer soundPlayer in soundPlayers)
            soundPlayer.Load();

        Menu();
    }

    public static void Menu()
    {
        Clear();
        SetWindow(menuWindowSize);
        (explosion, win) = (false, false);
        cursor = (0, 0);
        CursorVisible = false;

        

        int mainSel = ListMenu(Const.GameTitle,
            new string[] { "Play", "Custom game", "Exit" });

        if (mainSel == 0)
        {
            int sel = ListMenu(Const.GameTitle,
                new string[] { Const.easyModeText, Const.mediumModeText, Const.hardModeText }, 3);

            (int w, int h, int m) mode = (-1, -1, -1);
            if (sel == -1)
            {
                Menu();
                return;
            }
            else if (sel == 0) mode = Const.easyMode;
            else if (sel == 1) mode = Const.mediumMode;
            else if (sel == 2) mode = Const.hardMode;

            (fieldSize.x, fieldSize.y, mineCount) = mode;

            MainLoop();
        }
        else if (mainSel == 1)
        {
            int[]? nums = NumberMenu(Const.GameTitle,
            new string[] { "Field width:", "Field height:", "Mine count:" },
            new int[] { defaultSize.x, defaultSize.y, defaultSize.x },
            new int[] { 5, 5, 1 },
            new int[] { maxSize.x, maxSize.y, maxSize.x * maxSize.y - 1 });

            if (nums == null)
            {
                Menu();
                return;
            }

            (fieldSize.x, fieldSize.y, mineCount) = (nums[0], nums[1], nums[2]);
            if (mineCount >= fieldSize.x * fieldSize.y)
                mineCount = fieldSize.x * fieldSize.y - 1;

            MainLoop();
        }
        else if (mainSel == 2)
        {
            Environment.Exit(0);
        }
        else
        {
            WriteLine("Something went wrong... 'O.o");
            Environment.Exit(1);
        }
    }

    public static void WinMenu()
    {
        Clear();
        SetWindow(menuWindowSize);

        ForegroundColor = ConsoleColor.Green;
        Write("│▀▀"); ResetColor();

        WriteLine(" You win!");

        TimeSpan time = stopTimestamp.Subtract(startTimestamp);
        WriteLine($"Time:" +
            $" {Const.DoubleZero(time.Minutes)}" +
            $":{Const.DoubleZero(time.Seconds)}" +
            $".{Const.DoubleZero(time.Milliseconds)}");

        Audio.sIntermission.Play();

        ReadKey(true);
        Audio.sBlip3.Play();
    }
    public static void LoseMenu()
    {
        Clear();
        SetWindow(menuWindowSize);

        ForegroundColor = ConsoleColor.Red;
        Write("-‼-"); ResetColor();

        WriteLine(" You lose!");

        TimeSpan time = stopTimestamp.Subtract(startTimestamp);
        WriteLine($"Time:" +
            $" {Const.DoubleZero(time.Minutes)}" +
            $":{Const.DoubleZero(time.Seconds)}" +
            $".{Const.DoubleZero(time.Milliseconds)}");

        Audio.sIntermission.Play();

        ReadKey(true);
        Audio.sBlip3.Play();
    }

    public static int ListMenu(string title, string[] names, int optHeight = 1)
    {
        WriteLine($"{title}\n");
        ForegroundColor = ConsoleColor.DarkGray;
        for (int i = 0; i < names.Length; i++)
        {
            WriteLine($"  {names[i]}");
        }
        ResetColor();

        int sel = 0;
        ConsoleKey? input = null;
        while (input != ConsoleKey.Spacebar)
        {
            SetCursorPosition(0, sel * optHeight + 2);
            ForegroundColor = ConsoleColor.DarkGray;
            WriteLine($"  {names[sel]}");
            ResetColor();

            bool sound = true;
            switch (input)
            {
                case ConsoleKey.Escape: return -1;

                case ConsoleKey.UpArrow:
                    sel = Const.LoopNum(sel - 1, 0, names.Length);
                    break;
                case ConsoleKey.DownArrow:
                    sel = Const.LoopNum(sel + 1, 0, names.Length);
                    break;

                default: sound = false; break;
            }
            if (sound) Audio.sBlip3.Play();

            SetCursorPosition(0, sel * optHeight + 2);

            ForegroundColor = ConsoleColor.White;
            WriteLine($"> {names[sel]}");
            ResetColor();

            input = ReadKey(true).Key;
        }

        Clear();
        return sel;
    }
    public static int[]? NumberMenu(string title, string[] names, int[] defVals, int[] minVals, int[] maxVals)
    {
        int menuSize = names.Length;
        if (defVals.Length != menuSize || minVals.Length != menuSize || maxVals.Length != menuSize)
            throw new ArgumentException();

        CursorVisible = false;
        int[] vals = defVals;

        WriteLine($"{title}\n");
        ForegroundColor = ConsoleColor.DarkGray;
        for (int i = 0; i < menuSize; i++)
        {
            WriteLine($"  {names[i]} {vals[i]}");
        }
        ResetColor();

        int sel = 0;
        ConsoleKey? input = null;
        while (input != ConsoleKey.Spacebar)
        {
            SetCursorPosition(0, sel + 2);
            ForegroundColor = ConsoleColor.DarkGray;
            WriteLine($"  {names[sel]} {vals[sel]}");
            ResetColor();

            bool sound = true;
            switch (input)
            {
                case ConsoleKey.Escape: return null;

                case ConsoleKey.UpArrow:
                    sel = Const.LoopNum(sel - 1, 0, menuSize);
                    break;
                case ConsoleKey.DownArrow:
                    sel = Const.LoopNum(sel + 1, 0, menuSize);
                    break;

                case ConsoleKey.LeftArrow:
                    vals[sel] = Const.LoopNum(vals[sel] - 1, minVals[sel], maxVals[sel], true);
                    break;
                case ConsoleKey.RightArrow:
                    vals[sel] = Const.LoopNum(vals[sel] + 1, minVals[sel], maxVals[sel], true);
                    break;

                default: sound = false; break;
            }
            if (sound) Audio.sBlip3.Play();

            SetCursorPosition(0, sel + 2);

            ForegroundColor = ConsoleColor.White;
            WriteLine($"> {names[sel]} {vals[sel]}       ");
            ResetColor();

            input = ReadKey(true).Key;
        }

        Clear();
        return vals;
    }

    /*
    static void DrawRect((int x, int y) pos, (int x, int y) size)
    {
        string horiz = "";
        for (int i = 0; i < size.x - 2; i++) horiz += "─";

        SetCursorPosition(pos.x, pos.y + 1);
        for (int i = 0; i < size.y - 2; i++) WriteLine("│");
        SetCursorPosition(pos.x + size.x - 1, pos.y + 1);
        for (int i = 0; i < size.y - 2; i++) WriteLine("│");

        SetCursorPosition(pos.x, pos.y);
        Write($"┌{horiz}┐");
        SetCursorPosition(pos.x, pos.y + size.y - 1);
        Write($"└{horiz}┘");
    }
    */

    public static void Sleep(TimeSpan time)
    {
        Thread.Sleep(time);
        ClearBuffer();
    }
    public static void ClearBuffer()
    {
        while (KeyAvailable)
        {
            ReadKey(false);
        }
    }

    public static void SetWindow((int x, int y) size)
    {
        WindowWidth = size.x;
        WindowHeight = size.y;
        BufferWidth = WindowWidth + 1;
        BufferHeight = WindowHeight + 1;
    }
    public static void SetWindow(int x, int y)
        => SetWindow((x, y));
}