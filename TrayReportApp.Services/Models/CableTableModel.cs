namespace TrayReportApp.Services.Models
{
    public class CableTableModel
    {
        public int Id { get; set; }
        public string Tag { get; set; }
        public string? Type { get; set; }
        public string? Purpose { get; set; }
        public double? Diameter { get; set; }
        public double? Weight { get; set; }
        public string? FromLocation { get; set; }
        public string? ToLocation { get; set; }
        public string? Routing { get; set; }

    }
}
