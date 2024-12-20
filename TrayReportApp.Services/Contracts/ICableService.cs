﻿using Microsoft.AspNetCore.Components.Forms;
using TrayReportApp.Services.Models;

namespace TrayReportApp.Services.Contracts
{
    public interface ICableService
    {
        Task CreateCableAsync(CableServiceModel cable);

        Task DeleteCableAsync(int id);

        Task<CableServiceModel> GetCableAsync(int id);

        Task<List<CableServiceModel>> GetCablesAsync();

        Task UpdateCableAsync(CableServiceModel cable);

        Task UploadFromFileAsync(IBrowserFile file);

        Task ExportTableEntriesAsync();

        Task ExportFilteredTableEntriesAsync(IEnumerable<CableServiceModel> cables);
    }
}
