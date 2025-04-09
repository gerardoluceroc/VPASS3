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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración explícita de la relación uno a muchos (opcional)
            modelBuilder.Entity<Establishment>()
                .HasMany(e => e.Users)
                .WithOne() // No se especifica propiedad de navegación en User
                .HasForeignKey("EstablishmentId") // Se define la FK shadow property
                .OnDelete(DeleteBehavior.SetNull); // Opcional: qué hacer si se borra un Establishment
        }
    }
}

