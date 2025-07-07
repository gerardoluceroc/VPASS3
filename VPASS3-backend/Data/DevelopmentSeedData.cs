// VPASS3_backend/Data/DevelopmentSeedData.cs
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;
using VPASS3_backend.Models;
using VPASS3_backend.Context;
using VPASS3_backend.Data;
using System.Collections.Generic;
using VPASS3_backend.Enums;
using VPASS3_backend.Models.CommonAreas;
using VPASS3_backend.Utils;

namespace VPASS3_backend.Data
{
    public static class DevelopmentSeedData
    {
        // --- Variables de Configuración para Desarrollo ---
        private static readonly string _adminRoleName = SeedData._adminRoleName;

        private const string _adminEmail = "gerardoluceroc@gmail.com";
        private const string _adminPassword = "Clave123.";
        private const string _adminUserName = "ADMIN_EXAMPLE";

        private const string _establishmentName = "Condominio de Prueba VPASS3";

        private static readonly List<string> _parkingSpotNames = new List<string>
        {
            "P101", "P102", "P103", "P104", "P105"
        };

        private static readonly List<string> _zoneNames = new List<string>
        {
            "Piso 2", "Piso 3", "Piso 4"
        };

        private static readonly Dictionary<string, List<string>> _apartmentsByZone = new Dictionary<string, List<string>>
        {
            { "Piso 2", new List<string> { "201", "202", "203" } },
            { "Piso 3", new List<string> { "301", "302", "303" } },
            { "Piso 4", new List<string> { "401", "402", "403" } }
        };

        private static readonly List<string> _visitTypeNames = new List<string>
        {
            "Visita normal", "Tecnico", "VTR", "Gasfiter", "WOM"
        };

        private static readonly List<(string Name, CommonAreaMode Mode, int? MaxCapacity)> _commonAreaDetails = new List<(string Name, CommonAreaMode Mode, int? MaxCapacity)>
        {
            ("Piscina", CommonAreaMode.Usable | CommonAreaMode.Reservable, null),
            ("Gimnasio", CommonAreaMode.Usable | CommonAreaMode.Reservable, 20),
            ("Centro de eventos", CommonAreaMode.Reservable, 50)
        };

        private static readonly List<(string Names, string LastNames, string IdentificationNumber)> _personDetails = new List<(string Names, string LastNames, string IdentificationNumber)>
        {
            ("Juanito Andres", "Silva Gomez", "17766627-0"),
            ("Antonio Momo", "Margarette Gonzalez", "15370138-5"),
            ("Valentina Andre", "Perez Olguin", "17765744-1")
        };

        // Nueva variable: Definición de las relaciones de propiedad de apartamentos
        private static readonly List<(string ApartmentName, string PersonIdentificationNumber)> _ownershipDetails = new List<(string ApartmentName, string PersonIdentificationNumber)>
        {
            ("201", "17766627-0"), // Juanito Andres Silva Gomez en el 201
            ("301", "15370138-5"), // Antonio Momo Margarette Gonzalez en el 301
            ("302", "17765744-1")  // Valentina Andre Perez Olguin en el 302
        };
        // --- Fin Variables de Configuración ---


        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var services = scope.ServiceProvider;
                var userManager = services.GetRequiredService<UserManager<User>>();
                var roleManager = services.GetRequiredService<RoleManager<Role>>();
                var context = services.GetRequiredService<AppDbContext>();

                Console.WriteLine("Iniciando proceso de siembra de datos de DESARROLLO...");

                // --- 1. Sembrar Usuario ADMIN de Ejemplo ---
                Console.WriteLine("Sembrando usuario ADMIN de ejemplo...");

                if (await userManager.FindByEmailAsync(_adminEmail) == null)
                {
                    var adminUser = new User
                    {
                        UserName = _adminUserName,
                        Email = _adminEmail,
                        EmailConfirmed = true,
                        EstablishmentId = null
                    };

                    var result = await userManager.CreateAsync(adminUser, _adminPassword);

                    if (result.Succeeded)
                    {
                        if (await roleManager.RoleExistsAsync(_adminRoleName))
                        {
                            await userManager.AddToRoleAsync(adminUser, _adminRoleName);
                            Console.WriteLine($"- Usuario '{_adminEmail}' ({_adminUserName}) creado y rol '{_adminRoleName}' asignado.");
                        }
                        else
                        {
                            Console.WriteLine($"Advertencia: El rol '{_adminRoleName}' no existe, no se pudo asignar al usuario '{_adminEmail}'.");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Error al crear usuario '{_adminEmail}':");
                        foreach (var error in result.Errors)
                        {
                            Console.WriteLine($"- {error.Description}");
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"- Usuario '{_adminEmail}' ({_adminUserName}) ya existe.");

                    var existingAdminUser = await userManager.FindByEmailAsync(_adminEmail);
                    if (existingAdminUser != null && !await userManager.IsInRoleAsync(existingAdminUser, _adminRoleName))
                    {
                        if (await roleManager.RoleExistsAsync(_adminRoleName))
                        {
                            await userManager.AddToRoleAsync(existingAdminUser, _adminRoleName);
                            Console.WriteLine($"- Rol '{_adminRoleName}' asignado a usuario existente '{_adminEmail}'.");
                        }
                    }
                }
                Console.WriteLine("Usuario ADMIN de ejemplo sembrado.");


                // --- 2. Sembrar Establecimiento de Ejemplo y Asignarlo al usuario ADMIN ---
                Console.WriteLine("Sembrando establecimiento de ejemplo...");
                var existingEstablishment = await context.Establishments.FirstOrDefaultAsync(e => e.Name == _establishmentName);

                Establishment createdOrExistingEstablishment = null;

                if (existingEstablishment == null)
                {
                    var newEstablishment = new Establishment
                    {
                        Name = _establishmentName,
                    };
                    context.Establishments.Add(newEstablishment);
                    await context.SaveChangesAsync();
                    createdOrExistingEstablishment = newEstablishment;
                    Console.WriteLine($"- Establecimiento '{newEstablishment.Name}' creado con Id: {newEstablishment.Id}.");
                }
                else
                {
                    createdOrExistingEstablishment = existingEstablishment;
                    Console.WriteLine($"- Establecimiento '{_establishmentName}' ya existe. (ID: {createdOrExistingEstablishment.Id})");
                }

                // Se asigna el EstablishmentId al usuario ADMIN de ejemplo.
                var adminUserToUpdate = await userManager.FindByEmailAsync(_adminEmail);
                if (adminUserToUpdate != null && createdOrExistingEstablishment != null)
                {
                    if (adminUserToUpdate.EstablishmentId == null || adminUserToUpdate.EstablishmentId != createdOrExistingEstablishment.Id)
                    {
                        adminUserToUpdate.EstablishmentId = createdOrExistingEstablishment.Id;
                        var updateResult = await userManager.UpdateAsync(adminUserToUpdate);
                        if (updateResult.Succeeded)
                        {
                            Console.WriteLine($"- Usuario '{_adminEmail}' asignado al establecimiento '{createdOrExistingEstablishment.Name}'.");
                        }
                        else
                        {
                            Console.WriteLine($"Error al asignar establecimiento al usuario '{_adminEmail}':");
                            foreach (var error in updateResult.Errors)
                            {
                                Console.WriteLine($"- {error.Description}");
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine($"- Usuario '{_adminEmail}' ya está asignado al establecimiento '{createdOrExistingEstablishment.Name}'.");
                    }
                }
                Console.WriteLine("Establecimiento de ejemplo sembrado y asignado.");

                // --- 3. Sembrar Estacionamientos de Prueba ---
                Console.WriteLine("Sembrando estacionamientos de prueba...");

                if (createdOrExistingEstablishment != null)
                {
                    foreach (var spotName in _parkingSpotNames)
                    {
                        var existingParkingSpot = await context.ParkingSpots
                                                               .FirstOrDefaultAsync(p => p.Name == spotName && p.IdEstablishment == createdOrExistingEstablishment.Id);

                        if (existingParkingSpot == null)
                        {
                            var newParkingSpot = new ParkingSpot
                            {
                                Name = spotName,
                                IsAvailable = true,
                                IdEstablishment = createdOrExistingEstablishment.Id
                            };
                            context.ParkingSpots.Add(newParkingSpot);
                            Console.WriteLine($"- Estacionamiento '{spotName}' marcado para inserción para el establecimiento '{createdOrExistingEstablishment.Name}'.");
                        }
                        else
                        {
                            Console.WriteLine($"- Estacionamiento '{spotName}' ya existe para el establecimiento '{createdOrExistingEstablishment.Name}'.");
                        }
                    }
                    await context.SaveChangesAsync();
                    Console.WriteLine("Estacionamientos de prueba sembrados.");
                }
                else
                {
                    Console.WriteLine("No se pudo sembrar estacionamientos: El establecimiento de ejemplo no fue creado o encontrado.");
                }

                // --- 4. Sembrar Zonas de Prueba ---
                Console.WriteLine("Sembrando zonas de prueba...");

                if (createdOrExistingEstablishment != null)
                {
                    foreach (var zoneName in _zoneNames)
                    {
                        var existingZone = await context.Zones
                                                        .FirstOrDefaultAsync(z => z.Name == zoneName && z.EstablishmentId == createdOrExistingEstablishment.Id);

                        if (existingZone == null)
                        {
                            var newZone = new Zone
                            {
                                Name = zoneName,
                                EstablishmentId = createdOrExistingEstablishment.Id,
                                IsDeleted = false
                            };
                            context.Zones.Add(newZone);
                            Console.WriteLine($"- Zona '{zoneName}' marcada para inserción para el establecimiento '{createdOrExistingEstablishment.Name}'.");
                        }
                        else
                        {
                            Console.WriteLine($"- Zona '{zoneName}' ya existe para el establecimiento '{createdOrExistingEstablishment.Name}'.");
                        }
                    }
                    await context.SaveChangesAsync();
                    Console.WriteLine("Zonas de prueba sembradas.");
                }
                else
                {
                    Console.WriteLine("No se pudo sembrar zonas: El establecimiento de ejemplo no fue creado o encontrado.");
                }

                // --- 5. Sembrar Apartamentos de Prueba ---
                Console.WriteLine("Sembrando apartamentos de prueba...");

                if (createdOrExistingEstablishment != null)
                {
                    var zones = await context.Zones
                                             .Where(z => z.EstablishmentId == createdOrExistingEstablishment.Id)
                                             .ToListAsync();

                    foreach (var zone in zones)
                    {
                        if (_apartmentsByZone.TryGetValue(zone.Name, out var apartmentNamesForZone))
                        {
                            foreach (var apartmentName in apartmentNamesForZone)
                            {
                                var existingApartment = await context.Apartments
                                                                     .FirstOrDefaultAsync(a => a.Name == apartmentName && a.IdZone == zone.Id);

                                if (existingApartment == null)
                                {
                                    var newApartment = new Apartment
                                    {
                                        Name = apartmentName,
                                        IdZone = zone.Id,
                                        IsDeleted = false
                                    };
                                    context.Apartments.Add(newApartment);
                                    Console.WriteLine($"- Apartamento '{apartmentName}' marcado para inserción para la zona '{zone.Name}'.");
                                }
                                else
                                {
                                    Console.WriteLine($"- Apartamento '{apartmentName}' ya existe para la zona '{zone.Name}'.");
                                }
                            }
                        }
                    }
                    await context.SaveChangesAsync();
                    Console.WriteLine("Apartamentos de prueba sembrados.");
                }
                else
                {
                    Console.WriteLine("No se pudo sembrar apartamentos: El establecimiento de ejemplo no fue creado o encontrado.");
                }

                // --- 6. Sembrar Tipos de Visita de Prueba ---
                Console.WriteLine("Sembrando tipos de visita de prueba...");

                if (createdOrExistingEstablishment != null)
                {
                    foreach (var visitTypeName in _visitTypeNames)
                    {
                        var existingVisitType = await context.VisitTypes
                                                             .FirstOrDefaultAsync(vt => vt.Name == visitTypeName && vt.IdEstablishment == createdOrExistingEstablishment.Id);

                        if (existingVisitType == null)
                        {
                            var newVisitType = new VisitType
                            {
                                Name = visitTypeName,
                                IdEstablishment = createdOrExistingEstablishment.Id
                            };
                            context.VisitTypes.Add(newVisitType);
                            Console.WriteLine($"- Tipo de visita '{visitTypeName}' marcado para inserción para el establecimiento '{createdOrExistingEstablishment.Name}'.");
                        }
                        else
                        {
                            Console.WriteLine($"- Tipo de visita '{visitTypeName}' ya existe para el establecimiento '{createdOrExistingEstablishment.Name}'.");
                        }
                    }
                    await context.SaveChangesAsync();
                    Console.WriteLine("Tipos de visita de prueba sembrados.");
                }
                else
                {
                    Console.WriteLine("No se pudo sembrar tipos de visita: El establecimiento de ejemplo no fue creado o encontrado.");
                }

                // --- 7. Sembrar Áreas Comunes de Prueba ---
                Console.WriteLine("Sembrando áreas comunes de prueba...");

                if (createdOrExistingEstablishment != null)
                {
                    foreach (var commonAreaDetail in _commonAreaDetails)
                    {
                        var commonAreaName = commonAreaDetail.Name;
                        var commonAreaMode = commonAreaDetail.Mode;
                        var commonAreaMaxCapacity = commonAreaDetail.MaxCapacity;

                        var existingCommonArea = await context.CommonAreas
                                                               .FirstOrDefaultAsync(ca => ca.Name == commonAreaName && ca.IdEstablishment == createdOrExistingEstablishment.Id);

                        if (existingCommonArea == null)
                        {
                            var newCommonArea = new CommonArea
                            {
                                Name = commonAreaName,
                                IdEstablishment = createdOrExistingEstablishment.Id,
                                Mode = commonAreaMode,
                                MaxCapacity = commonAreaMaxCapacity,
                                Status = CommonAreaStatus.Available
                            };
                            context.CommonAreas.Add(newCommonArea);
                            Console.WriteLine($"- Área común '{commonAreaName}' (Modo: {commonAreaMode}) marcada para inserción para el establecimiento '{createdOrExistingEstablishment.Name}'.");
                        }
                        else
                        {
                            Console.WriteLine($"- Área común '{commonAreaName}' ya existe para el establecimiento '{createdOrExistingEstablishment.Name}'.");
                        }
                    }
                    await context.SaveChangesAsync();
                    Console.WriteLine("Áreas comunes de prueba sembradas.");
                }
                else
                {
                    Console.WriteLine("No se pudo sembrar áreas comunes: El establecimiento de ejemplo no fue creado o encontrado.");
                }

                // --- 8. Sembrar Personas de Prueba ---
                Console.WriteLine("Sembrando personas de prueba...");

                foreach (var personDetail in _personDetails)
                {
                    var names = personDetail.Names;
                    var lastNames = personDetail.LastNames;
                    var identificationNumber = personDetail.IdentificationNumber;

                    var existingPerson = await context.Persons
                                                       .FirstOrDefaultAsync(p => p.IdentificationNumber == identificationNumber);

                    if (existingPerson == null)
                    {
                        var newPerson = new Person
                        {
                            Names = names,
                            LastNames = lastNames,
                            IdentificationNumber = identificationNumber
                        };
                        context.Persons.Add(newPerson);
                        Console.WriteLine($"- Persona '{names} {lastNames}' (RUT: {identificationNumber}) marcada para inserción.");
                    }
                    else
                    {
                        Console.WriteLine($"- Persona '{names} {lastNames}' (RUT: {identificationNumber}) ya existe.");
                    }
                }
                await context.SaveChangesAsync();
                Console.WriteLine("Personas de prueba sembradas.");

                // --- 9. Sembrar Propietarios de Apartamentos de Prueba ---
                Console.WriteLine("Sembrando propietarios de apartamentos de prueba...");

                // Obtener las personas y apartamentos que acabamos de crear (o que ya existían)
                var people = await context.Persons.ToListAsync();
                var apartments = await context.Apartments.ToListAsync();

                foreach (var ownershipDetail in _ownershipDetails)
                {
                    var apartmentName = ownershipDetail.ApartmentName;
                    var personIdentificationNumber = ownershipDetail.PersonIdentificationNumber;

                    var apartment = apartments.FirstOrDefault(a => a.Name == apartmentName);
                    var person = people.FirstOrDefault(p => p.IdentificationNumber == personIdentificationNumber);

                    if (apartment != null && person != null)
                    {
                        // Verificar si esta relación de propiedad ya existe
                        var existingOwnership = await context.ApartmentOwnerships
                                                             .FirstOrDefaultAsync(ao => ao.IdApartment == apartment.Id && ao.IdPerson == person.Id && ao.EndDate == null);

                        if (existingOwnership == null)
                        {
                            var newOwnership = new ApartmentOwnership
                            {
                                IdApartment = apartment.Id,
                                IdPerson = person.Id,
                                StartDate = TimeHelper.GetSantiagoTime(), // Fecha actual de Santiago
                                EndDate = null // Propietario actual, por lo tanto EndDate es null
                            };
                            context.ApartmentOwnerships.Add(newOwnership);
                            Console.WriteLine($"- Propietario '{person.Names} {person.LastNames}' asignado al apartamento '{apartment.Name}'.");
                        }
                        else
                        {
                            Console.WriteLine($"- Propietario '{person.Names} {person.LastNames}' ya está asignado actualmente al apartamento '{apartment.Name}'.");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Advertencia: No se pudo asignar propietario. Apartamento '{apartmentName}' o Persona con RUT '{personIdentificationNumber}' no encontrada.");
                    }
                }
                await context.SaveChangesAsync(); // Se guardan todas las relaciones de propiedad.
                Console.WriteLine("Propietarios de apartamentos de prueba sembrados.");

                Console.WriteLine("Proceso de siembra de datos de DESARROLLO completado.");
            }
        }
    }
}