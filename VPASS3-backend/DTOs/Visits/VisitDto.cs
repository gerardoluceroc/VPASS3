namespace VPASS3_backend.DTOs.Visits
{
    public class VisitDto
    {
        public int EstablishmentId { get; set; }

        public int VisitorId { get; set; }

        public int ZoneId { get; set; }

        public DateTime EntryDate { get; set; }

        public int IdDirection { get; set; }
    }
}
