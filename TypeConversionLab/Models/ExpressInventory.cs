namespace TypeConversionLab.Models;

public sealed class ExpressInventory : Inventory
{
    public ExpressInventory() { }

    public ExpressInventory(int totalUnits) : base(totalUnits) { }

    // В .NET 9 / C# 13 статические операторы не являются виртуальными.
    // Чтобы сохранить «удвоенное» поведение, переопределяем + для ExpressInventory.
    public static ExpressInventory operator +(ExpressInventory left, int units)
    {
        ArgumentNullException.ThrowIfNull(left);
        if (units < 0)
            throw new ArgumentOutOfRangeException(nameof(units));

        return new ExpressInventory(checked(left.TotalUnits + units * 2));
    }
}
