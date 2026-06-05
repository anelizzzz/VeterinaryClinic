using VeterinaryClinic.API.Repositories.Adoptions;
using VeterinaryClinic.API.Repositories.Appointments;
using VeterinaryClinic.API.Repositories.Clients;
using VeterinaryClinic.API.Repositories.Diagnoses;
using VeterinaryClinic.API.Repositories.Doctors;
using VeterinaryClinic.API.Repositories.LabReults;
using VeterinaryClinic.API.Repositories.MedicalRecords;
using VeterinaryClinic.API.Repositories.Pets;
using VeterinaryClinic.API.Repositories.Users;

namespace VeterinaryClinic.API.Services
{
    public interface IUnitOfWork
    {
        AdoptionAnimalRepository? AdoptionAnimals { get; }
        AdoptionRequestRepository? AdoptionRequests { get; }
        AppointmentsRepository? Appointments { get; }
        ClientRepository? Clients { get; }
        DiagnosisRepository Diagnoses { get; }
        DoctorRepository? Doctors { get; }
        LabResultRepository? LabResults { get; }
        MedicalRecordRepository? MedicalRecords { get; }
        PetRepository? Pets { get; }
        UserRepository? Users { get; }
        void Dispose();
        Task<int> SaveChangesAsync();
    }
}