using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using VPASS3_backend.DTOs;
using VPASS3_backend.Interfaces;
using VPASS3_backend.Models;

namespace VPASS3_backend.Services
{
    public class AuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IAuditLogService _auditLogService;

        public AuthService(UserManager<User> userManager, IConfiguration configuration, IAuditLogService auditLogService)
        {
            _userManager = userManager;
            _configuration = configuration;
            _auditLogService = auditLogService;
        }

        public async Task<ResponseDto> LoginAsync(string email, string password)
        {
            try
            {
                // Forzar error 500 manualmente para pruebas
                //throw new Exception("Error interno simulado desde AuthService.LoginAsync");

                // Buscar al usuario por email
                var user = await _userManager.Users
                    .Include(u => u.establishment)
                    .FirstOrDefaultAsync(u => u.Email == email);

                if (user == null || !await _userManager.CheckPasswordAsync(user, password))
                {
                    // Log del intento fallido si el usuario existe
                    if (user != null)
                    {
                        await _auditLogService.LogManualAsync(
                            action: "Intento de login fallido",
                            email: user.Email,
                            role: "DESCONOCIDO",
                            userId: user.Id,
                            endpoint: "/auth/login",
                            httpMethod: "POST",
                            statusCode: 401
                        );
                    }

                    return new ResponseDto(401, message: "Credenciales incorrectas.");
                }

                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email)
        };

                if (user.establishment != null)
                {
                    claims.Add(new Claim("establishment_id", user.establishment.Id.ToString()));
                }

                var roles = await _userManager.GetRolesAsync(user);
                foreach (var role in roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: _configuration["Jwt:Issuer"],
                    audience: _configuration["Jwt:Audience"],
                    claims: claims,
                    expires: DateTime.Now.AddHours(12),
                    signingCredentials: creds
                );

                // Registrar el login exitoso
                await _auditLogService.LogManualAsync(
                    action: "Inicio de sesión",
                    email: user.Email,
                    role: roles.FirstOrDefault() ?? "UNASSIGNED",
                    userId: user.Id,
                    endpoint: "/auth/login",
                    httpMethod: "POST",
                    statusCode: 200
                );

                return new ResponseDto(200, data: new { Token = new JwtSecurityTokenHandler().WriteToken(token) });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en LoginAsync: " + ex.Message);
                return new ResponseDto(500, message: "Error en el servidor.");
            }
        }
    }
}