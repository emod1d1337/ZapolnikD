namespace ClinicCRM.Models;

public class Doctor : MedicalEmployee, IExportable
{
    public string Category { get; private set; }
    public int DepartmentId { get; set; }

    public Doctor(int id, string fullName, decimal baseSalary, string category, int departmentId, MedicalLicense? license = null)
        : base(id, fullName, baseSalary, license)
    {
        Category = category;
        DepartmentId = departmentId;
    }

    public override decimal CalculateMonthlyPayout(int appointmentsCount) => BaseSalary + appointmentsCount * 450m;
    string IExportable.GetSummary() => $"Отчет врача: {FullName}; категория: {Category}; лицензия: {License.Number}";
}
