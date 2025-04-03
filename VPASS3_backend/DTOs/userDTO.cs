namespace VPASS3_backend.DTOs
{
    public class UserDTO
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
        public int RoleId { get; set; } // Sólo se necesita roleId
    }
}