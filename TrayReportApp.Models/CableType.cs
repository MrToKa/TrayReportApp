using System.ComponentModel.DataAnnotations;
using TrayReportApp.Models.Enum;

namespace TrayReportApp.Models
{
    public class CableType
    {
        public CableType()
        {
            Cables = new List<Cable>();
        }
        [Key]
        public int Id { get; set; }
        public Purpose Purpose { get; set; }
        public string Type { get; set; }
        public double Diameter { get; set; }
        public double Weight { get; set; }
        public List<Cable> Cables { get; set; }
    }
}
