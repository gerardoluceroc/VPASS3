namespace VPASS3_backend.DTOs.Visits
{
    public class VisitDto
    {
        public int? EstablishmentId { get; set; }

        public int IdPerson { get; set; }

        public int ZoneId { get; set; }

        public int IdDirection { get; set; }

        public int? IdApartment {  get; set; }

        public bool VehicleIncluded { get; set; }

        public TimeSpan? AuthorizedTime { get; set; }

        public string? LicensePlate { get; set; }

        public int? IdParkingSpot { get; set; }

        public int IdVisitType { get; set; }
    }
}

