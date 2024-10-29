namespace TrayReportApp.Services.Models
{
    public class TrayTypeServiceModel
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int? Length { get; set; }
        public double Weight { get; set; }
        public int SupportId { get; set; }
    }
}
