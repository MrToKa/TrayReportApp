using System.ComponentModel.DataAnnotations;

namespace TrayReportApp.Models
{
    public class Support
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public double Weight { get; set; }
        public double Distance { get; set; }
    }
}
