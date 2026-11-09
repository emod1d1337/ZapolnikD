namespace PZ7_NET9;

public sealed class PremiumAccount : AccountBalance
{
    public PremiumAccount(decimal totalBalance) : base(totalBalance) { }
    public override void AddInPlace(decimal amount)
    {
        decimal cashback = amount > 0 ? amount * 0.05m : 0m;
        TotalBalance += amount + cashback;
    }
}
