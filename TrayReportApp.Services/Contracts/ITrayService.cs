using Microsoft.AspNetCore.Components.Forms;
using TrayReportApp.Services.Models;

namespace TrayReportApp.Services.Contracts
{
    public interface ITrayService
    {
        Task<List<TrayServiceModel>> GetTraysAsync();
        Task<TrayServiceModel> GetTrayAsync(int id);
        Task<TrayServiceModel> CreateTrayAsync(TrayServiceModel tray);
        Task<TrayServiceModel> UpdateTrayAsync(TrayServiceModel tray);
        Task DeleteTrayAsync(int id);
        Task UploadFromFileAsync(IBrowserFile file);
    }
}
