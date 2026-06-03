namespace VeterinaryClinic.API.Dtos.Client;

public class ClientUpdateDto
{
    public string Address { get; set; } = string.Empty;
    // Poate actualiza si datele din User
    public string Name { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
}
