using Microsoft.EntityFrameworkCore;
using VeterinaryClinic.API.Models.Entities;
using VeterinaryClinic.API.Models.Entities.Enums;

namespace VeterinaryClinic.API.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
    public DbSet<User> Users { get; set; }
    public DbSet<Client> Clients { get; set; }
    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<Pet> Pets { get; set; }
    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<MedicalRecord> MedicalRecords { get; set; }
    public DbSet<LabResult> LabResults { get; set; }
    public DbSet<AdoptionRequest> AdoptionRequests { get; set; }
    public DbSet<AdoptionAnimal> AdoptionAnimals { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<User>()
            .Property(u => u.Role)
            .HasConversion<int>()
            .HasDefaultValue(UserRole.Client);
        modelBuilder.Entity<LabResult>()
            .Property(l => l.TestType)
            .HasConversion<int>();

        modelBuilder.Entity<Pet>()
                .Property(p => p.Status)
                .HasConversion<int>();

        modelBuilder.Entity<Appointment>()
            .Property(a => a.Status)
            .HasConversion<int>();
        //.HasDefaultValue(AppointmentStatus.Pending);

        modelBuilder.Entity<Appointment>()
            .Property(a => a.Type)
            .HasConversion<int>();
        //.HasDefaultValue(AppointmentType.Checkup);

        modelBuilder.Entity<AdoptionAnimal>()
            .Property(a => a.Status)
            .HasConversion<int>();
        //.HasDefaultValue(AdoptionStatus.Available);

        modelBuilder.Entity<AdoptionRequest>()
                .Property(ar => ar.Status)
                .HasConversion<int>();
                //.HasDefaultValue(AdoptionStatus.Pending);

        modelBuilder.Entity<Client>()
               .HasOne(c => c.User)
               .WithMany()  
               .HasForeignKey(c => c.UserId)
               .OnDelete(DeleteBehavior.Cascade)
               .IsRequired();

        modelBuilder.Entity<Doctor>()
                .HasOne(d => d.User)
                .WithMany()
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

        modelBuilder.Entity<Pet>()
               .HasOne(p => p.Client)
               .WithMany(c => c.Pets) 
               .HasForeignKey(p => p.ClientId)
               .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Client)
                .WithMany(c => c.Appointments)
                .HasForeignKey(a => a.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Appointment>()
            .HasOne(a => a.Doctor)
            .WithMany(d => d.Appointments)
            .HasForeignKey(a => a.DoctorId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Appointment>()
            .HasOne(a => a.Pet)
            .WithMany(p => p.Appointments)
            .HasForeignKey(a => a.PetId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<MedicalRecord>()
                .HasOne(mr => mr.Pet)
                .WithMany(p => p.MedicalRecords)
                .HasForeignKey(mr => mr.PetId)
                .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<MedicalRecord>()
            .HasOne(mr => mr.Doctor)
            .WithMany(d => d.MedicalRecords)
            .HasForeignKey(mr => mr.DoctorId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<LabResult>()
               .HasOne(lr => lr.Pet)
               .WithMany(p => p.LabResults)
               .HasForeignKey(lr => lr.PetId)
               .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<AdoptionRequest>()
                .HasOne(ar => ar.Client)
                .WithMany(c => c.AdoptionRequests)
                .HasForeignKey(ar => ar.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<AdoptionRequest>()
            .HasOne(ar => ar.AdoptionAnimal)
            .WithMany(aa => aa.AdoptionRequests)
            .HasForeignKey(ar => ar.AdoptionAnimalId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
