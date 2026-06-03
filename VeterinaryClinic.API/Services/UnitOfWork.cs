using VeterinaryClinic.API.Data;
using VeterinaryClinic.API.Models.Entities;
using VeterinaryClinic.API.Repositories.Adoptions;
using VeterinaryClinic.API.Repositories.Appointments;
using VeterinaryClinic.API.Repositories.Clients;
using VeterinaryClinic.API.Repositories.Doctors;
using VeterinaryClinic.API.Repositories.LabReults;
using VeterinaryClinic.API.Repositories.MedicalRecords;
using VeterinaryClinic.API.Repositories.Pets;
using VeterinaryClinic.API.Repositories.Users;

namespace VeterinaryClinic.API.Services
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public AppointmentsRepository? Appointments { get; }
        public AdoptionAnimalRepository? AdoptionAnimals { get; }
        public AdoptionRequestRepository? AdoptionRequests { get; }
        public LabResultRepository? LabResults { get; }
        public MedicalRecordRepository? MedicalRecords { get; }

        public UserRepository? Users { get; }
        public ClientRepository? Clients { get; }
        public DoctorRepository? Doctors { get; }
        public PetRepository? Pets { get; }

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            Appointments = new AppointmentsRepository(_context);
            AdoptionAnimals = new AdoptionAnimalRepository(_context);
            AdoptionRequests = new AdoptionRequestRepository(_context);
            LabResults = new LabResultRepository(_context);
            MedicalRecords = new MedicalRecordRepository(_context);
            Users = new UserRepository(_context);
            Clients = new ClientRepository(_context);
            Doctors = new DoctorRepository(_context);
            Pets = new PetRepository(_context);
        }
        public async Task<int> SaveChangesAsync()
            => await _context.SaveChangesAsync();

        public void Dispose()
            => _context.Dispose();
    }
}
