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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //// Relación entre User y Roles por medio de tabla intermedia (ya lo hace Identity, pero lo ponemos explícito)
            //modelBuilder.Entity<User>()
            //    .HasMany<IdentityUserRole<string>>()
            //    .WithOne()
            //    .HasForeignKey(ur => ur.UserId)
            //    .IsRequired();
        }

        // Si tienes otras entidades, mantenlas
        //public DbSet<Person> Persons { get; set; }
    }
}

