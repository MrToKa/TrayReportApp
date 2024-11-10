using Microsoft.AspNetCore.Components.Forms;
using TrayReportApp.Services.Models;

namespace TrayReportApp.Services.Contracts
{
    public interface ITrayTypeService 
    {
        Task<List<TrayTypeServiceModel>> GetTrayTypesAsync();
        Task<TrayTypeServiceModel> GetTrayTypeAsync(string type);
        Task<TrayTypeServiceModel> CreateTrayTypeAsync(TrayTypeServiceModel trayType);
        Task<TrayTypeServiceModel> UpdateTrayTypeAsync(TrayTypeServiceModel trayType);
        Task DeleteTrayTypeAsync(int id);
        Task UploadFromFileAsync(IBrowserFile file);
    }
}
