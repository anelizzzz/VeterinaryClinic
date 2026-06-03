using Microsoft.EntityFrameworkCore;
using VeterinaryClinic.API.Data;
using VeterinaryClinic.API.Models.Entities;

namespace VeterinaryClinic.API.Repositories.LabReults
{
    public class LabResultRepository : IRepository<LabResult>
    {
        private readonly ApplicationDbContext _context;

        public LabResultRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<LabResult> AddAsync(LabResult entity)
        {
            _context.LabResults.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<IEnumerable<LabResult>> GetAllAsync()
        {
            return await _context.LabResults
                .Include(l => l.Pet)
                .ToListAsync();
        }

        public async Task<LabResult?> GetByIdAsync(int id)
        {
            return await _context.LabResults
                .Include(l => l.Pet)
                .FirstOrDefaultAsync(l => l.Id == id);
        }

        public async Task UpdateAsync(LabResult entity)
        {
            _context.LabResults.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var labResult = await _context.LabResults.FindAsync(id);
            if (labResult != null)
            {
                _context.LabResults.Remove(labResult);
                await _context.SaveChangesAsync();
            }
        }
    }
}