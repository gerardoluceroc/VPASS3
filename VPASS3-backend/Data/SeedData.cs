using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VPASS3_backend.Context;
using VPASS3_backend.Models;

namespace VPASS3_backend.Data
{
    public static class SeedData
    {
        // --- Variables de Configuración ---
        // Nombres literales de los roles
        private const string _superAdminRoleName = "SUPERADMIN";
        public static readonly string _adminRoleName = "ADMIN";

        // IDs específicos para los roles (siempre que se haga la inserción SQL directa)
        private const int _superAdminRoleId = 1;
        private const int _adminRoleId = 2;

        // Datos del usuario SUPERADMIN
        private const string _superAdminEmail = "superadmin@vpass3.cl";
        private const string _superAdminPassword = "Clave123."; // ADVERTENCIA: Considerar usar Secrets o Variables de Entorno en producción.
        private const string _superAdminUserName = "SUPERADMIN";

        // Nombres literales de las direcciones de visita
        private const string _entryDirectionName = "Entrada";
        private const string _exitDirectionName = "Salida";

        // IDs específicos para las direcciones de visita
        private const int _entryDirectionId = 1;
        private const int _exitDirectionId = 2;
        // --- Fin Variables de Configuración ---


        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var services = scope.ServiceProvider;
                var userManager = services.GetRequiredService<UserManager<User>>();
                var roleManager = services.GetRequiredService<RoleManager<Role>>();
                var context = services.GetRequiredService<AppDbContext>();

                Console.WriteLine("Iniciando proceso de siembra de datos...");

                // Se aplica las migraciones antes de cualquier operación de siembra.
                Console.WriteLine("Aplicando migraciones si es necesario...");
                await context.Database.MigrateAsync();
                Console.WriteLine("Migraciones aplicadas.");

                // --- 1. Sembrar Roles con IDs específicos ---
                Console.WriteLine("Sembrando roles con IDs específicos...");

                // Se verifica qué roles necesitan ser insertados.
                var existingRoles = await context.Roles.Where(r => r.Id == _superAdminRoleId || r.Id == _adminRoleId).ToListAsync();
                var rolesToInsert = new List<(int Id, string Name)>();

                if (!existingRoles.Any(r => r.Id == _superAdminRoleId))
                {
                    rolesToInsert.Add((_superAdminRoleId, _superAdminRoleName));
                    Console.WriteLine($"- Rol '{_superAdminRoleName}' (ID: {_superAdminRoleId}) marcado para inserción.");
                }
                else
                {
                    Console.WriteLine($"- Rol '{_superAdminRoleName}' (ID: {_superAdminRoleId}) ya existe.");
                }

                if (!existingRoles.Any(r => r.Id == _adminRoleId))
                {
                    rolesToInsert.Add((_adminRoleId, _adminRoleName));
                    Console.WriteLine($"- Rol '{_adminRoleName}' (ID: {_adminRoleId}) marcado para inserción.");
                }
                else
                {
                    Console.WriteLine($"- Rol '{_adminRoleName}' (ID: {_adminRoleId}) ya existe.");
                }

                if (rolesToInsert.Any())
                {
                    // Se inicia una transacción para asegurar la consistencia del IDENTITY_INSERT.
                    using (var transaction = await context.Database.BeginTransactionAsync())
                    {
                        try
                        {
                            // Se habilita IDENTITY_INSERT para la tabla AspNetRoles.
                            await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT AspNetRoles ON;");

                            foreach (var roleData in rolesToInsert)
                            {
                                string concurrencyStamp = Guid.NewGuid().ToString(); // Se genera un nuevo GUID para ConcurrencyStamp.

                                await context.Database.ExecuteSqlInterpolatedAsync($@"
                                    INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
                                    VALUES ({roleData.Id}, {roleData.Name}, {roleData.Name.ToUpper()}, {concurrencyStamp});
                                ");
                            }

                            // Se deshabilita IDENTITY_INSERT después de la inserción.
                            await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT AspNetRoles OFF;");

                            // Se confirma la transacción.
                            await transaction.CommitAsync();
                            Console.WriteLine("Roles sembrados con IDs específicos y transacción completada.");
                        }
                        catch (Exception ex)
                        {
                            // En caso de error, se revierte la transacción.
                            await transaction.RollbackAsync();
                            Console.WriteLine($"Error al sembrar roles con IDs específicos: {ex.Message}");
                            Console.WriteLine("Transacción revertida.");
                            throw; // Se relanza la excepción.
                        }
                    }
                }
                else
                {
                    Console.WriteLine("No se necesitan sembrar roles nuevos con IDs específicos.");
                }

                // --- 2. Sembrar Usuario SUPERADMIN ---
                Console.WriteLine("Sembrando usuario SUPERADMIN...");

                if (await userManager.FindByEmailAsync(_superAdminEmail) == null)
                {
                    var superAdminUser = new User
                    {
                        UserName = _superAdminUserName,
                        Email = _superAdminEmail,
                        EmailConfirmed = true,
                        EstablishmentId = null
                    };

                    var result = await userManager.CreateAsync(superAdminUser, _superAdminPassword);

                    if (result.Succeeded)
                    {
                        if (await roleManager.RoleExistsAsync(_superAdminRoleName))
                        {
                            await userManager.AddToRoleAsync(superAdminUser, _superAdminRoleName);
                            Console.WriteLine($"- Usuario '{_superAdminEmail}' ({_superAdminUserName}) creado y rol '{_superAdminRoleName}' asignado.");
                        }
                        else
                        {
                            Console.WriteLine($"Advertencia: El rol '{_superAdminRoleName}' no existe, no se pudo asignar al usuario '{_superAdminEmail}'.");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Error al crear usuario '{_superAdminEmail}':");
                        foreach (var error in result.Errors)
                        {
                            Console.WriteLine($"- {error.Description}");
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"- Usuario '{_superAdminEmail}' ({_superAdminUserName}) ya existe.");

                    var existingSuperAdminUser = await userManager.FindByEmailAsync(_superAdminEmail);
                    if (existingSuperAdminUser != null && !await userManager.IsInRoleAsync(existingSuperAdminUser, _superAdminRoleName))
                    {
                        if (await roleManager.RoleExistsAsync(_superAdminRoleName))
                        {
                            await userManager.AddToRoleAsync(existingSuperAdminUser, _superAdminRoleName);
                            Console.WriteLine($"- Rol '{_superAdminRoleName}' asignado a usuario existente '{_superAdminEmail}'.");
                        }
                    }
                }
                Console.WriteLine("Usuario SUPERADMIN sembrado.");

                // --- 3. Sembrar Direcciones de Visita con IDs específicos ---
                Console.WriteLine("Sembrando direcciones de visita...");

                // Se verifica qué direcciones necesitan ser insertadas.
                var existingDirections = await context.Directions.Where(d => d.Id == _entryDirectionId || d.Id == _exitDirectionId).ToListAsync();
                var directionsToInsert = new List<(int Id, string VisitDirection)>();

                if (!existingDirections.Any(d => d.Id == _entryDirectionId))
                {
                    directionsToInsert.Add((_entryDirectionId, _entryDirectionName));
                    Console.WriteLine($"- Dirección '{_entryDirectionName}' (ID: {_entryDirectionId}) marcada para inserción.");
                }
                else
                {
                    Console.WriteLine($"- Dirección '{_entryDirectionName}' (ID: {_entryDirectionId}) ya existe.");
                }

                if (!existingDirections.Any(d => d.Id == _exitDirectionId))
                {
                    directionsToInsert.Add((_exitDirectionId, _exitDirectionName));
                    Console.WriteLine($"- Dirección '{_exitDirectionName}' (ID: {_exitDirectionId}) marcada para inserción.");
                }
                else
                {
                    Console.WriteLine($"- Dirección '{_exitDirectionName}' (ID: {_exitDirectionId}) ya existe.");
                }

                if (directionsToInsert.Any())
                {
                    // Se inicia una transacción para asegurar la consistencia del IDENTITY_INSERT.
                    using (var transaction = await context.Database.BeginTransactionAsync())
                    {
                        try
                        {
                            // Se habilita IDENTITY_INSERT para la tabla Directions.
                            await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Directions ON;");

                            foreach (var directionData in directionsToInsert)
                            {
                                await context.Database.ExecuteSqlInterpolatedAsync($@"
                                    INSERT INTO Directions (Id, VisitDirection)
                                    VALUES ({directionData.Id}, {directionData.VisitDirection});
                                ");
                            }

                            // Se deshabilita IDENTITY_INSERT después de la inserción.
                            await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Directions OFF;");

                            // Se confirma la transacción.
                            await transaction.CommitAsync();
                            Console.WriteLine("Direcciones de visita sembradas con IDs específicos y transacción completada.");
                        }
                        catch (Exception ex)
                        {
                            // En caso de error, se revierte la transacción.
                            await transaction.RollbackAsync();
                            Console.WriteLine($"Error al sembrar direcciones de visita con IDs específicos: {ex.Message}");
                            Console.WriteLine("Transacción revertida.");
                            throw; // Se relanza la excepción.
                        }
                    }
                }
                else
                {
                    Console.WriteLine("No se necesitan sembrar direcciones de visita nuevas con IDs específicos.");
                }

                Console.WriteLine("Proceso de siembra de datos completado.");
            }
        }
    }
}
