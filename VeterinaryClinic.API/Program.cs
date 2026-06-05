using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using VeterinaryClinic.API.Data;
using VeterinaryClinic.API.Repositories.Users;
using VeterinaryClinic.API.Services;
using VeterinaryClinic.API.Services.AiDiagnosis;
using VeterinaryClinic.API.Services.Diagnosis;
using VeterinaryClinic.API.Services.Doctor;
using VeterinaryClinic.API.Services.Email;
using VeterinaryClinic.API.Services.Pdf;

var builder = WebApplication.CreateBuilder(args);

var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseNpgsql(connectionString);
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("ProductionCors", policy =>
    {
        policy
            .WithOrigins("https://veterinary-clinic-red.vercel.app")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "VeterinaryClinic API",
        Version = "v1"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Introdu token-ul JWT. Exemplu: Bearer eyJhbGciOi..."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<AppointmentService>();
builder.Services.AddScoped<PetService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<ClientService>();
builder.Services.AddScoped<DoctorService>();
builder.Services.AddScoped<LabResultService>();
builder.Services.AddScoped<MedicalRecordService>();
builder.Services.AddScoped<AdoptionAnimalService>();
builder.Services.AddScoped<AdoptionRequestService>();
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddHttpClient<IAIDiagnosisService, AIDiagnosisService>();
builder.Services.AddScoped<IPdfService, PdfService>();
builder.Services.AddScoped<DiagnosisService>();
builder.Services.AddScoped<IEmailService, EmailService>();

var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI();

app.UseRouting();

app.UseCors("ProductionCors");
app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles();

app.MapControllers();

app.Run();