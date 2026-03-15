namespace VeterinaryClinic.API.Models.Entities;

public class Client
{
    public int Id { get; set; }
    public string Address { get; set; } = string.Empty;
    public int  UserId { get; set; }
    public User User { get; set; } = null!;
    public ICollection<Pet> Pets { get; set; } = new List<Pet>();
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    public ICollection<AdoptionRequest> AdoptionRequests = new List<AdoptionRequest>();
}
