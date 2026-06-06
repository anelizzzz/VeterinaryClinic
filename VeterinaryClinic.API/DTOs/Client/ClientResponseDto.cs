namespace VeterinaryClinic.API.Dtos.Client;

public class ClientResponseDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Address { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public bool IsApproved { get; set; }
}