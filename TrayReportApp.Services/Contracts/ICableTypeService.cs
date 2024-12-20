﻿using Microsoft.AspNetCore.Components.Forms;
using TrayReportApp.Services.Models;

namespace TrayReportApp.Services.Contracts
{
    public interface ICableTypeService
    {
        Task<List<CableTypeServiceModel>> GetCableTypesAsync();
        Task<CableTypeServiceModel> GetCableTypeAsync(string type);
        Task CreateCableTypeAsync(CableTypeServiceModel cableType);
        Task UpdateCableTypeAsync(CableTypeServiceModel cableType);
        Task DeleteCableTypeAsync(int id);
        Task UploadFromFileAsync(IBrowserFile file);
        Task<List<string>> GetCablesPurposes();
        Task ExportTableEntriesAsync();
        Task ExportFilteredTableEntriesAsync(IEnumerable<CableTypeServiceModel> cableTypes);
    }
}
