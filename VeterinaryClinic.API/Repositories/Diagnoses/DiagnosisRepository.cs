using Microsoft.EntityFrameworkCore;
using VeterinaryClinic.API.Data;
using VeterinaryClinic.API.Models.Entities;

namespace VeterinaryClinic.API.Repositories.Diagnoses
{
    public class DiagnosisRepository : IRepository<Diagnosis>
    {
        private readonly ApplicationDbContext _context;

        public DiagnosisRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Diagnosis> AddAsync(Diagnosis entity)
        {
            _context.Diagnoses.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<IEnumerable<Diagnosis>> GetAllAsync()
        {
            return await _context.Diagnoses
                .OrderBy(d => d.Name)
                .ToListAsync();
        }

        public async Task<Diagnosis?> GetByIdAsync(int id)
        {
            return await _context.Diagnoses.FindAsync(id);
        }

        public async Task UpdateAsync(Diagnosis entity)
        {
            _context.Diagnoses.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var d = await _context.Diagnoses.FindAsync(id);
            if (d != null)
            {
                _context.Diagnoses.Remove(d);
                await _context.SaveChangesAsync();
            }
        }
    }
}