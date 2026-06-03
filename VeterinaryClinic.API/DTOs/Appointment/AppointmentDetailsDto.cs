namespace VeterinaryClinic.API.DTOs.Appointment
{
    public class AppointmentDetailsDto
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int Status { get; set; }
        public int Type { get; set; }
        public string Notes { get; set; } = string.Empty;

        public AppointmentClientDetailsDto Client { get; set; } = new();
        public AppointmentPetDetailsDto Pet { get; set; } = new();
        public AppointmentDoctorDetailsDto Doctor { get; set; } = new();
    }
}
