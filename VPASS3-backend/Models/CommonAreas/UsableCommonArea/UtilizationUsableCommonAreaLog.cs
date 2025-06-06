using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace VPASS3_backend.Models.CommonAreas.UsableCommonArea
{
    public class UtilizationUsableCommonAreaLog
    {
        [Key]
        public int Id { get; set; }

        public int IdUtilizationUsableCommonAreaLog { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public int IdVisitor { get; set; }

        [ForeignKey("IdVisitor")]
        public Visitor Visitor { get; set; }

        public int? GuestsNumber { get; set; }

        [ForeignKey("IdUtilizationUsableCommonAreaLog")]
        public UsableCommonArea CommonArea { get; set; }
    }
}
