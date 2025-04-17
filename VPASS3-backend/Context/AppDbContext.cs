using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VPASS3_backend.Models;

namespace VPASS3_backend.Context
{
    public class AppDbContext : IdentityDbContext<User, Role, string>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Registrar la entidad Establishment
        public DbSet<Establishment> Establishments { get; set; }
        public DbSet<Zone> Zones { get; set; }
        public DbSet<Visitor> Visitors { get; set; }

        public DbSet<Direction> Directions { get; set; }

        public DbSet<Visit> Visits { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Relación User - Establishment
            modelBuilder.Entity<User>()
            .HasOne(u => u.establishment) // la propiedad de navegación en User
            .WithMany(e => e.Users)       // la colección en Establishment
            .HasForeignKey(u => u.EstablishmentId) // clave foránea
            .OnDelete(DeleteBehavior.SetNull);

            // Relación entre Zone y Establishment (uno a muchos)
            modelBuilder.Entity<Zone>()
                .HasOne(z => z.Establishment) // propiedad de navegación en Zone
                .WithMany(e => e.Zones)       // colección en Establishment
                .HasForeignKey(z => z.EstablishmentId) // clave foránea
                .OnDelete(DeleteBehavior.Cascade); // En este caso, si un Establishment se elimina, todas las zonas asociadas también se eliminarán

            // Relación Visit - Establishment uno es a muchos
            modelBuilder.Entity<Visit>()
                .HasOne(v => v.Establishment)
                .WithMany(e => e.Visits)
                .HasForeignKey(v => v.EstablishmentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relación Visit - Visitor uno es a muchos
            modelBuilder.Entity<Visit>()
                .HasOne(v => v.Visitor)
                .WithMany(vis => vis.Visits)
                .HasForeignKey(v => v.VisitorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relación Visit - Zone uno es a muchos
            modelBuilder.Entity<Visit>()
                .HasOne(v => v.Zone)
                .WithMany(z => z.Visits)
                .HasForeignKey(v => v.ZoneId)
                .OnDelete(DeleteBehavior.Cascade);

            //Relacion Visit - Direction, uno es a muchos
            // Una visita tiene un sentido, y un sentido está asociado a muchas visitas
            modelBuilder.Entity<Visit>()
            .HasOne(v => v.Direction)
            .WithMany(d => d.Visits)
            .HasForeignKey(v => v.IdDirection);
        }
    }
}

