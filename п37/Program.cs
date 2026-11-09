namespace PZ7_NET9;

internal class Program
{
    private static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        Console.WriteLine("========== ЗАДАНИЕ 1. MONEY ==========");
        Money m1 = new Money(150, 80);
        Money m2 = new Money(50, 40);

        Console.WriteLine($"m1 = {m1}");
        Console.WriteLine($"m2 = {m2}");
        Console.WriteLine($"m1 + m2 = {m1 + m2}");
        Console.WriteLine($"m1 - m2 = {m1 - m2}");
        Console.WriteLine($"m1 + 100 рублей = {m1 + 100}");
        Console.WriteLine($"m1 > m2: {m1 > m2}");
        Console.WriteLine($"m1 == m2: {m1 == m2}");
        if (m1) Console.WriteLine("m1 активен (баланс больше нуля).");
        if (!new Money(0, 0)) Console.WriteLine("Нулевой баланс неактивен.");

        Money normalized = new Money(10, 175);
        Console.WriteLine($"Нормализация 10 руб. 175 коп. = {normalized}");

        Console.WriteLine();
        Console.WriteLine("========== ЗАДАНИЕ 2. ПРЕОБРАЗОВАНИЯ ==========");
        Money fromLong = 12550L; // implicit: long (копейки) -> Money
        long allKopeks = (long)fromLong; // explicit: Money -> long
        CurrencyRecord record = (CurrencyRecord)fromLong;
        Money fromRecord = (Money)record;

        Console.WriteLine($"Money из 12550 копеек: {fromLong}");
        Console.WriteLine($"Явное преобразование в long: {allKopeks}");
        Console.WriteLine($"CurrencyRecord: {record}");
        Console.WriteLine($"Money обратно из CurrencyRecord: {fromRecord}");

        Console.WriteLine();
        Console.WriteLine("========== ЗАДАНИЕ 3. ACCOUNT BALANCE ==========");
        AccountBalance account = new AccountBalance(1000m);
        int initialId = account.Id;
        Console.WriteLine($"Начальный объект: {account}");

        for (int i = 0; i < 10000; i++)
            account += 10m;

        Console.WriteLine($"После 10000 операций += 10: {account}");
        Console.WriteLine($"Id изменился: {initialId != account.Id} (в .NET 9 operator + создаёт новый объект)");

        account.AddInPlace(50m);
        Console.WriteLine($"После AddInPlace(50): {account}");

        AccountBalance another = new AccountBalance(200m);
        account.MergeInPlace(another);
        Console.WriteLine($"После объединения счетов на месте: {account}");

        AccountBalance incremented = ++account;
        Console.WriteLine($"Результат ++account: {incremented}");

        PremiumAccount premium = new PremiumAccount(500m);
        premium.AddInPlace(100m);
        Console.WriteLine($"Premium после пополнения 100 руб. с кэшбэком 5%: {premium}");

        Console.WriteLine();
        Console.WriteLine("Программа завершена.");
    }
}
