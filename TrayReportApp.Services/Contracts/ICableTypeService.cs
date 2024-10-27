using Microsoft.AspNetCore.Components.Forms;
using TrayReportApp.Services.Models;

namespace TrayReportApp.Services.Contracts
{
    public interface ICableTypeService
    {
        Task<List<CableTypeServiceModel>> GetCableTypesAsync();
        Task<CableTypeServiceModel> GetCableTypeAsync(int id);
        Task<CableTypeServiceModel> CreateCableTypeAsync(CableTypeServiceModel cableType);
        Task<CableTypeServiceModel> UpdateCableTypeAsync(CableTypeServiceModel cableType);
        Task DeleteCableTypeAsync(int id);
        Task UploadFromFileAsync(IBrowserFile file);
        Task<List<string>> GetCablesPurposes();
    }
}
