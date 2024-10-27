using System.ComponentModel.DataAnnotations.Schema;

namespace TrayReportApp.Models
{
    public class TrayCable
    {
        [ForeignKey(nameof(Tray))]
        public int? TrayId { get; set; }
        public Tray? Tray { get; set; }
        [ForeignKey(nameof(Cable))]
        public int? CableId { get; set; }
        public Cable? Cable { get; set; }
    }
}
