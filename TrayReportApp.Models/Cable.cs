using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrayReportApp.Models
{
    public class Cable
    {
        public Cable()
        {
            Routing = new List<Tray>();
        }

        [Key]
        public int Id { get; set; }
        public string Tag { get; set; }

        [ForeignKey(nameof(CableType))]
        public int? Type { get; set; }
        public CableType? CableType { get; set; }
        public string? FromLocation { get; set; }
        public string? ToLocation { get; set; }
        public List<Tray> Routing { get; set; }
    }
}