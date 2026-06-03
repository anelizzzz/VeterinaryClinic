using Microsoft.EntityFrameworkCore;
using VeterinaryClinic.API.Data;
using VeterinaryClinic.API.Models.Entities;

namespace VeterinaryClinic.API.Repositories.Adoptions
{
    public class AdoptionAnimalRepository : IRepository<AdoptionAnimal>
    {
        private readonly ApplicationDbContext _context;

        public AdoptionAnimalRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<AdoptionAnimal> AddAsync(AdoptionAnimal entity)
        {
            _context.AdoptionAnimals.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<IEnumerable<AdoptionAnimal>> GetAllAsync()
        {
            return await _context.AdoptionAnimals
                .Include(a => a.AdoptionRequests)
                .ToListAsync();
        }

        public async Task<AdoptionAnimal?> GetByIdAsync(int id)
        {
            return await _context.AdoptionAnimals
                .Include(a => a.AdoptionRequests)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task UpdateAsync(AdoptionAnimal entity)
        {
            _context.AdoptionAnimals.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var adoptionAnimal = await _context.AdoptionAnimals.FindAsync(id);
            if (adoptionAnimal != null)
            {
                _context.AdoptionAnimals.Remove(adoptionAnimal);
                await _context.SaveChangesAsync();
            }
        }
    }
}