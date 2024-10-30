using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrayReportApp.Models
{
    public class Cable
    {
        [Key]
        public int Id { get; set; }
        public string Tag { get; set; }
        public string? Type { get; set; }

        [ForeignKey(nameof(CableType))]
        public int? CableTypeId { get; set; }
        public CableType? CableType { get; set; }
        public string? FromLocation { get; set; }
        public string? ToLocation { get; set; }
        public string? Routing { get; set; }
    }
}