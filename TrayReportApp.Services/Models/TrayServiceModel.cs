namespace TrayReportApp.Services.Models
{
    public class TrayServiceModel
    {
        public TrayServiceModel() 
        {
            Cables = new List<CableServiceModel>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public double Length { get; set; }
        public double? Weight { get; set; }
        public string Purpose { get; set; }
        public int? SupportsCount { get; set; }
        public double? FreeSpace { get; set; }
        public double? FreePercentage { get; set; }
        public List<CableServiceModel> Cables { get; set; }
        public int TrayTypeId { get; set; }
    }
}
