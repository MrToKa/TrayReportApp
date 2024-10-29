using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrayReportApp.Models
{
    public class TrayType
    {
        [Key]
        public int Id { get; set; }
        public string Type { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int? Length { get; set; }
        public double Weight { get; set; }
        [ForeignKey(nameof(Support))]
        public int SupportId { get; set; }
        public Support Supports { get; set; }
    }
}
