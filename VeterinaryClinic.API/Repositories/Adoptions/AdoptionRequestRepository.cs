using Microsoft.EntityFrameworkCore;
using VeterinaryClinic.API.Data;
using VeterinaryClinic.API.Models.Entities;

namespace VeterinaryClinic.API.Repositories.Adoptions
{
    public class AdoptionRequestRepository : IRepository<AdoptionRequest>
    {
        private readonly ApplicationDbContext _context;

        public AdoptionRequestRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<AdoptionRequest> AddAsync(AdoptionRequest entity)
        {
            _context.AdoptionRequests.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<IEnumerable<AdoptionRequest>> GetAllAsync()
        {
            return await _context.AdoptionRequests
                .Include(r => r.Client)
                    .ThenInclude(c => c.User)
                .Include(r => r.AdoptionAnimal)
                .ToListAsync();
        }

        public async Task<AdoptionRequest?> GetByIdAsync(int id)
        {
            return await _context.AdoptionRequests
                .Include(r => r.Client)
                    .ThenInclude(c => c.User)
                .Include(r => r.AdoptionAnimal)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task UpdateAsync(AdoptionRequest entity)
        {
            _context.AdoptionRequests.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var adoptionRequest = await _context.AdoptionRequests.FindAsync(id);
            if (adoptionRequest != null)
            {
                _context.AdoptionRequests.Remove(adoptionRequest);
                await _context.SaveChangesAsync();
            }
        }
    }
}