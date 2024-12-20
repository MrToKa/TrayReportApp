﻿namespace TrayReportApp.Services.Models
{
    public class CableServiceModel
    {
        public int Id { get; set; }
        public string Tag { get; set; }
        public string? Type { get; set; }
        public int? CableTypeId { get; set; }
        public string? FromLocation { get; set; }
        public string? ToLocation { get; set; }
        public string? Routing { get; set; }
    }
}
