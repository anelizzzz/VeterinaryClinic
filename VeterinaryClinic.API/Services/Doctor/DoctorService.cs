using VeterinaryClinic.API.Dtos.Doctor;
using VeterinaryClinic.API.Models.Entities;

namespace VeterinaryClinic.API.Services.Doctor
{
    public class DoctorService
    {
        private readonly IUnitOfWork _unitOfWork;

        public DoctorService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<DoctorResponseDto>> GetAllAsync()
        {
            var doctors = await _unitOfWork.Doctors.GetAllAsync();
            return doctors.Select(d => new DoctorResponseDto
            {
                Id = d.Id,
                Specialization = d.Specialization,
                Bio = d.Bio,
                Schedule = d.Schedule,
                Name = d.User?.Name ?? string.Empty,
                Email = d.User?.Email ?? string.Empty,
                Phone = d.User?.Phone ?? string.Empty
            });
        }

        public async Task<DoctorResponseDto?> GetByIdAsync(int id)
        {
            var doctor = await _unitOfWork.Doctors.GetByIdAsync(id);
            if (doctor is null) return null;

            return new DoctorResponseDto
            {
                Id = doctor.Id,
                Specialization = doctor.Specialization,
                Bio = doctor.Bio,
                Schedule = doctor.Schedule,
                Name = doctor.User?.Name ?? string.Empty,
                Email = doctor.User?.Email ?? string.Empty,
                Phone = doctor.User?.Phone ?? string.Empty
            };
        }

        public async Task<DoctorResponseDto?> GetByUserIdAsync(int userId)
        {
            var doctors = await _unitOfWork.Doctors.GetAllAsync();
            var doctor = doctors.FirstOrDefault(d => d.UserId == userId);
            if (doctor is null) return null;

            return new DoctorResponseDto
            {
                Id = doctor.Id,
                Specialization = doctor.Specialization,
                Bio = doctor.Bio,
                Schedule = doctor.Schedule,
                Name = doctor.User?.Name ?? string.Empty,
                Email = doctor.User?.Email ?? string.Empty,
                Phone = doctor.User?.Phone ?? string.Empty
            };
        }
        private int CalculateAge(DateTime birthdate)
        {
            var today = DateTime.Today;
            var age = today.Year - birthdate.Year;

            if (birthdate.Date > today.AddYears(-age))
                age--;

            return age < 0 ? 0 : age;
        }
        public async Task<IEnumerable<DoctorPatientDto>> GetMyPatientsAsync(int userId)
        {
            var doctor = await _unitOfWork.Doctors.GetByUserIdAsync(userId);
            if (doctor == null)
                return Enumerable.Empty<DoctorPatientDto>();

            var pets = await _unitOfWork.Appointments.GetDistinctPetsByDoctorIdAsync(doctor.Id);

            return pets.Select(p => new DoctorPatientDto
            {
                Id = p.Id,
                Name = p.Name,
                Species = p.Species,
                Breed = p.Breed,
                Age = CalculateAge(p.Birthdate),
                OwnerName = p.Client != null && p.Client.User != null
                    ? p.Client.User.Name
                    : string.Empty
            });
        }
        public async Task<DoctorPatientDetailsDto?> GetPatientDetailsAsync(int petId)
        {
            var pet = await _unitOfWork.Pets.GetByIdAsync(petId);
            if (pet == null)
                return null;

            return new DoctorPatientDetailsDto
            {
                Id = pet.Id,
                Name = pet.Name,
                Species = pet.Species,
                Breed = pet.Breed,
                Birthdate = pet.Birthdate,
                Status = pet.Status,
                Vaccines = pet.Vaccines,
                ImageUrl = pet.ImageUrl,
                ClientId = pet.ClientId,
                OwnerName = pet.Client != null && pet.Client.User != null
                    ? pet.Client.User.Name
                    : string.Empty
            };
        }
        public async Task<DoctorResponseDto?> UpdateAsync(int id, DoctorUpdateDto dto)
        {
            var doctor = await _unitOfWork.Doctors.GetByIdAsync(id);
            if (doctor is null) return null;

            doctor.Specialization = dto.Specialization;
            doctor.Bio = dto.Bio;
            doctor.Schedule = dto.Schedule;

            if (doctor.User is not null)
            {
                doctor.User.Name = dto.Name;
                doctor.User.Phone = dto.Phone;
            }

            await _unitOfWork.Doctors.UpdateAsync(doctor);

            return new DoctorResponseDto
            {
                Id = doctor.Id,
                Specialization = doctor.Specialization,
                Bio = doctor.Bio,
                Schedule = doctor.Schedule,
                Name = doctor.User?.Name ?? string.Empty,
                Email = doctor.User?.Email ?? string.Empty,
                Phone = doctor.User?.Phone ?? string.Empty
            };
        }
        public async Task<DoctorResponseDto?> UpdateByUserIdAsync(int userId, DoctorUpdateDto dto)
        {
            var doctor = await _unitOfWork.Doctors.GetByUserIdAsync(userId);
            if (doctor == null) return null;

            doctor.User.Name = dto.Name;
            doctor.User.Phone = dto.Phone;
            doctor.Specialization = dto.Specialization;
            doctor.Bio = dto.Bio;
            doctor.Schedule = dto.Schedule;

            await _unitOfWork.SaveChangesAsync();

            return new DoctorResponseDto
            {
                Id = doctor.Id,
                Name = doctor.User.Name,
                Email = doctor.User.Email,
                Phone = doctor.User.Phone,
                Specialization = doctor.Specialization,
                Bio = doctor.Bio,
                Schedule = doctor.Schedule
            };
        }
        public async Task<bool> DeleteAsync(int id)
        {
            var doctor = await _unitOfWork.Doctors.GetByIdAsync(id);
            if (doctor is null) return false;

            await _unitOfWork.Doctors.DeleteAsync(id);
            return true;
        }
    }
}
