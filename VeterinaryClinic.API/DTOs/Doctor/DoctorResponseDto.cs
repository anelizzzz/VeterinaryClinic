namespace VeterinaryClinic.API.Dtos.Doctor;

public class DoctorResponseDto
{
    public int Id { get; set; }
    public string Specialization { get; set; } = string.Empty;
    public string Bio { get; set; } = string.Empty;
    public string Schedule { get; set; } = "[]";
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
}
