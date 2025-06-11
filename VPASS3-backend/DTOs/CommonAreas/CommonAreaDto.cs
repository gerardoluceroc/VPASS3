using VPASS3_backend.Models.CommonAreas.ReservableCommonArea;
using VPASS3_backend.Models.CommonAreas.UsableCommonArea;

namespace VPASS3_backend.DTOs.CommonAreas
{
    public class CommonAreaDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int IdEstablishment { get; set; }
        public int Type { get; set; }
        public int? MaxCapacity { get; set; }
        public List<UtilizationUsableCommonAreaLog>? UtilizationLogs { get; set; }
        public List<CommonAreaReservation>? Reservations { get; set; }
    }

}
