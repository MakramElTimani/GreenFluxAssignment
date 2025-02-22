using Microsoft.EntityFrameworkCore;

namespace GreenFluxAssignment.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<GroupDataModel> Groups { get; set; }
    public DbSet<ChargeStationDataModel> ChargeStations { get; set; }
    public DbSet<ConnectorDataModel> Connectors { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //Configure Keys
        modelBuilder.Entity<GroupDataModel>().HasKey(g => g.Id);
        modelBuilder.Entity<ChargeStationDataModel>().HasKey(cs => cs.Id);
        modelBuilder.Entity<ConnectorDataModel>().HasKey(c => new { c.Id, c.ChargeStationId });

        // Configure the relationships between the entities

        // A group can have many charge stations
        // A charge station belongs to a group
        // When a group is deleted, all charge stations in that group will be deleted
        modelBuilder.Entity<GroupDataModel>().HasMany(g => g.ChargeStations).WithOne(cs => cs.Group).OnDelete(DeleteBehavior.Cascade);

        // A charge station can have many connectors
        // A connector belongs to a charge station
        // When a charge station is deleted, all connectors in that charge station will be deleted
        modelBuilder.Entity<ChargeStationDataModel>().HasMany(cs => cs.Connectors).WithOne(c => c.ChargeStation).OnDelete(DeleteBehavior.Cascade);
    }
}
