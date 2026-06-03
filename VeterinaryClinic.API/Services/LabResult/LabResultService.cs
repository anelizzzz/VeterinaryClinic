using VeterinaryClinic.API.DTOs.LabResult;
using VeterinaryClinic.API.Models.Entities;

namespace VeterinaryClinic.API.Services
{
    public class LabResultService
    {
        private readonly IUnitOfWork _unitOfWork;

        public LabResultService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<LabResultResponseDto>> GetAllAsync()
        {
            var labResults = await _unitOfWork.LabResults.GetAllAsync();
            return labResults.Select(l => new LabResultResponseDto
            {
                Id = l.Id,
                TestType = l.TestType,
                Date = l.Date,
                FilePath = l.FilePath,
                KeyValues = l.KeyValues,
                Interpretation = l.Interpretation,
                PetName = l.Pet?.Name ?? string.Empty
            });
        }

        public async Task<LabResultResponseDto?> GetByIdAsync(int id)
        {
            var labResult = await _unitOfWork.LabResults.GetByIdAsync(id);
            if (labResult is null) return null;

            return new LabResultResponseDto
            {
                Id = labResult.Id,
                TestType = labResult.TestType,
                Date = labResult.Date,
                FilePath = labResult.FilePath,
                KeyValues = labResult.KeyValues,
                Interpretation = labResult.Interpretation,
                PetName = labResult.Pet?.Name ?? string.Empty
            };
        }

        public async Task<IEnumerable<LabResultResponseDto>> GetByPetIdAsync(int petId)
        {
            var labResults = await _unitOfWork.LabResults.GetAllAsync();
            return labResults
                .Where(l => l.PetId == petId)
                .Select(l => new LabResultResponseDto
                {
                    Id = l.Id,
                    TestType = l.TestType,
                    Date = l.Date,
                    FilePath = l.FilePath,
                    KeyValues = l.KeyValues,
                    Interpretation = l.Interpretation,
                    PetName = l.Pet?.Name ?? string.Empty
                });
        }

        public async Task<LabResultResponseDto> CreateAsync(LabResultCreateDto dto)
        {
            var labResult = new LabResult
            {
                TestType = dto.TestType,
                FilePath = dto.FilePath,
                KeyValues = dto.KeyValues,
                Interpretation = dto.Interpretation,
                PetId = dto.PetId,
                Date = DateTime.UtcNow
            };

            await _unitOfWork.LabResults.AddAsync(labResult);

            return new LabResultResponseDto
            {
                Id = labResult.Id,
                TestType = labResult.TestType,
                Date = labResult.Date,
                FilePath = labResult.FilePath,
                KeyValues = labResult.KeyValues,
                Interpretation = labResult.Interpretation
            };
        }

        public async Task<LabResultResponseDto?> UpdateAsync(int id, LabResultCreateDto dto)
        {
            var labResult = await _unitOfWork.LabResults.GetByIdAsync(id);
            if (labResult is null) return null;

            labResult.TestType = dto.TestType;
            labResult.FilePath = dto.FilePath;
            labResult.KeyValues = dto.KeyValues;
            labResult.Interpretation = dto.Interpretation;
            labResult.PetId = dto.PetId;

            await _unitOfWork.LabResults.UpdateAsync(labResult);

            return new LabResultResponseDto
            {
                Id = labResult.Id,
                TestType = labResult.TestType,
                Date = labResult.Date,
                FilePath = labResult.FilePath,
                KeyValues = labResult.KeyValues,
                Interpretation = labResult.Interpretation,
                PetName = labResult.Pet?.Name ?? string.Empty
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var labResult = await _unitOfWork.LabResults.GetByIdAsync(id);
            if (labResult is null) return false;

            await _unitOfWork.LabResults.DeleteAsync(id);
            return true;
        }
    }
}