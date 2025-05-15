using System;

namespace VPASS3_backend.Models
{
    public class AuditLog
    {
        public int Id { get; set; }
        public string Action { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string HttpMethod { get; set; }
        public string Endpoint { get; set; }
        public int StatusCode { get; set; }
        public DateTime Timestamp { get; set; }
        public int? UserId { get; set; }
    }
}
