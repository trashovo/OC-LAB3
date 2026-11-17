class Program
{
    static void Main()
    {
        int N = 8192;
        int[] a = new int[N * N];

        for (int i = 0; i < a.Length; i++)
            a[i] = i % 10;

        for (int run = 1; run <= 3; run++)
        {
            Console.WriteLine($"\nЗамер {run}");

            // По строкам
            long sumRow = 0;
            var sw = Stopwatch.StartNew();
            for (int i = 0; i < N; i++)
                for (int j = 0; j < N; j++)
                    sumRow += a[i * N + j];
            sw.Stop();
            Console.WriteLine($"По строкам:  {sw.Elapsed.TotalMilliseconds:F2} мс (сумма = {sumRow})");

            // По столбцам
            long sumCol = 0;
            sw.Restart();
            for (int j = 0; j < N; j++)
                for (int i = 0; i < N; i++)
                    sumCol += a[i * N + j];
            sw.Stop();
            Console.WriteLine($"По столбцам: {sw.Elapsed.TotalMilliseconds:F2} мс (сумма = {sumCol})");
        }
    }
}
