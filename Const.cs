namespace Minesweeper;

public static class Const
{
    public const string GameTitle = "│▀▀ Minesweeper";

    public static readonly (int w, int h, int m) easyMode =
        (8, 5, 6);
    public static readonly (int w, int h, int m) mediumMode =
        (16, 9, 16);
    public static readonly (int w, int h, int m) hardMode =
        (32, 16, 84);

    public static readonly string easyModeText =
        $"""
        Easy
        {easyMode.w}x{easyMode.h}, {easyMode.m} mines

        """;
    public static readonly string mediumModeText =
        $"""
        Medium
        {mediumMode.w}x{mediumMode.h}, {mediumMode.m} mines

        """;
    public static readonly string hardModeText =
        $"""
        Hard
        {hardMode.w}x{hardMode.h}, {hardMode.m} mines

        """;

    public static class Sounds
    {
        private static readonly string Dir
            = Environment.CurrentDirectory + @"\Assets\";

        public static readonly string start = Dir + "start.wav";
        public static readonly string win = Dir + "win.wav";
        public static readonly string explosion = Dir + "explosion.wav";
        public static readonly string intermission = Dir + "intermission.wav";
        public static readonly string blip1 = Dir + "blip1.wav";
        public static readonly string blip2 = Dir + "blip2.wav";
        public static readonly string blip3 = Dir + "blip3.wav";
    }

    public static T[,] FilledArray<T>((int x, int y) size, T value)
    {
        T[,] array = new T[size.x, size.y];

        for (int x = 0; x < size.x; x++)
            for (int y = 0; y < size.y; y++)
                array[x, y] = value;

        return array;
    }

    public static (int x, int y) GetSize<T>(this T[,] array)
    {
        return (array.GetLength(0), array.GetLength(1));
    }
    public static bool Contains<T>(this T[,] array, (int x, int y) pos)
    {
        (int x, int y) = GetSize(array);

        return !(pos.x < 0 || pos.x >= x
            || pos.y < 0 || pos.y >= y);
    }

    public static int LoopNum(int value, int min, int max, bool maxInclusive = false)
    {
        if (maxInclusive)
        {
            if (value < min) return max;
            if (value > max) return min;
            return value;
        }
        else
        {
            if (value < min) return max - 1;
            if (value >= max) return min;
            return value;
        }
    }
    public static string DoubleZero(int number)
    {
        return number > 9 ? $"{number}" : $"0{number}";
    }
}
