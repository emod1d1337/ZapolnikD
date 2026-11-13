namespace ObrabotkaIsklyucheniy
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("ЗАДАНИЕ 1");
            SafeDivide(10, 2);
            SafeDivide(10, 0);

            Console.WriteLine();
            Console.WriteLine("ЗАДАНИЕ 2");
            ProcessTransaction(0, 0);
            ProcessTransaction(-5, 3);
            ProcessTransaction(10, 2);

            Console.WriteLine();
            Console.WriteLine("ЗАДАНИЕ 3");
            CalculateSquareTryCatch("abc");
            CalculateSquareTryParse("abc");

            CalculateSquareTryCatch("15");
            CalculateSquareTryParse("15");
        }

        static void SafeDivide(int x, int y)
        {
            try
            {
                int result = x / y;
                Console.WriteLine($"Результат {x} / {y} = {result}");
            }
            catch (DivideByZeroException ex)
            {
                Console.WriteLine("Ошибка деления на 0: " + ex.Message);
            }
            finally
            {
                Console.WriteLine("Операция деления завершена (блок finally)");
            }
        }
        static void ProcessTransaction(int errorCode, int attempts)
        {
            Console.WriteLine($"\nВызов ProcessTransaction({errorCode}, {attempts})");

            try
            {
                int x = 1;
                int y = 0;
                int result = x / y;
                Console.WriteLine($"Результат операции: {result}");
            }
            catch (DivideByZeroException) when (attempts == 0)
            {
                Console.WriteLine("Фильтр 1: Попытки исчерпаны (attempts == 0)");
            }
            catch (DivideByZeroException ex) when (errorCode < 0)
            {
                Console.WriteLine($"Фильтр 2: Отрицательный код {errorCode}: {ex.Message}");
            }
            catch (DivideByZeroException ex)
            {
                Console.WriteLine($"Общий catch: {ex.Message}");
            }
        }
        static void CalculateSquareTryCatch(string data)
        {
            try
            {
                int value = int.Parse(data);
                Console.WriteLine($"TryCatch: Квадрат числа {value} = {value * value}");
            }
            catch (FormatException)
            {
                Console.WriteLine("TryCatch: Некорректный формат числа");
            }
        }
        static void CalculateSquareTryParse(string data)
        {
            if (int.TryParse(data, out int value))
            {
                Console.WriteLine($"TryParse: Квадрат числа {value} = {value * value}");
            }
            else
            {
                Console.WriteLine("TryParse: Некорректный ввод (без исключения)");
            }
        }
    }
}
