namespace TypeConversionLab.Models;

public class Inventory
{
    private static int _globalCount;

    public int Id { get; }
    public int TotalUnits { get; protected set; }

    public Inventory()
    {
        Id = ++_globalCount;
    }

    public Inventory(int totalUnits) : this()
    {
        if (totalUnits < 0)
            throw new ArgumentOutOfRangeException(nameof(totalUnits));

        TotalUnits = totalUnits;
    }

    // Классический статический оператор + (для .NET 9 / C# 13).
    // Составное присваивание += компилятор синтезирует сам: a += b  ⇒  a = a + b
    public static Inventory operator +(Inventory left, int units)
    {
        ArgumentNullException.ThrowIfNull(left);
        if (units < 0)
            throw new ArgumentOutOfRangeException(nameof(units));

        return new Inventory(checked(left.TotalUnits + units));
    }

    public static Inventory operator +(Inventory left, Inventory right)
    {
        ArgumentNullException.ThrowIfNull(left);
        ArgumentNullException.ThrowIfNull(right);

        return new Inventory(checked(left.TotalUnits + right.TotalUnits));
    }

    // Унарный ++ — статический, возвращает новое значение.
    public static Inventory operator ++(Inventory inventory)
    {
        ArgumentNullException.ThrowIfNull(inventory);
        return new Inventory(checked(inventory.TotalUnits + 1));
    }
}
