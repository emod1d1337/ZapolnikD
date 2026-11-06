namespace ClinicCRM.Models;

public sealed class MedicalLicense
{
    public string Number { get; set; }
    public DateTime ValidUntil { get; set; }

    public MedicalLicense(string number, DateTime validUntil)
    {
        Number = number;
        ValidUntil = validUntil;
    }
}
