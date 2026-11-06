namespace ClinicCRM.Models;

public sealed class Appointment
{
    public int Id { get; set; }
    public int DoctorId { get; set; }
    public string PatientName { get; set; } = "";
    public decimal Cost { get; set; }
    public DateTime Date { get; set; }
    public string Status { get; set; } = "Planned";
}

public sealed class AppointmentViewDto
{
    public string Patient { get; set; } = "";
    public string Doctor { get; set; } = "";
    public string Department { get; set; } = "";
    public decimal Cost { get; set; }
    public string Date { get; set; } = "";
    public string Status { get; set; } = "";
}

public sealed class AppointmentEventArgs : EventArgs
{
    public string DoctorName { get; }
    public string PatientName { get; }
    public decimal Cost { get; }
    public string Message { get; }
    public AppointmentEventArgs(string doctorName, string patientName, decimal cost, string message)
    { DoctorName = doctorName; PatientName = patientName; Cost = cost; Message = message; }
}
