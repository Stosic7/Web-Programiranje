namespace WebTemplate.Models;

public class IspitContext : DbContext
{
    // DbSet kolekcije!

    public DbSet<Aerodrom> Aerodromi { get; set; }

    public DbSet<Letelica> Letelice { get; set; }

    public DbSet<Let> Letovi { get; set; }

    public IspitContext(DbContextOptions options) : base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Konfiguracija modela ako je potrebna
        modelBuilder.Entity<Let>().HasOne(l => l.PolazniAerodrom).WithMany(a => a.PolazniLetovi);

        modelBuilder.Entity<Let>().HasOne(l => l.DolazniAerodrom).WithMany(a => a.DolazniLetovi);
    }
}
