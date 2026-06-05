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
    public DbSet<Diagnosis> Diagnoses { get; set; }

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

        modelBuilder.Entity<Appointment>()
            .Property(a => a.Type)
            .HasConversion<int>();

        modelBuilder.Entity<AdoptionAnimal>()
            .Property(a => a.Status)
            .HasConversion<int>();

        modelBuilder.Entity<AdoptionRequest>()
            .Property(ar => ar.Status)
            .HasConversion<int>();

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

        modelBuilder.Entity<MedicalRecord>()
            .HasOne(mr => mr.DiagnosisRef)
            .WithMany(d => d.MedicalRecords)
            .HasForeignKey(mr => mr.DiagnosisId)
            .OnDelete(DeleteBehavior.SetNull)
            .IsRequired(false);

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

        // Seed date inițiale pentru diagnostice comune
        modelBuilder.Entity<Diagnosis>().HasData(
            new Diagnosis { Id = 1, Name = "Otită externă", Description = "Inflamație a urechii externe", Treatment = "Curățare auriculară cu soluție antiseptică. Picături auriculare cu antibiotic și antiinflamator (ex: Otomax) timp de 7-10 zile.", Observations = "Monitorizare săptămânală. Evitați intrarea apei în urechi.", Species = "Câine" },
            new Diagnosis { Id = 2, Name = "Dermatită alergică", Description = "Reacție alergică cutanată", Treatment = "Antihistaminice (Cetirizină 5mg/zi). Șampoane hipoalergenice. Corticosteroizi în cazuri severe (Prednisolon 0.5mg/kg/zi, 5 zile).", Observations = "Identificați și eliminați alergenul. Dietă hipoalergenică recomandată.", Species = "" },
            new Diagnosis { Id = 3, Name = "Gastroenterită", Description = "Inflamație a tractului gastrointestinal", Treatment = "Dietă blândă 48-72h (orez fiert + pui fiert fără sare). Hidratare corespunzătoare. Probiotice (Fortiflora 1 plic/zi, 7 zile). Metronidazol 15mg/kg de 2 ori pe zi, 5 zile dacă este necesar.", Observations = "Monitorizați hidratarea. Dacă vărsăturile persistă peste 24h, repetați consultul.", Species = "" },
            new Diagnosis { Id = 4, Name = "Infecție urinară", Description = "Infecție bacteriană a tractului urinar", Treatment = "Amoxicilină-clavulanat 15mg/kg de 2 ori pe zi, 10-14 zile. Creșteți aportul de apă. Urocolvin sau alt supliment urinar.", Observations = "Recoltați urocultură dacă infecțiile sunt recurente. Reevaluare la 2 săptămâni.", Species = "" },
            new Diagnosis { Id = 5, Name = "Conjunctivită", Description = "Inflamație a conjunctivei oculare", Treatment = "Curățarea ochilor cu ser fiziologic steril. Picături oftalmice cu antibiotic (Tobramicină sau Cloramfenicol) de 3 ori pe zi, 7 zile.", Observations = "Evitați expunerea la vânt și praf. Dacă nu se ameliorează în 5 zile, consultați un oftalmolog veterinar.", Species = "" },
            new Diagnosis { Id = 6, Name = "Parazitoză intestinală", Description = "Infestare cu paraziți intestinali", Treatment = "Fenbendazol 50mg/kg/zi, 3-5 zile sau Pyrantel 5mg/kg doză unică. Deparazitare internă periodică la 3-6 luni.", Observations = "Igienizarea mediului. Deparazitare și la alți animale de companie din gospodărie.", Species = "" },
            new Diagnosis { Id = 7, Name = "Diabet zaharat", Description = "Insuficiență a reglării glicemice", Treatment = "Insulină (dozaj individualizat în funcție de glicemie). Dietă cu conținut redus de carbohidrați. Monitorizare glicemie zilnică.", Observations = "Educarea proprietarului privind semnele de hipoglicemie. Control lunar.", Species = "" },
            new Diagnosis { Id = 8, Name = "Hipotiroidism", Description = "Producție insuficientă de hormoni tiroidieni", Treatment = "Levotiroxină sodică 0.02mg/kg de 2 ori pe zi. Ajustarea dozei după 4-6 săptămâni în funcție de TSH.", Observations = "Tratament pe viață. Control tiroidian la 6 luni.", Species = "Câine" },
            new Diagnosis { Id = 9, Name = "Stomatită", Description = "Inflamație a mucoasei bucale", Treatment = "Curățare dentară profesională sub anestezie. Antibiotice (Clindamicină 5mg/kg de 2 ori pe zi, 10 zile). Gel oral antiseptic local.", Observations = "Periaj dentar regulat. Dietă cu hrană umedă în perioada de recuperare.", Species = "" },
            new Diagnosis { Id = 10, Name = "Artrită", Description = "Inflamație articulară degenerativă", Treatment = "Meloxicam 0.1mg/kg/zi cu mâncare. Suplimente condroprotectoare (Glucozamină + Condroitin). Fizioterapie recomandată.", Observations = "Evitați efortul fizic intens. Suprafețe moi pentru odihnă. Monitorizare renală la tratament lung cu AINS.", Species = "" }
        );
    }
}