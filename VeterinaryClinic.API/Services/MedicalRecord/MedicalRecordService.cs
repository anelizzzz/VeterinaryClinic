using VeterinaryClinic.API.DTOs.MedicalRecord;
using VeterinaryClinic.API.Models.Entities;

namespace VeterinaryClinic.API.Services
{
    public class MedicalRecordService
    {
        private readonly IUnitOfWork _unitOfWork;

        public MedicalRecordService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<MedicalRecordResponseDto>> GetAllAsync()
        {
            var records = await _unitOfWork.MedicalRecords.GetAllAsync();
            return records.Select(m => new MedicalRecordResponseDto
            {
                Id = m.Id,
                Date = m.Date,
                PetName = m.Pet?.Name ?? string.Empty,
                DoctorName = m.Doctor?.User?.Name ?? string.Empty,
                Diagnosis = m.Diagnosis,
                Treatment = m.Treatment,
                Observations = m.Observations
            });
        }

        public async Task<MedicalRecordResponseDto?> GetByIdAsync(int id)
        {
            var record = await _unitOfWork.MedicalRecords.GetByIdAsync(id);
            if (record is null) return null;

            return new MedicalRecordResponseDto
            {
                Id = record.Id,
                Date = record.Date,
                PetName = record.Pet?.Name ?? string.Empty,
                DoctorName = record.Doctor?.User?.Name ?? string.Empty,
                Diagnosis = record.Diagnosis,
                Treatment = record.Treatment,
                Observations = record.Observations
            };
        }

        public async Task<IEnumerable<MedicalRecordResponseDto>> GetByPetIdAsync(int petId)
        {
            var records = await _unitOfWork.MedicalRecords.GetAllAsync();
            return records
                .Where(m => m.PetId == petId)
                .Select(m => new MedicalRecordResponseDto
                {
                    Id = m.Id,
                    Date = m.Date,
                    PetName = m.Pet?.Name ?? string.Empty,
                    DoctorName = m.Doctor?.User?.Name ?? string.Empty,
                    Diagnosis = m.Diagnosis,
                    Treatment = m.Treatment,
                    Observations = m.Observations
                });
        }

        public async Task<IEnumerable<MedicalRecordResponseDto>> GetByDoctorIdAsync(int doctorId)
        {
            var records = await _unitOfWork.MedicalRecords.GetAllAsync();
            return records
                .Where(m => m.DoctorId == doctorId)
                .Select(m => new MedicalRecordResponseDto
                {
                    Id = m.Id,
                    Date = m.Date,
                    PetName = m.Pet?.Name ?? string.Empty,
                    DoctorName = m.Doctor?.User?.Name ?? string.Empty,
                    Diagnosis = m.Diagnosis,
                    Treatment = m.Treatment,
                    Observations = m.Observations
                });
        }

        public async Task<MedicalRecordResponseDto> CreateAsync(MedicalRecordCreateDto dto)
        {
            var record = new MedicalRecord
            {
                PetId = dto.PetId,
                DoctorId = dto.DoctorId,
                Diagnosis = dto.Diagnosis,
                Treatment = dto.Treatment,
                Observations = dto.Observations,
                Date = DateTime.UtcNow
            };

            await _unitOfWork.MedicalRecords.AddAsync(record);

            return new MedicalRecordResponseDto
            {
                Id = record.Id,
                Date = record.Date,
                Diagnosis = record.Diagnosis,
                Treatment = record.Treatment,
                Observations = record.Observations
            };
        }

        public async Task<MedicalRecordResponseDto?> UpdateAsync(int id, MedicalRecordUpdateDto dto)
        {
            var record = await _unitOfWork.MedicalRecords.GetByIdAsync(id);
            if (record is null) return null;

            record.Diagnosis = dto.Diagnosis;
            record.Treatment = dto.Treatment;
            record.Observations = dto.Observations;

            await _unitOfWork.MedicalRecords.UpdateAsync(record);

            return new MedicalRecordResponseDto
            {
                Id = record.Id,
                Date = record.Date,
                PetName = record.Pet?.Name ?? string.Empty,
                DoctorName = record.Doctor?.User?.Name ?? string.Empty,
                Diagnosis = record.Diagnosis,
                Treatment = record.Treatment,
                Observations = record.Observations
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var record = await _unitOfWork.MedicalRecords.GetByIdAsync(id);
            if (record is null) return false;

            await _unitOfWork.MedicalRecords.DeleteAsync(id);
            return true;
        }
    }
}