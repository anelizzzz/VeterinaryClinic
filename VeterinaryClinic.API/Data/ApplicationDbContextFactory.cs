using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using VeterinaryClinic.API.Data;

namespace VeterinaryClinic.API.Data
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

            optionsBuilder.UseNpgsql(
                "Host=zephyr.proxy.rlwy.net:53438;Port=5432;Database=railway;Username=postgres;Password=aHdkMemiiCikngBruBonvtlkgzrOpBSI");

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}

