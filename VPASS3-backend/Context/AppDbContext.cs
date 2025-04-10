using System.Collections.Generic;
using System.Reflection.Emit;
using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VPASS3_backend.Models;
using Microsoft.AspNetCore.Identity;

namespace VPASS3_backend.Context
{
    public class AppDbContext : IdentityDbContext<User, Role, string>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Registrar la entidad Establishment
        public DbSet<Establishment> Establishments { get; set; }
        public DbSet<Zone> Zones { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

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
        }
    }
}

