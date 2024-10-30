using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
using TrayReportApp.Data.Repositories;
using TrayReportApp.Models;
using TrayReportApp.Services.Contracts;
using TrayReportApp.Services.Models;

namespace TrayReportApp.Services
{
    public class TrayService : ITrayService
    {
        private readonly ITrayReportAppDbRepository _repo;
        private readonly ISupportService _supportService;
        private readonly ICableService _cableService;
        private readonly ICableTypeService _cableTypeService;

        public TrayService(ITrayReportAppDbRepository repo,
            ISupportService supportService,
            ICableService cableService,
            ICableTypeService cableTypeService)
        {
            _repo = repo;
            _supportService = supportService;
            _cableService = cableService;
            _cableTypeService = cableTypeService;
        }

        public async Task CreateTrayAsync(TrayServiceModel tray)
        {
            bool trayExists = await _repo.All<Tray>().AnyAsync(t => t.Name == tray.Name && t.Type == tray.Type);

            if (trayExists)
            {
                return;
            }

            TrayType? trayType = await _repo.GetByIdAsync<TrayType>(tray.TrayTypeId);
            int supportsCount = CalculateSupportsCount(tray);

            Tray newTray = new Tray
            {
                Name = tray.Name,
                Type = tray.Type,
                Length = tray.Length,
                Purpose = tray.Purpose,
                TrayTypeId = tray.TrayTypeId,
                TrayType = trayType,
                SupportsCount = supportsCount
            };

            await _repo.AddAsync(newTray);
            await _repo.SaveChangesAsync();

            newTray.Weight = CalculateTrayWeight(newTray);
            //newTray.FreeSpace = newTray.Length;
            //newTray.FreePercentage = 100;

            await _repo.SaveChangesAsync();
        }

        public async Task DeleteTrayAsync(int id)
        {
            var tray = this._repo.All<Tray>().FirstOrDefault(t => t.Id == id);
            if (tray == null)
            {
                return;
            }

            await _repo.DeleteAsync<Tray>(tray.Id);
            await _repo.SaveChangesAsync();
        }

        public async Task<TrayServiceModel> GetTrayAsync(int id)
        {
            var tray = await _repo.GetByIdAsync<Tray>(id);
            if (tray == null)
            {
                return null;
            }

            var trayCables = await GetTrayCablesAsync(tray.Name);

            return new TrayServiceModel
            {
                Id = tray.Id,
                Name = tray.Name,
                Type = tray.Type,
                Length = tray.Length,
                Weight = tray.Weight,
                Purpose = tray.Purpose,
                TrayTypeId = tray.TrayTypeId,
                FreeSpace = tray.FreeSpace,
                FreePercentage = tray.FreePercentage,
                Cables = trayCables,
                SupportsCount = tray.SupportsCount                
            };
        }

        public async Task<List<TrayServiceModel>> GetTraysAsync()
        {
            var trays = await _repo.All<Tray>()
                .ToListAsync();
            return trays.Select(t => new TrayServiceModel
            {
                Id = t.Id,
                Name = t.Name,
                Type = t.Type,
                Length = t.Length,
                Weight = t.Weight,
                Purpose = t.Purpose,
                TrayTypeId = t.TrayTypeId,
                FreeSpace = t.FreeSpace,
                FreePercentage = t.FreePercentage,
                SupportsCount = t.SupportsCount,
                Cables = GetTrayCablesAsync(t.Name).Result                
            }).ToList();
        }

        public async Task UpdateTrayAsync(TrayServiceModel tray)
        {
            var existingTray = await _repo.All<Tray>().FirstOrDefaultAsync(t => t.Id == tray.Id);

            if (existingTray == null)
            {
                return;
            }

            existingTray.Name = tray.Name;
            existingTray.Type = tray.Type;
            existingTray.Length = tray.Length;
            existingTray.Purpose = tray.Purpose;
            existingTray.TrayTypeId = tray.TrayTypeId;

            if (existingTray.Type != tray.Type)
            {
                existingTray.Type = tray.Type;
                existingTray.SupportsCount = CalculateSupportsCount(tray);
            }

            await _repo.SaveChangesAsync();
        }

        //public async Task UploadFromFileAsync(IBrowserFile file)
        //{
        //    List<TrayServiceModel> trays = new List<TrayServiceModel>();
        //    string? value = null;

        //    using MemoryStream memoryStream = new MemoryStream();
        //    await file.OpenReadStream().CopyToAsync(memoryStream);
        //    memoryStream.Position = 0;
        //    MemoryStream stream = memoryStream;

        //    using SpreadsheetDocument document = SpreadsheetDocument.Open(stream, false);

        //    if (document is not null)
        //    {
        //        WorkbookPart workbookPart = document.WorkbookPart;
        //        WorksheetPart worksheetPart = workbookPart.WorksheetParts.First();
        //        SheetData sheetData = worksheetPart.Worksheet.Elements<SheetData>().First();

        //        for (int i = 1; i < sheetData.Elements<Row>().Count(); i++)
        //        {
        //            TrayServiceModel tray = new TrayServiceModel();

        //            Row row = sheetData.Elements<Row>().ElementAt(i);
        //            // get the row number from outerxml
        //            int rowNumber = int.Parse(row.RowIndex);


        //            foreach (Cell cell in row.Elements<Cell>())
        //            {
        //                value = cell.InnerText;

        //                if (cell.DataType != null && cell.DataType == CellValues.SharedString)
        //                {
        //                    SharedStringTablePart stringTablePart = workbookPart.GetPartsOfType<SharedStringTablePart>().FirstOrDefault();
        //                    if (stringTablePart != null)
        //                        value = stringTablePart.SharedStringTable.ElementAt(int.Parse(value)).InnerText;
        //                }

        //                if (cell.CellReference == "A" + rowNumber)
        //                {
        //                    tray.Name = value;
        //                }
        //                else if (cell.CellReference == "B" + rowNumber)
        //                {
        //                    tray.Type = value;
        //                }
        //                else if (cell.CellReference == "C" + rowNumber)
        //                {
        //                    tray.Length = double.Parse(value);
        //                }
        //                else if (cell.CellReference == "D" + rowNumber)
        //                {
        //                    tray.Purpose = value;
        //                }
        //                else if (cell.CellReference == "E" + rowNumber)
        //                {
        //                    tray.TrayTypeId = int.Parse(value);
        //                }
        //            }

        //            //tray.SupportId = GetSupportTypeAsync(tray.Type).Result;

        //            trays.Add(tray);
        //        }

        //        foreach (var tray in trays)
        //        {
        //            await CreateTrayAsync(tray);
        //        }
        //    }
        //}

        private async Task<List<CableServiceModel>> GetTrayCablesAsync(string trayName)
        {
            var cables = await _cableService.GetCablesAsync();
            return cables.Where(c => c.Routing.Split('/', StringSplitOptions.RemoveEmptyEntries)
                                                            .Any(segment => string.Equals(segment, trayName, StringComparison.OrdinalIgnoreCase)))
                                                            .ToList();
        }

        private int CalculateSupportsCount(TrayServiceModel tray)
        {
            var traySupportDistance = _supportService.GetSupportAsync(tray.Type).Result.Distance;
            var trayLength = tray.Length;

            if (trayLength <= 0)
            {
                return 0;
            }

            int supportsCount = Math.Max(2, (int)Math.Ceiling((decimal)trayLength / (decimal)traySupportDistance));

            if (trayLength > traySupportDistance * 1.2)
            {
                supportsCount++;
            }

            return supportsCount;
        }

        private double CalculateTrayWeight(Tray tray)
        {
            double trayWeight = 0;

            if (tray != null)
            {
                var cables = GetTrayCablesAsync(tray.Name).Result;

                double supportWeight = tray.TrayType.Supports.Weight;
                int supportsCount = tray.SupportsCount;
                trayWeight = supportWeight * supportsCount;

                trayWeight += tray.Length * tray.TrayType.Weight;

                foreach (var cable in cables)
                {
                    var cableType = cable.Type;

                    if (cableType == null)
                    {
                        continue;
                    }

                    double cableWeight = _cableTypeService.GetCableTypeAsync(cableType).Result.Weight;
                    trayWeight += cableWeight * tray.Length;
                }
            }

            return trayWeight;
        }

        private double CalculateFreeSpace(Tray tray)
        {
            return 0;
        }

        private double CalculateFreePercentage(Tray tray)
        {
            return 0;
        }
    }
}
