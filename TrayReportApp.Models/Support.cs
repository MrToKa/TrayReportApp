using System.ComponentModel.DataAnnotations;

namespace TrayReportApp.Models
{
    public class Support
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public int Weight { get; set; }
        public int Distance { get; set; }
    }
}
