using Microsoft.EntityFrameworkCore;
using VeterinaryClinic.API.Data;
using VeterinaryClinic.API.Models.Entities;

namespace VeterinaryClinic.API.Repositories.MedicalRecords
{
    public class MedicalRecordRepository : IRepository<MedicalRecord>
    {
        private readonly ApplicationDbContext _context;

        public MedicalRecordRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<MedicalRecord> AddAsync(MedicalRecord entity)
        {
            _context.MedicalRecords.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<IEnumerable<MedicalRecord>> GetAllAsync()
        {
            return await _context.MedicalRecords
                .Include(m => m.Pet)
                .Include(m => m.Doctor)
                    .ThenInclude(d => d.User)
                .ToListAsync();
        }

        public async Task<MedicalRecord?> GetByIdAsync(int id)
        {
            return await _context.MedicalRecords
                .Include(m => m.Pet)
                .Include(m => m.Doctor)
                    .ThenInclude(d => d.User)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task UpdateAsync(MedicalRecord entity)
        {
            _context.MedicalRecords.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var medicalRecord = await _context.MedicalRecords.FindAsync(id);
            if (medicalRecord != null)
            {
                _context.MedicalRecords.Remove(medicalRecord);
                await _context.SaveChangesAsync();
            }
        }
    }
}