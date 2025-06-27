using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VPASS3_backend.Enums;
using VPASS3_backend.Models;
using VPASS3_backend.Models.CommonAreas;

namespace VPASS3_backend.Context
{
    public class AppDbContext : IdentityDbContext<User, Role, int>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Registrar la entidad Establishment
        public DbSet<Establishment> Establishments { get; set; }
        public DbSet<Zone> Zones { get; set; }
        //public DbSet<Visitor> Visitors { get; set; }

        public DbSet<Direction> Directions { get; set; }

        public DbSet<Visit> Visits { get; set; }

        public DbSet<Apartment> Apartments { get; set; }

        public DbSet<ParkingSpot> ParkingSpots { get; set; }

        public DbSet<VisitType> VisitTypes { get; set; }

        //Entidad para guardar las bitacoras de algunas acciones
        public DbSet<AuditLog> AuditLogs { get; set; }

        public DbSet<Blacklist> Blacklists { get; set; }

        public DbSet<ParkingSpotUsageLog> ParkingSpotUsageLogs { get; set; }

        public DbSet<Person> Persons { get; set; }

        public DbSet<CommonArea> CommonAreas { get; set; }

        public DbSet<CommonAreaReservation> CommonAreaReservations { get; set; }

        public DbSet<CommonAreaUsageLog> CommonAreaUsageLogs { get; set; }

        public DbSet<ApartmentOwnership> ApartmentOwnerships { get; set; }

        public DbSet<Package> Packages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Relación User - Establishment
            modelBuilder.Entity<User>()
            .HasOne(u => u.Establishment)
            .WithMany(e => e.Users)
            .HasForeignKey(u => u.EstablishmentId)
            .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<User>()
            .HasOne(u => u.Establishment)
            .WithMany(e => e.Users)
            .HasForeignKey(u => u.EstablishmentId)
            .OnDelete(DeleteBehavior.SetNull);// evita eliminación en cascada si deseas proteger al usuario


            // Relación entre Visit y Apartment (para que use IdApartment como FK)
            modelBuilder.Entity<Visit>()
            .HasOne(v => v.Apartment)
            .WithMany() // No necesitas colección inversa
            .HasForeignKey(v => v.IdApartment)
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
                .HasOne(v => v.Person)
                .WithMany(vis => vis.Visits)
                .HasForeignKey(v => v.IdPerson)
                .OnDelete(DeleteBehavior.Restrict);

            // Relación Blacklist - Person (muchos a uno)
            modelBuilder.Entity<Blacklist>()
                .HasOne(b => b.Person)
                .WithMany(v => v.Blacklists)
                .HasForeignKey(b => b.IdPerson)
                .OnDelete(DeleteBehavior.Restrict);

            // Relación Blacklist - Establishment (muchos a uno)
            modelBuilder.Entity<Blacklist>()
                .HasOne(b => b.Establishment)
                .WithMany(e => e.Blacklists)
                .HasForeignKey(b => b.IdEstablishment)
                .OnDelete(DeleteBehavior.Restrict);

            // Relación Visit - Zone uno es a muchos
            modelBuilder.Entity<Visit>()
            .HasOne(v => v.Zone)
            .WithMany() // Sin propiedad de navegación en Zone
            .HasForeignKey(v => v.ZoneId)
            .OnDelete(DeleteBehavior.Restrict);

            //Relacion Visit - Direction, uno es a muchos
            // Una visita tiene un sentido, y un sentido está asociado a muchas visitas
            modelBuilder.Entity<Visit>()
            .HasOne(v => v.Direction)
            .WithMany(d => d.Visits)
            .HasForeignKey(v => v.IdDirection);

            // Relacion uno es a muchos, una zona tiene muchos departamentos, y cada departamento está asociado solamente a una zona.
            modelBuilder.Entity<Zone>()
            .HasMany(z => z.Apartments)
            .WithOne(zs => zs.Zone)
            .HasForeignKey(zs => zs.IdZone)
            .OnDelete(DeleteBehavior.Cascade); // Esto elimina las ZoneSections si se borra la Zone

            // Relacion de uno es a muchos con establecimiento. Un establecimiento tiene muchas zonas de estacionamiento, pero cada estacionamiento está´asociado a un establecimiento.
            modelBuilder.Entity<ParkingSpot>()
            .HasOne(p => p.Establishment)
            .WithMany(e => e.ParkingSpots)
            .HasForeignKey(p => p.IdEstablishment)
            .OnDelete(DeleteBehavior.Cascade);

            // Relacion de uno es a muchos con Visitas. Una visita puede tener asociado un estacionamiento, y cada estacionamiento puede estar asociado a varias visitas.
            modelBuilder.Entity<Visit>()
            .HasOne(v => v.ParkingSpot)
            .WithMany(p => p.Visits)
            .HasForeignKey(v => v.IdParkingSpot)
            .OnDelete(DeleteBehavior.Restrict);

            // Relación de uno es a muchos con visitType. Una visita tiene un tipo de visita asociado, pero el tipo de visita puede repetirse en otras visitas diferentes.
            modelBuilder.Entity<Visit>()
            .HasOne(v => v.VisitType)
            .WithMany() // <-- Sin navegación inversa
            .HasForeignKey(v => v.IdVisitType)
            .OnDelete(DeleteBehavior.Restrict);

            // Relación Establishment -> CommonAreas (uno a muchos)
            modelBuilder.Entity<CommonArea>()
                .HasOne(ca => ca.Establishment)
                .WithMany(e => e.CommonAreas)
                .HasForeignKey(ca => ca.IdEstablishment)
                .OnDelete(DeleteBehavior.Cascade);

            // CommonArea -> Reservations (uno a muchos)
            modelBuilder.Entity<CommonAreaReservation>()
                .HasOne(r => r.CommonArea)
                .WithMany(ca => ca.Reservations)
                .HasForeignKey(r => r.IdCommonArea)
                .OnDelete(DeleteBehavior.Cascade);

            // CommonArea -> Usages (uno a muchos)
            modelBuilder.Entity<CommonAreaUsageLog>()
                .HasOne(u => u.CommonArea)
                .WithMany(ca => ca.Usages)
                .HasForeignKey(u => u.IdCommonArea)
                .OnDelete(DeleteBehavior.Cascade);

            // Relación Reservation -> Persona que reserva (muchos a uno)
            modelBuilder.Entity<CommonAreaReservation>()
                .HasOne(r => r.ReservedBy)
                .WithMany()
                .HasForeignKey(r => r.IdPersonReservedBy)
                .OnDelete(DeleteBehavior.Restrict);

            // Relación Usage -> Persona que usa (muchos a uno)
            modelBuilder.Entity<CommonAreaUsageLog>()
                .HasOne(u => u.Person)
                .WithMany()
                .HasForeignKey(u => u.IdPerson)
                .OnDelete(DeleteBehavior.Restrict);

            // Relación: Apartment -> ApartmentOwnership
            modelBuilder.Entity<ApartmentOwnership>()
                .HasOne(ao => ao.Apartment)
                .WithMany(a => a.Ownerships)
                .HasForeignKey(ao => ao.IdApartment)
                .OnDelete(DeleteBehavior.Restrict);

            // Relación: Person -> ApartmentOwnership
            modelBuilder.Entity<ApartmentOwnership>()
                .HasOne(ao => ao.Person)
                .WithMany(p => p.ApartmentOwnerships)
                .HasForeignKey(ao => ao.IdPerson)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}