namespace VeterinaryClinic.API.Dtos.Doctor;

public class DoctorUpdateDto
{
    public string Specialization { get; set; } = string.Empty;
    public string Bio { get; set; } = string.Empty;
    public string Schedule { get; set; } = "[]";
    public string Name { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
}
