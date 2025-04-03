using Microsoft.EntityFrameworkCore;
using VPASS3_backend.Models;

namespace VPASS3_backend.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Person> Persons { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración de la relación muchos a uno entre User y Role
            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)  // Un User tiene un Role
                .WithMany()  // Un Role puede tener muchos Users, pero no necesitamos la colección de Users en Role
                .HasForeignKey(u => u.RoleId); // Definir la clave foránea en User

            // Configuración para asegurarse que el email sea único
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique(); // Esto hace que el email sea único en la base de datos
    }
    }
}

