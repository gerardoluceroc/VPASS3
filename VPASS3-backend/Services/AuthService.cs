using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using VPASS3_backend.DTOs;
using VPASS3_backend.Models;

namespace VPASS3_backend.Services
{
    public class AuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;

        public AuthService(UserManager<User> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
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
                    return new ResponseDto(401, message: "Credenciales incorrectas.");
                }

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email)
                };

                //  Claim del establecimiento desde la propiedad de navegación
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
                    expires: DateTime.Now.AddHours(1),
                    signingCredentials: creds
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
