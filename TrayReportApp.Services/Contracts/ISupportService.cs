﻿using TrayReportApp.Models;
using TrayReportApp.Services.Models;

namespace TrayReportApp.Services.Contracts
{
    public interface ISupportService
    {
        Task<List<SupportServiceModel>> GetSupportsAsync();
        Task<SupportServiceModel> GetSupportAsync(int id);
        Task<SupportServiceModel> CreateSupportAsync(SupportServiceModel support);
        Task<SupportServiceModel> UpdateSupportAsync(SupportServiceModel support);
        Task DeleteSupportAsync(int id);
    }
}