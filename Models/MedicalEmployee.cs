namespace ClinicCRM.Models;

public abstract class MedicalEmployee : IComparable, ICloneable
{
    public int Id { get; private set; }
    public string FullName { get; private set; }
    public decimal BaseSalary { get; private set; }
    public MedicalLicense License { get; set; }

    protected MedicalEmployee(int id, string fullName, decimal baseSalary, MedicalLicense? license = null)
    {
        Id = id;
        FullName = fullName;
        BaseSalary = baseSalary;
        License = license ?? new MedicalLicense("LIC-" + id.ToString("000"), DateTime.Today.AddYears(2));
    }

    public virtual string GetInfo() => $"{FullName} | оклад: {BaseSalary:N0}";
    public abstract decimal CalculateMonthlyPayout(int appointmentsCount);
    public override string ToString() => $"#{Id} {FullName} ({GetType().Name})";
    public override bool Equals(object? obj) => obj is MedicalEmployee other && Id == other.Id;
    public override int GetHashCode() => Id.GetHashCode();
    public int CompareTo(object? obj)
    {
        if (obj is not MedicalEmployee other) throw new ArgumentException("Объект должен быть MedicalEmployee");
        return other.BaseSalary.CompareTo(BaseSalary);
    }
    public virtual object Clone()
    {
        var clone = (MedicalEmployee)MemberwiseClone();
        clone.License = new MedicalLicense(License.Number, License.ValidUntil);
        return clone;
    }
}
