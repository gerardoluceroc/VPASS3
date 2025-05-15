namespace VPASS3_backend.DTOs.AuditLogs
{
    public class AuditLogDto
    {
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
