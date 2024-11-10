using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrayReportApp.Models
{
    public class Tray
    {
        [Key]
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Type { get; set; }
        public required string Purpose { get; set; }
        public double Length { get; set; }
        public double? Weight { get; set; }
        public int SupportsCount { get; set; }
        public double? FreeSpace { get; set; }
        public double? FreePercentage { get; set; }
        public string? ReportType { get; set; }
        [ForeignKey(nameof(TrayType))]
        public int TrayTypeId { get; set; }
        public required TrayType TrayType { get; set; }
    }
}
