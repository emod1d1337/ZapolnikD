namespace PZ7_NET9;

public sealed class Money : IEquatable<Money>
{
    private readonly long _totalKopeks;

    public int Rubles { get; }
    public int Kopeks { get; }

    public Money(int rubles, int kopeks)
    {
        _totalKopeks = checked((long)rubles * 100 + kopeks);

        long normalizedRubles = Math.DivRem(_totalKopeks, 100, out long remainder);
        if (remainder < 0)
        {
            normalizedRubles--;
            remainder += 100;
        }

        Rubles = checked((int)normalizedRubles);
        Kopeks = (int)remainder;
    }

    private Money(long totalKopeks) : this(0, checked((int)totalKopeks))
    {
    }

    public long TotalKopeks => _totalKopeks;

    public static Money operator +(Money a, Money b) =>
        new(checked(a._totalKopeks + b._totalKopeks));

    public static Money operator -(Money a, Money b) =>
        new(checked(a._totalKopeks - b._totalKopeks));

    public static Money operator +(Money money, int rubles) =>
        new(checked(money._totalKopeks + (long)rubles * 100));

    public static Money operator +(int rubles, Money money) => money + rubles;

    public static bool operator <(Money a, Money b) => a._totalKopeks < b._totalKopeks;
    public static bool operator >(Money a, Money b) => a._totalKopeks > b._totalKopeks;
    public static bool operator <=(Money a, Money b) => a._totalKopeks <= b._totalKopeks;
    public static bool operator >=(Money a, Money b) => a._totalKopeks >= b._totalKopeks;
    public static bool operator ==(Money? a, Money? b) =>
        ReferenceEquals(a, b) || (a is not null && b is not null && a._totalKopeks == b._totalKopeks);
    public static bool operator !=(Money? a, Money? b) => !(a == b);

    public static bool operator true(Money money) => money._totalKopeks > 0;
    public static bool operator false(Money money) => money._totalKopeks <= 0;
    public static bool operator !(Money money) => money._totalKopeks <= 0;

    public static explicit operator long(Money money) => money._totalKopeks;
    public static implicit operator Money(long totalKopeks) => new(totalKopeks);

    public static explicit operator CurrencyRecord(Money money) =>
        new("RUB", money._totalKopeks / 100m);

    public static explicit operator Money(CurrencyRecord record)
    {
        decimal roundedKopeks = decimal.Round(record.Amount * 100m, 0, MidpointRounding.AwayFromZero);
        return new Money(checked((long)roundedKopeks));
    }

    public bool Equals(Money? other) => other is not null && _totalKopeks == other._totalKopeks;
    public override bool Equals(object? obj) => obj is Money other && Equals(other);
    public override int GetHashCode() => _totalKopeks.GetHashCode();

    public override string ToString()
    {
        decimal amount = _totalKopeks / 100m;
        return $"{amount:N2} руб.";
    }
}
