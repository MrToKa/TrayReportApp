namespace TrayReportApp.Services.Models
{
    public class CableServiceModel
    {
        public int Id { get; set; }
        public string Tag { get; set; }
        public int? Type { get; set; }
        public string? FromLocation { get; set; }
        public string? ToLocation { get; set; }
        public string? Routing { get; set; }
    }
}
