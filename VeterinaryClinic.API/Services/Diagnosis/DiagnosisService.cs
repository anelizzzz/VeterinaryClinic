using VeterinaryClinic.API.DTOs.Diagnosis;
using DiagnosisEntity = VeterinaryClinic.API.Models.Entities.Diagnosis;

namespace VeterinaryClinic.API.Services.Diagnosis
{
    public class DiagnosisService
    {
        private readonly IUnitOfWork _unitOfWork;

        public DiagnosisService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        private static DiagnosisResponseDto MapToDto(DiagnosisEntity d) => new()
        {
            Id = d.Id,
            Name = d.Name,
            Description = d.Description,
            Treatment = d.Treatment,
            Observations = d.Observations,
            Species = d.Species
        };

        public async Task<IEnumerable<DiagnosisResponseDto>> GetAllAsync()
        {
            var list = await _unitOfWork.Diagnoses.GetAllAsync();
            return list.Select(MapToDto);
        }

        public async Task<IEnumerable<DiagnosisResponseDto>> GetBySpeciesAsync(string species)
        {
            var list = await _unitOfWork.Diagnoses.GetAllAsync();
            return list
                .Where(d => string.IsNullOrWhiteSpace(d.Species) || d.Species == species)
                .Select(MapToDto);
        }

        public async Task<DiagnosisResponseDto?> GetByIdAsync(int id)
        {
            var d = await _unitOfWork.Diagnoses.GetByIdAsync(id);
            return d is null ? null : MapToDto(d);
        }

        public async Task<DiagnosisResponseDto> CreateAsync(DiagnosisCreateDto dto)
        {
            var d = new DiagnosisEntity
            {
                Name = dto.Name,
                Description = dto.Description,
                Treatment = dto.Treatment,
                Observations = dto.Observations,
                Species = dto.Species
            };
            await _unitOfWork.Diagnoses.AddAsync(d);
            return MapToDto(d);
        }

        public async Task<DiagnosisResponseDto?> UpdateAsync(int id, DiagnosisUpdateDto dto)
        {
            var d = await _unitOfWork.Diagnoses.GetByIdAsync(id);
            if (d is null) return null;

            d.Name = dto.Name;
            d.Description = dto.Description;
            d.Treatment = dto.Treatment;
            d.Observations = dto.Observations;
            d.Species = dto.Species;

            await _unitOfWork.Diagnoses.UpdateAsync(d);
            return MapToDto(d);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var d = await _unitOfWork.Diagnoses.GetByIdAsync(id);
            if (d is null) return false;
            await _unitOfWork.Diagnoses.DeleteAsync(id);
            return true;
        }
    }
}