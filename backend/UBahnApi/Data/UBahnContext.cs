using Microsoft.EntityFrameworkCore;
using UBahnApi.Models;

namespace UBahnApi.Data;

public class UBahnContext(DbContextOptions<UBahnContext> options) : DbContext(options)
{
    public DbSet<Linie> Linien => Set<Linie>();
    public DbSet<Station> Stationen => Set<Station>();
    public DbSet<Fahrzeit> Fahrzeiten => Set<Fahrzeit>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Fahrzeit>()
            .HasOne(f => f.VonStation)
            .WithMany()
            .HasForeignKey(f => f.VonStationId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Fahrzeit>()
            .HasOne(f => f.NachStation)
            .WithMany()
            .HasForeignKey(f => f.NachStationId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Station>()
            .HasOne(s => s.Linie)
            .WithMany(l => l.Stationen)
            .HasForeignKey(s => s.LinieId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
