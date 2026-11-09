namespace PZ7_NET9;

public class AccountBalance
{
    private static int _nextId;

    public int Id { get; }
    public decimal TotalBalance { get; protected set; }

    public AccountBalance(decimal totalBalance)
    {
        Id = Interlocked.Increment(ref _nextId);
        TotalBalance = totalBalance;
    }
    public static AccountBalance operator +(AccountBalance account, decimal amount) =>
        new(account.TotalBalance + amount);

    public static AccountBalance operator +(AccountBalance first, AccountBalance second) =>
        new(first.TotalBalance + second.TotalBalance);

    public virtual void AddInPlace(decimal amount) => TotalBalance += amount;

    public void MergeInPlace(AccountBalance other) => TotalBalance += other.TotalBalance;

    public static AccountBalance operator ++(AccountBalance account) =>
        new(account.TotalBalance + 1m);

    public override string ToString() => $"Счёт #{Id}: {TotalBalance:N2} руб.";
}
