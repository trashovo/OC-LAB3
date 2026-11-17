unsafe class Program
{
    [DllImport("libc")]
    static extern byte* mmap(void* addr, nuint length, int prot, int flags, int fd, nint offset);

    [DllImport("libc")]
    static extern int munmap(void* addr, nuint length);

    static void ShowMem(string step)
    {
        Console.WriteLine($"\n--- {step} ---");
        foreach (string line in File.ReadAllLines("/proc/self/status"))
        {
            if (line.StartsWith("VmSize:") || line.StartsWith("VmRSS:"))
                Console.WriteLine(line);
        }
        string stat = File.ReadAllText("/proc/self/stat");
        string[] parts = stat.Substring(stat.LastIndexOf(')') + 2).Split(' ');
        Console.WriteLine($"minor faults: {parts[7]}");
    }

    static void Main()
    {
        int pageSize = 4096;
        nuint size = 256 * 1024 * 1024;

        ShowMem("1. Начало");

        byte* block = mmap(null, size, 3, 0x22, -1, 0);
        ShowMem("2. После выделения");

        long sum = 0;
        var sw = Stopwatch.StartNew();
        for (nuint i = 0; i < size; i += (nuint)pageSize)
            sum += Volatile.Read(ref block[i]);
        sw.Stop();
        ShowMem($"3. После чтения ({sw.Elapsed.TotalMilliseconds:F2} мс, сумма = {sum})");

        sw.Restart();
        for (nuint i = 0; i < size; i += (nuint)pageSize)
            Volatile.Write(ref block[i], 1);
        sw.Stop();
        ShowMem($"4. После первой записи ({sw.Elapsed.TotalMilliseconds:F2} мс)");

        sw.Restart();
        for (nuint i = 0; i < size; i += (nuint)pageSize)
            Volatile.Write(ref block[i], 2);
        sw.Stop();
        ShowMem($"5. После повторной записи ({sw.Elapsed.TotalMilliseconds:F2} мс)");
        Console.WriteLine($"Проверка: {block[0]}, {block[4096]}, {block[8192]}, {block[size - 4096]}");

        munmap(block, size);
        ShowMem("6. После освобождения");
    }
}
