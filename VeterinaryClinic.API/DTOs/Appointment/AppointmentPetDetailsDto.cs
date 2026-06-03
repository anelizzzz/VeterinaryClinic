namespace VeterinaryClinic.API.DTOs.Appointment
{
    public class AppointmentPetDetailsDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Species { get; set; } = string.Empty;
        public string Breed { get; set; } = string.Empty;
        public DateTime? Birthdate { get; set; }
        public string Vaccines { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public int Status { get; set; }
    }
}
