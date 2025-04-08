namespace VPASS3_backend.DTOs
{
    public class ResponseDto
    {
        public int StatusCode { get; set; }

        public object? Data { get; set; } = null;

        public string? Message { get; set; } = null;

        public ResponseDto() { }

        public ResponseDto(int statusCode, object? data = null, string? message = null)
        {
            StatusCode = statusCode;
            Data = data;
            Message = message;
        }
    }
}
