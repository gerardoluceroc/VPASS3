using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace VPASS3_backend.Models.CommonAreas
{
    public class CommonAreaUsageLog
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        public TimeSpan? UsageTime { get; set; }

        public DateTime? EndTime => UsageTime.HasValue
            ? StartTime + UsageTime.Value
            : (DateTime?)null;

        [Required]
        public int IdPerson { get; set; }

        [ForeignKey("IdPerson")]
        public Person Person { get; set; }

        //// Invitados opcionales al uso
        //public ICollection<Person> InvitedGuests { get; set; } = new List<Person>();

        //// Número de invitados calculado dinámicamente
        //[NotMapped]
        //public int GuestsNumber => InvitedGuests?.Count ?? 0;

        // Número de invitados manual
        public int? GuestsNumber { get; set; } = 0;

        [Required]
        public int IdCommonArea { get; set; }

        [ForeignKey("IdCommonArea")]
        [JsonIgnore]
        public CommonArea CommonArea { get; set; }
    }
}