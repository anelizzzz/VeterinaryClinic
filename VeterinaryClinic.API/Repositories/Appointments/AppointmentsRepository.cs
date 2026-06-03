using Microsoft.EntityFrameworkCore;
using VeterinaryClinic.API.Data;
using VeterinaryClinic.API.Models.Entities;

namespace VeterinaryClinic.API.Repositories.Appointments
{
    public class AppointmentsRepository : IRepository<Appointment>
    {
        private readonly ApplicationDbContext _context;

        public AppointmentsRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Appointment> AddAsync(Appointment entity)
        {
            _context.Appointments.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
        public async Task<Appointment?> GetByIdWithDetailsAsync(int id)
        {
            return await _context.Appointments
                .Include(a => a.Pet)
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.User)
                .Include(a => a.Client)
                    .ThenInclude(c => c.User)
                .FirstOrDefaultAsync(a => a.Id == id);
        }
        public async Task<IEnumerable<Appointment>> GetAllAsync()
        {
            return await _context.Appointments
                .Include(a => a.Pet)
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.User)
                .Include(a => a.Client)
                    .ThenInclude(c => c.User)
                .ToListAsync();
        }

        public async Task<Appointment?> GetByIdAsync(int id)
        {
            return await _context.Appointments
                .Include(a => a.Pet)
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.User)
                .Include(a => a.Client)
                    .ThenInclude(c => c.User)
                .FirstOrDefaultAsync(a => a.Id == id);
        }
        public async Task<IEnumerable<Pet>> GetDistinctPetsByDoctorIdAsync(int doctorId)
        {
            return await _context.Appointments
                .Where(a => a.DoctorId == doctorId)
                .Include(a => a.Pet)
                    .ThenInclude(p => p.Client)
                        .ThenInclude(c => c.User)
                .Select(a => a.Pet)
                .GroupBy(p => p.Id)
                .Select(g => g.First())
                .ToListAsync();
        }
        public async Task UpdateAsync(Appointment entity)
        {
            _context.Appointments.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment != null)
            {
                _context.Appointments.Remove(appointment);
                await _context.SaveChangesAsync();
            }
        }
    }
}