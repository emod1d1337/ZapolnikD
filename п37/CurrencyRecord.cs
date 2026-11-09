namespace PZ7_NET9;

public sealed class CurrencyRecord
{
    public string Code { get; set; }
    public decimal Amount { get; set; }

    public CurrencyRecord(string code, decimal amount)
    {
        Code = code;
        Amount = amount;
    }

    public override string ToString() => $"{Amount:N2} {Code}";
}
