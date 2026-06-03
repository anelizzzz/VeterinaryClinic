using Microsoft.EntityFrameworkCore;
using VeterinaryClinic.API.Dtos.Appointment;
using VeterinaryClinic.API.DTOs.Appointment;
using VeterinaryClinic.API.Models.Entities;
using VeterinaryClinic.API.Models.Entities.Enums;

namespace VeterinaryClinic.API.Services
{
    public class AppointmentService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AppointmentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<AppointmentResponseDto>> GetAllAsync()
        {
            var appointments = await _unitOfWork.Appointments.GetAllAsync();
            return appointments.Select(a => new AppointmentResponseDto
            {
                Id = a.Id,
                Date = a.Date,
                Status = a.Status,
                Type = a.Type,
                Notes = a.Notes,
                PetName = a.Pet?.Name ?? string.Empty,
                DoctorName = a.Doctor?.User?.Name ?? string.Empty,
                ClientName = a.Client?.User?.Name ?? string.Empty
            });
        }

        public async Task<AppointmentResponseDto?> GetByIdAsync(int id)
        {
            var appointment = await _unitOfWork.Appointments.GetByIdAsync(id);
            if (appointment is null) return null;

            return new AppointmentResponseDto
            {
                Id = appointment.Id,
                Date = appointment.Date,
                Status = appointment.Status,
                Type = appointment.Type,
                Notes = appointment.Notes,
                PetName = appointment.Pet?.Name ?? string.Empty,
                DoctorName = appointment.Doctor?.User?.Name ?? string.Empty,
                ClientName = appointment.Client?.User?.Name ?? string.Empty
            };
        }

        public async Task<IEnumerable<AppointmentResponseDto>> GetByClientIdAsync(int clientId)
        {
            var appointments = await _unitOfWork.Appointments.GetAllAsync();
            return appointments
                .Where(a => a.ClientId == clientId)
                .Select(a => new AppointmentResponseDto
                {
                    Id = a.Id,
                    Date = a.Date,
                    Status = a.Status,
                    Type = a.Type,
                    Notes = a.Notes,
                    PetName = a.Pet?.Name ?? string.Empty,
                    DoctorName = a.Doctor?.User?.Name ?? string.Empty,
                    ClientName = a.Client?.User?.Name ?? string.Empty
                });
        }

        public async Task<IEnumerable<AppointmentResponseDto>> GetByDoctorIdAsync(int doctorId)
        {
            var appointments = await _unitOfWork.Appointments.GetAllAsync();
            return appointments
                .Where(a => a.DoctorId == doctorId)
                .Select(a => new AppointmentResponseDto
                {
                    Id = a.Id,
                    Date = a.Date,
                    Status = a.Status,
                    Type = a.Type,
                    Notes = a.Notes,
                    PetName = a.Pet?.Name ?? string.Empty,
                    DoctorName = a.Doctor?.User?.Name ?? string.Empty,
                    ClientName = a.Client?.User?.Name ?? string.Empty
                });
        }
        public async Task<AppointmentDetailsDto?> GetAppointmentDetailsAsync(int id)
        {
            var appointment = await _unitOfWork.Appointments.GetByIdWithDetailsAsync(id);
            if (appointment == null)
                return null;

            return new AppointmentDetailsDto
            {
                Id = appointment.Id,
                Date = appointment.Date,
                Status = (int)appointment.Status,
                Type = (int)appointment.Type,
                Notes = appointment.Notes ?? string.Empty,

                Client = new AppointmentClientDetailsDto
                {
                    Id = appointment.Client.Id,
                    Name = appointment.Client.User.Name,
                    Email = appointment.Client.User.Email,
                    Phone = appointment.Client.User.Phone,
                    Address = appointment.Client.Address
                },

                Pet = new AppointmentPetDetailsDto
                {
                    Id = appointment.Pet.Id,
                    Name = appointment.Pet.Name,
                    Species = appointment.Pet.Species,
                    Breed = appointment.Pet.Breed,
                    Birthdate = appointment.Pet.Birthdate,
                    Vaccines = appointment.Pet.Vaccines ?? string.Empty,
                    ImageUrl = appointment.Pet.ImageUrl ?? string.Empty,
                    Status = (int)appointment.Pet.Status
                },

                Doctor = new AppointmentDoctorDetailsDto
                {
                    Id = appointment.Doctor.Id,
                    Name = appointment.Doctor.User.Name,
                    Email = appointment.Doctor.User.Email,
                    Specialization = appointment.Doctor.Specialization
                }
            };
        }
        public async Task<AppointmentResponseDto> CreateAsync(AppointmentCreateDto dto)
        {
            var appointment = new Appointment
            {
                Date = dto.Date,
                Type = dto.Type,
                Notes = dto.Notes,
                Status = AppointmentStatus.Pending,
                ClientId = dto.ClientId,
                DoctorId = dto.DoctorId,
                PetId = dto.PetId
            };

            await _unitOfWork.Appointments.AddAsync(appointment);

            return new AppointmentResponseDto
            {
                Id = appointment.Id,
                Date = appointment.Date,
                Status = appointment.Status,
                Type = appointment.Type,
                Notes = appointment.Notes
            };
        }

        public async Task<AppointmentResponseDto?> UpdateAsync(int id, AppointmentUpdateDto dto)
        {
            var appointment = await _unitOfWork.Appointments.GetByIdAsync(id);
            if (appointment is null) return null;

            var doctorExists = await _unitOfWork.Doctors.ExistsAsync(dto.DoctorId);
            if (!doctorExists)
                throw new Exception("Doctorul selectat nu există.");

            appointment.Date = dto.Date;
            appointment.Status = dto.Status;
            appointment.Type = dto.Type;
            appointment.Notes = dto.Notes;
            appointment.DoctorId = dto.DoctorId;

            await _unitOfWork.Appointments.UpdateAsync(appointment);

            var updatedAppointment = await _unitOfWork.Appointments.GetByIdWithDetailsAsync(id);
            if (updatedAppointment is null) return null;

            return new AppointmentResponseDto
            {
                Id = updatedAppointment.Id,
                Date = updatedAppointment.Date,
                Status = updatedAppointment.Status,
                Type = updatedAppointment.Type,
                Notes = updatedAppointment.Notes,
                PetName = updatedAppointment.Pet?.Name ?? string.Empty,
                DoctorName = updatedAppointment.Doctor?.User?.Name ?? string.Empty,
                ClientName = updatedAppointment.Client?.User?.Name ?? string.Empty
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var appointment = await _unitOfWork.Appointments.GetByIdAsync(id);
            if (appointment is null) return false;

            await _unitOfWork.Appointments.DeleteAsync(id);
            return true;
        }
    }
}