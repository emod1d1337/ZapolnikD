namespace ClinicCRM.Models;

public sealed class Surgeon : Doctor
{
    private readonly int _complexOperationsExperience;
    public Surgeon(int id, string fullName, decimal baseSalary, string category, int departmentId, int complexOperationsExperience, MedicalLicense? license = null)
        : base(id, fullName, baseSalary, category, departmentId, license) => _complexOperationsExperience = complexOperationsExperience;

    public override decimal CalculateMonthlyPayout(int appointmentsCount)
    {
        decimal categoryBonus = Category == "Высшая" ? 15000m : 7000m;
        return BaseSalary + appointmentsCount * 1200m + categoryBonus + _complexOperationsExperience * 500m;
    }
    public override string GetInfo() => base.GetInfo() + $" | стаж сложных операций: {_complexOperationsExperience} лет";
}
