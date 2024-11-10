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
        private readonly ICableService _cableService;
        private readonly ICableTypeService _cableTypeService;
        private readonly ITrayTypeService _trayTypeService;

        public TrayService(ITrayReportAppDbRepository repo,
            ICableService cableService,
            ICableTypeService cableTypeService,
            ITrayTypeService trayTypeService)
        {
            _repo = repo;
            _cableService = cableService;
            _cableTypeService = cableTypeService;
            _trayTypeService = trayTypeService;
        }

        public async Task CreateTrayAsync(TrayServiceModel tray)
        {
            bool trayExists = await _repo.All<Tray>().AnyAsync(t => t.Name == tray.Name && t.Type == tray.Type);

            if (trayExists)
            {
                return;
            }

            TrayType trayType = await _repo.All<TrayType>().FirstOrDefaultAsync(t => t.Type == tray.Type);

            Tray newTray = new Tray
            {
                Name = tray.Name,
                Type = tray.Type,
                Length = tray.Length,
                Purpose = tray.Purpose,
                ReportType = tray.ReportType,
                TrayType = trayType,
                TrayTypeId = trayType.Id,
            };

            await _repo.AddAsync(newTray);
            await _repo.SaveChangesAsync();
             
            newTray.SupportsCount = await CalculateSupportsCount(newTray);
            newTray.Weight = await CalculateTrayWeight(newTray);
            newTray.FreeSpace = await CalculateFreeSpace(newTray);
            newTray.FreePercentage = await CalculateFreePercentage(newTray);

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

            //var trayCables = await GetTrayCablesAsync(tray.Name);

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
                SupportsCount = tray.SupportsCount,
                ReportType = tray.ReportType
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
                ReportType = t.ReportType,               
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
            existingTray.ReportType = tray.ReportType;

            //if (existingTray.Type != tray.Type)
            //{
            //    existingTray.Type = tray.Type;
            //    existingTray.TrayTypeId = tray.TrayTypeId;
            //    existingTray.SupportsCount = CalculateSupportsCount(tray);
            //}

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

        //private async Task<List<Cable>> GetTrayCablesAsync(string trayName)
        //{
        //    var cables = await _repo.All<Cable>()
        //        .Include(c => c.CableType)
        //        .ToListAsync();

        //    return cables.Where(c => c.Routing.Split('/', StringSplitOptions.RemoveEmptyEntries)
        //                                                    .Any(segment => string.Equals(segment, trayName, StringComparison.OrdinalIgnoreCase)))
        //                                                    .ToList();
        //}

        private async Task<int> CalculateSupportsCount(Tray tray)
        {
            var traySupportDistance = await _repo.All<Support>()
                .Where(s => s.Id == tray.TrayType.SupportId)
                .Select(s => s.Distance)
                .FirstOrDefaultAsync();
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

        private async Task<double> CalculateTrayWeight(Tray tray)
        {
            double totalTrayWeight = 0;

            HashSet<string> missingTypes = new HashSet<string>();

            if (tray != null)
            {
                var cables = await _repo.All<Cable>()
                    .Include(c => c.CableType)
                    .ToListAsync();

                var filteredCables = cables
                    .Where(c => c.Routing != null && c.Routing.Split('/', StringSplitOptions.RemoveEmptyEntries)
                                                .Any(segment => string.Equals(segment, tray.Name, StringComparison.OrdinalIgnoreCase)))
                    .ToList();

                double supportWeight = tray.TrayType.Supports.Weight;
                int supportsCount = tray.SupportsCount;
                double supportsWeight = supportWeight * supportsCount;
                double supportsWeightPerMeter = supportsWeight / tray.Length;

                double trayWeightLoadPerMeter = tray.TrayType.Weight + supportsWeightPerMeter;
                totalTrayWeight = trayWeightLoadPerMeter * tray.Length;

                //if (tray.TrayType.Length.HasValue)
                //{
                //    trayWeight += (tray.Length * 1000) * (tray.TrayType.Weight / tray.TrayType.Length.Value);
                //}
                double cableWeight = 0;

                foreach (var cable in filteredCables)
                {
                    var cableType = cable.Type;

                    //if (cableType == null)
                    //{
                    //    missingTypes.Add("No type for cable assigned");
                    //    continue;
                    //}

                    var cableTypeEntity = _repo.All<CableType>().FirstOrDefault(ct => ct.Type == cableType);
                    if (cableTypeEntity != null)
                    {
                        cableWeight += cableTypeEntity.Weight / 1000;
                    }
                    else
                    {
                        missingTypes.Add(cableType);
                    }
                }

                double totalCablesWeight = cableWeight * tray.Length;
                totalTrayWeight += totalCablesWeight;

                //totalWeightPerMeter
                double totalWeightPerMeter = trayWeightLoadPerMeter + cableWeight;



                //if (missingTypes.Count > 0)
                //{
                //    throw new ArgumentException($"Cable types not found: {string.Join(", ", missingTypes)}");
                //}
            }

            return Math.Round(totalTrayWeight, 3);
        }

        private async Task<double> CalculateFreeSpace(Tray tray)
        {
            return 0;
        }

        private async Task<double> CalculateFreePercentage(Tray tray)
        {
            return 0;
        }
    }
}
