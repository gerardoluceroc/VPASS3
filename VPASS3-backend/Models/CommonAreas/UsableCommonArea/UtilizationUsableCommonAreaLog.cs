using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace VPASS3_backend.Models.CommonAreas.UsableCommonArea
{
    public class UtilizationUsableCommonAreaLog
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        public TimeSpan? UsageTime { get; set; }

        public int? GuestsNumber { get; set; }

        [Required]
        [JsonIgnore]
        public int IdPerson { get; set; }

        [ForeignKey("IdPerson")]
        public Person Person { get; set; }

        [Required]
        public int IdUsableCommonArea { get; set; }

        [ForeignKey("IdUsableCommonArea")]
        [JsonIgnore]
        public UsableCommonArea UsableCommonArea { get; set; }
    }
}
