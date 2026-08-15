using DCWS.MainSite.Api.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace DCWS.MainSite.Api.Domain;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<StatusEntry> StatusEntries => Set<StatusEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<StatusEntry>(entity =>
        {
            entity.ToTable("StatusTest", "dbo");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Message).IsRequired();
            entity.Property(x => x.CreatedDateUtc).IsRequired();
        });

        base.OnModelCreating(modelBuilder);
    }
}
