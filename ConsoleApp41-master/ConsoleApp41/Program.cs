using System.Diagnostics;
namespace AsyncLab
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("=== ЗАДАНИЕ 1: Базовый асинхронный метод ===\n"); await RunTask1();
            Console.WriteLine("\n==============================================\n");

            Console.WriteLine("=== ЗАДАНИЕ 2: Оптимизация времени ===\n");
            await RunTask2();

            Console.WriteLine("\nНажмите любую клавишу для выхода...");
            Console.ReadKey();
        }

        static async Task RunTask1()
        {
            Task task = PrintMessageAsync("Загрузка конфигурации", 2000);

            Console.WriteLine("[Main] Продолжение работы основного потока.");

            await Task.Run(() =>
            {
                System.Threading.Thread.Sleep(500);
            });
            Console.WriteLine("[Main] Фоновая операция завершена.");

            await task;
        }

        static async Task PrintMessageAsync(string message, int delayMs)
        {
            Console.WriteLine($"[Async Start] Начало задачи: {message}.");

            await Task.Delay(delayMs);

            Console.WriteLine($"[Async End] Завершена задача: {message}.");
        }

        static async Task RunTask2()
        {
            Console.WriteLine("--- Сценарий А (Последовательный) ---");
            Stopwatch sw = Stopwatch.StartNew();

            await DownloadDataAsync("Сервер 1", 2000);
            await DownloadDataAsync("Сервер 2", 2000);
            await DownloadDataAsync("Сервер 3", 2000);

            sw.Stop();
            Console.WriteLine($"Общее время (Последовательно): {sw.ElapsedMilliseconds} мс (Ожидалось ~6000 мс)\n");


            Console.WriteLine("--- Сценарий Б (Параллельный) ---");
            sw.Restart();

            Task t1 = DownloadDataAsync("Сервер 1", 2000);
            Task t2 = DownloadDataAsync("Сервер 2", 2000);
            Task t3 = DownloadDataAsync("Сервер 3", 2000);

            await Task.WhenAll(t1, t2, t3);

            sw.Stop();
            Console.WriteLine($"Общее время (Параллельно): {sw.ElapsedMilliseconds} мс (Ожидалось ~2000 мс)");
        }

        static async Task DownloadDataAsync(string sourceName, int delayMs)
        {
            Console.WriteLine($"Начало загрузки с {sourceName}...");

            await Task.Delay(delayMs);

            Console.WriteLine($"Загрузка с {sourceName} завершена.");
        }
    }
}