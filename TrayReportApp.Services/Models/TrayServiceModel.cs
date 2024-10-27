namespace TrayReportApp.Services.Models
{
    public class TrayServiceModel
    {
        public TrayServiceModel() 
        {
            Cables = new List<TrayCableServiceModel>();
        }

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
        public int SupportId { get; set; }
        public List<TrayCableServiceModel> Cables { get; set; }
    }
}
