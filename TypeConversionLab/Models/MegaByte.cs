namespace TypeConversionLab.Models;

public sealed class MegaByte
{
    public double Value { get; set; }

    public override string ToString() => $"{Value:N6} МБ";
}
