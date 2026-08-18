using DCWS.MainSite.Api.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace DCWS.MainSite.Api.Domain;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<StatusTest> StatusEntries => Set<StatusTest>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<StatusTest>(entity =>
        {
            entity.ToTable("StatusTest", "dbo");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Message).IsRequired();
            entity.Property(x => x.CreatedDateUtc).IsRequired();
        });

        base.OnModelCreating(modelBuilder);
    }
}
