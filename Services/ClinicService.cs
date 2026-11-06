using Microsoft.Data.Sqlite;
using ClinicCRM.Models;

namespace ClinicCRM.Services;

public sealed class ClinicService
{
    public event EventHandler<AppointmentEventArgs>? AppointmentRegistered;
    public Action<string>? Logger { get; set; }

    public List<Doctor> GetDoctors() => new SqliteRepository<Doctor>(
        "SELECT Id,FullName,DepartmentId,Category,BaseSalary FROM Doctors",
        r => new Doctor(r.GetInt32(0), r.GetString(1), Convert.ToDecimal(r.GetDouble(4)), r.GetString(3), r.GetInt32(2)),
        (c,d) => { c.CommandText="INSERT INTO Doctors(FullName,DepartmentId,Category,BaseSalary) VALUES($n,$dep,$cat,$sal)"; c.Parameters.AddWithValue("$n",d.FullName); c.Parameters.AddWithValue("$dep",d.DepartmentId); c.Parameters.AddWithValue("$cat",d.Category); c.Parameters.AddWithValue("$sal",d.BaseSalary); }).GetAll();

    public List<Appointment> GetAppointments()
    {
        using var c = Database.OpenConnection(); using var cmd = c.CreateCommand();
        cmd.CommandText="SELECT Id,DoctorId,PatientName,Cost,Date,Status FROM Appointments";
        using var r=cmd.ExecuteReader(); var list=new List<Appointment>();
        while(r.Read()) list.Add(new Appointment { Id=r.GetInt32(0), DoctorId=r.GetInt32(1), PatientName=r.GetString(2), Cost=Convert.ToDecimal(r.GetDouble(3)), Date=DateTime.Parse(r.GetString(4)), Status=r.GetString(5) });
        return list;
    }

    public List<(int Id,string Title)> GetDepartments()
    {
        using var c=Database.OpenConnection(); using var cmd=c.CreateCommand(); cmd.CommandText="SELECT Id,Title FROM Departments ORDER BY Title";
        using var r=cmd.ExecuteReader(); var list=new List<(int,string)>(); while(r.Read()) list.Add((r.GetInt32(0),r.GetString(1))); return list;
    }

    public void RegisterAppointment(int doctorId, string patient, decimal cost, DateTime date)
    {
        using var c=Database.OpenConnection(); using var cmd=c.CreateCommand();
        cmd.CommandText="INSERT INTO Appointments(DoctorId,PatientName,Cost,Date,Status) VALUES($d,$p,$c,$date,'Planned')";
        cmd.Parameters.AddWithValue("$d",doctorId); cmd.Parameters.AddWithValue("$p",patient); cmd.Parameters.AddWithValue("$c",cost); cmd.Parameters.AddWithValue("$date",date.ToString("yyyy-MM-dd")); cmd.ExecuteNonQuery();
        var doctor=GetDoctors().FirstOrDefault(x=>x.Id==doctorId);
        AppointmentRegistered?.Invoke(this,new AppointmentEventArgs(doctor?.FullName ?? "Неизвестно",patient,cost,"Прием зарегистрирован"));
        Logger?.Invoke($"Создан прием: {patient}");
    }

    public Func<Appointment,bool> CreateBudgetFilter(decimal minCost) => a => a.Cost >= minCost;

    public List<Appointment> GetCompletedAppointments()
    {
        Predicate<Appointment> isCompleted = a => a.Status == "Completed";
        return GetAppointments().Where(a => isCompleted(a)).ToList();
    }

    public Func<Appointment, decimal> CreateDiscountCalculator(decimal discountRate)
        => appointment => appointment.Cost * (1m - discountRate);

    public List<AppointmentViewDto> GetAppointmentDtos()
    {
        var appointments=GetAppointments(); var doctors=GetDoctors(); var departments=GetDepartments();
        return (from a in appointments
                join d in doctors on a.DoctorId equals d.Id
                join dep in departments on d.DepartmentId equals dep.Id
                select new AppointmentViewDto { Patient=a.PatientName, Doctor=d.FullName, Department=dep.Title, Cost=a.Cost, Date=a.Date.ToString("dd.MM.yyyy"), Status=a.Status }).ToList();
    }

    public object GetDoctorAggregates() => GetAppointments().GroupBy(a=>a.DoctorId)
        .Select(g=>new { Doctor=GetDoctors().FirstOrDefault(d=>d.Id==g.Key)?.FullName ?? "Неизвестно", Count=g.Count(), Revenue=g.Sum(a=>a.Cost), Average=g.Average(a=>a.Cost) }).ToList();

    public List<AppointmentViewDto> GetCompletedSorted() => GetAppointmentDtos().Where(a=>a.Status=="Completed").OrderBy(a=>a.Department).ThenByDescending(a=>a.Cost).ToList();

    public (List<string> Both,List<string> TherapyOnly) GetPatientSets()
    {
        var rows=GetAppointmentDtos(); var therapy=rows.Where(x=>x.Department=="Терапия").Select(x=>x.Patient).Distinct();
        var surgery=rows.Where(x=>x.Department=="Хирургия").Select(x=>x.Patient).Distinct();
        return (therapy.Intersect(surgery).ToList(),therapy.Except(surgery).ToList());
    }

    public List<AppointmentViewDto> GetPage(int page,int pageSize) => GetAppointmentDtos().OrderByDescending(x=>x.Date).Skip((page-1)*pageSize).Take(pageSize).ToList();
}
