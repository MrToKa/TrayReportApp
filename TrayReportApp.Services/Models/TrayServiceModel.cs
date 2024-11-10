namespace TrayReportApp.Services.Models
{
    public class TrayServiceModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public double Length { get; set; }
        public double? Weight { get; set; }
        public  string Purpose { get; set; }
        public int SupportsCount { get; set; }
        public double? FreeSpace { get; set; }
        public double? FreePercentage { get; set; }
        public string? ReportType { get; set; }
        public int TrayTypeId { get; set; }
    }
}
