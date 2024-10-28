using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrayReportApp.Models
{
    public class Tray
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public int? Width { get; set; }
        public int? Height { get; set; }
        public double? Length { get; set; }
        public double? Weight { get; set; }
        public string Purpose { get; set; }
        public int? SupportsCount { get; set; }
        public double? FreeSpace { get; set; }
        public double? FreePercentage { get; set; }
        [ForeignKey(nameof(Support))]
        public int SupportId { get; set; }
        public Support Supports { get; set; }
    }
}
