using TypeConversionLab.Models;

Console.OutputEncoding = System.Text.Encoding.UTF8;

Console.WriteLine("=== Задание 1. Преобразование типов ===");
StorageSize size1 = 2_097_152L;
long rawBytes = (long)size1;
MegaByte mb = size1;
StorageSize size2 = (StorageSize)new MegaByte { Value = 1.5 };

Console.WriteLine($"size1: {size1}");
Console.WriteLine($"Явное преобразование в long: {rawBytes}");
Console.WriteLine($"Неявное преобразование в MegaByte: {mb}");
Console.WriteLine($"Явное преобразование 1.5 МБ: {size2}");

Console.WriteLine();
Console.WriteLine("=== Задание 2. Составные операторы += ===");
var warehouse = new Inventory(100);
Console.WriteLine($"Id до операций: {warehouse.Id}, товаров: {warehouse.TotalUnits}");

warehouse += 50;
warehouse += 25;
var anotherWarehouse = new Inventory(30);
warehouse += anotherWarehouse;

Console.WriteLine($"Id после операций: {warehouse.Id}, товаров: {warehouse.TotalUnits}");
Console.WriteLine("Внимание: в .NET 9 оператор + создаёт новый объект, поэтому Id изменился.");

Console.WriteLine();
Console.WriteLine("=== Задание 3. Инкремент и полиморфизм ===");
Inventory ordinary = new Inventory(10);
Inventory next = ++ordinary;
Console.WriteLine($"Обычный склад: Id={ordinary.Id}, товаров={ordinary.TotalUnits}");
Console.WriteLine($"Результат ++: Id={next.Id}, товаров={next.TotalUnits}");

ExpressInventory express = new ExpressInventory(10);
express += 5;
Console.WriteLine($"Срочный склад после += 5: Id={express.Id}, товаров={express.TotalUnits}");

Console.WriteLine();
Console.WriteLine("=== Контроль преобразования ===");
StorageSize fractional = (StorageSize)new MegaByte { Value = 0.1 };
Console.WriteLine($"0.1 МБ после округления: {fractional.Bytes} байт");
