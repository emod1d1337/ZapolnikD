namespace TypeConversionLab.Models;

public sealed class StorageSize
{
    public long Bytes { get; set; }

    public static implicit operator StorageSize(long bytes) =>
        new() { Bytes = bytes };

    public static explicit operator long(StorageSize size) =>
        size.Bytes;

    public static implicit operator MegaByte(StorageSize size) =>
        new() { Value = size.Bytes / 1_048_576.0 };

    public static explicit operator StorageSize(MegaByte megaByte)
    {
        ArgumentNullException.ThrowIfNull(megaByte);

        if (!double.IsFinite(megaByte.Value))
            throw new ArgumentOutOfRangeException(nameof(megaByte), "Размер должен быть конечным числом.");

        double bytes = megaByte.Value * 1_048_576.0;
        if (bytes < 0 || bytes > long.MaxValue)
            throw new ArgumentOutOfRangeException(nameof(megaByte), "Размер выходит за допустимый диапазон.");

        return new StorageSize { Bytes = checked((long)Math.Round(bytes)) };
    }

    public override string ToString() => $"{Bytes:N0} байт";
}
