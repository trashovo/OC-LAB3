using System.Diagnostics;
using System.Runtime.InteropServices;

unsafe class Program
{
    // Подключение системных вызовов Linux
    [DllImport("libc")]
    static extern byte* mmap(void* addr, nuint length, int prot, int flags, int fd, nint offset);

    [DllImport("libc")]
    static extern int munmap(void* addr, nuint length);

    static void Main()
    {
        int pageSize = 4096;
        nuint size = 256 * 1024 * 1024;

        Console.WriteLine("1. Начало.");
        Console.ReadLine();

        // Шаг 2: выделяем память
        byte* block = mmap(null, size, 3, 0x22, -1, 0);
        Console.WriteLine("2. Память выделена (mmap).");
        Console.ReadLine();

        // Шаг 3: первое чтение
        long sum = 0;
        var sw = Stopwatch.StartNew();
        for (nuint i = 0; i < size; i += (nuint)pageSize)
        {
            sum += Volatile.Read(ref block[i]);
        }
        sw.Stop();
        Console.WriteLine($"3. Чтение готово за {sw.Elapsed.TotalMilliseconds:F2} мс (сумма = {sum}).");
        Console.ReadLine();

        // Шаг 4: первая запись
        sw.Restart();
        for (nuint i = 0; i < size; i += (nuint)pageSize)
        {
            Volatile.Write(ref block[i], 1);
        }
        sw.Stop();
        Console.WriteLine($"4. Первая запись готова за {sw.Elapsed.TotalMilliseconds:F2} мс.");
        Console.ReadLine();

        // Шаг 5: повторная запись
        sw.Restart();
        for (nuint i = 0; i < size; i += (nuint)pageSize)
        {
            Volatile.Write(ref block[i], 2);
        }
        sw.Stop();
        Console.WriteLine($"5. Повторная запись готова за {sw.Elapsed.TotalMilliseconds:F2} мс.");
        Console.WriteLine($"Проверка: {block[0]}, {block[4096]}, {block[8192]}, {block[size - 4096]}");
        Console.WriteLine("Сверь память");
        Console.ReadLine();

        // Шаг 6: освобождение
        munmap(block, size);
        Console.WriteLine("6. Память освобождена (munmap). Сверь память");
        Console.ReadLine();
    }
}
