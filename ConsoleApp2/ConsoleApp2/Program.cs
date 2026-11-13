namespace ExceptionPractice
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("===== ЗАДАНИЕ 1 =====");

            TestRegistration("Alex", 20);
            TestRegistration("A", 20);
            TestRegistration("Alex", 15);
            TestRegistration("Alex", 121);

            Console.WriteLine("\n===== ЗАДАНИЕ 2 =====");

            try
            {
                PaymentProcessor.ExecutePayment();
            }
            catch (Exception ex)
            {
                Console.WriteLine("[Main] Ошибка: " + ex.Message);
                Console.WriteLine("Трассировка стека:");
                Console.WriteLine(ex.StackTrace);
            }

            Console.WriteLine("\n===== ЗАДАНИЕ 3 =====");

            TestParser(new string[] { "100", "25" }, 1, 5);
            TestParser(new string[] { "100", "abc" }, 1, 2);
            TestParser(new string[] { "100" }, 5, 2);
            TestParser(new string[] { "100" }, 0, 0);
            TestParser(null, 0, 2);

            Console.WriteLine("\nВсе задания выполнены!");
            Console.ReadKey();
        }

        static void TestRegistration(string username, int age)
        {
            try
            {
                UserRegistration.RegisterUser(username, age);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Console.WriteLine("[Ошибка возраста]: " + ex.Message);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine("[Ошибка имени]: " + ex.Message);
            }
        }

        static void TestParser(string[] data, int index, int divisor)
        {
            try
            {
                int result = DataParser.ParseAndDivide(data, index, divisor);
                Console.WriteLine("Результат: " + result);
            }
            catch (DivideByZeroException)
            {
                Console.WriteLine("Деление на ноль!");
            }
            catch (FormatException)
            {
                Console.WriteLine("Неверный формат числа!");
            }
            catch (IndexOutOfRangeException)
            {
                Console.WriteLine("Индекс вне диапазона!");
            }
            catch (NullReferenceException)
            {
                Console.WriteLine("Массив data равен null!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Общая ошибка: " + ex.Message);
            }
        }
    }
    public class UserRegistration
    {
        public static void RegisterUser(string username, int age)
        {
            if (username == null || username.Length < 2)
            {
                throw new ArgumentException(
                    "Имя пользователя должно содержать не менее 2 символов");
            }

            if (age < 18 || age > 120)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(age),
                    "Возраст должен быть в диапазоне от 18 до 120 лет");
            }

            Console.WriteLine(
                $"Пользователь {username} успешно зарегистрирован. Возраст: {age}");
        }
    }
    public class PaymentProcessor
    {
        private static void ProcessTransaction()
        {
            throw new InvalidOperationException(
                "Сбой соединения с сервером");
        }

        public static void ExecutePayment()
        {
            try
            {
                ProcessTransaction();
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    "[Лог сервиса]: " + ex.Message);

                throw;
            }
        }
    }
    public class DataParser
    {
        public static int ParseAndDivide(
            string[] data, int index, int divisor)
        {
            if (data == null)
            {
                throw new NullReferenceException(
                    "Массив data равен null");
            }

            if (index < 0 || index >= data.Length)
            {
                throw new IndexOutOfRangeException(
                    "Индекс вне диапазона массива");
            }

            int number = int.Parse(data[index]);

            return number / divisor;
        }
    }
}
