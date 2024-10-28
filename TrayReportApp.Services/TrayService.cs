﻿using DocumentFormat.OpenXml.Drawing;
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

        public TrayService(ITrayReportAppDbRepository repo,
            ISupportService supportService,
            ICableService cableService)
        {
            _repo = repo;
            _supportService = supportService;
            _cableService = cableService;
        }

        public async Task<TrayServiceModel> CreateTrayAsync(TrayServiceModel tray)
        {
            bool trayExists = await _repo.All<Tray>().AnyAsync(t => t.Name == tray.Name && t.Type == tray.Type);

            if (trayExists)
            {
                throw new Exception("Tray already exists");
            }

            List<SupportServiceModel> supportsId = await _supportService.GetSupportsAsync();


            Tray newTray = new Tray
            {
                Name = tray.Name,
                Type = tray.Type,
                Width = tray.Width,
                Height = tray.Height,
                Length = tray.Length,
                Weight = tray.Weight,
                Purpose = tray.Purpose,
                SupportId = await GetSupportTypeAsync(tray.Type)
            };


            await _repo.AddAsync(newTray);
            await _repo.SaveChangesAsync();

            return new TrayServiceModel
            {
                Name = newTray.Name,
                Type = newTray.Type,
                Width = newTray.Width,
                Height = newTray.Height,
                Length = newTray.Length,
                Weight = newTray.Weight,
                Purpose = newTray.Purpose,
                SupportId = newTray.SupportId
            };
        }

        public async Task DeleteTrayAsync(int id)
        {
            var tray = this._repo.All<Tray>().FirstOrDefault(t => t.Id == id);
            if (tray == null)
            {
                throw new Exception("Tray not found");
            }

            await _repo.DeleteAsync<Tray>(tray.Id);
            await _repo.SaveChangesAsync();
        }

        public async Task<List<CableServiceModel>> GetCables(string trayName)
        {
            var cables = await _cableService.GetCablesAsync();
            return cables.Where(c => c.Routing.Split('/', StringSplitOptions.RemoveEmptyEntries).Contains(trayName)).ToList();
        }

        public int CalculateSupportsCount(Tray tray)
        {
            var traySupportDistance = tray.Supports.Distance;
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

        public async Task<TrayServiceModel> GetTrayAsync(int id)
        {
            var tray = await _repo.GetByIdAsync<Tray>(id);
            if (tray == null)
            {
                throw new Exception("Tray not found");
            }

            var trayCables = await GetCables(tray.Name);

            return new TrayServiceModel
            {
                Id = tray.Id,
                Name = tray.Name,
                Type = tray.Type,
                Width = tray.Width,
                Height = tray.Height,
                Length = tray.Length,
                Weight = tray.Weight,
                Purpose = tray.Purpose,
                SupportsCount = CalculateSupportsCount(tray),
                //FreeSpace = tray.FreeSpace,
                //FreePercentage = tray.FreePercentage,
                SupportId = tray.SupportId,
                Cables = trayCables
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
                Width = t.Width,
                Height = t.Height,
                Length = t.Length,
                Weight = t.Weight,
                Purpose = t.Purpose,
                SupportsCount = CalculateSupportsCount(t),
                //FreeSpace = t.FreeSpace,
                //FreePercentage = t.FreePercentage,
                SupportId = t.SupportId
            }).ToList();
        }

        public async Task<TrayServiceModel> UpdateTrayAsync(TrayServiceModel tray)
        {
            var existingTray = await _repo.All<Tray>().FirstOrDefaultAsync(t => t.Id == tray.Id);

            if (existingTray == null)
            {
                throw new Exception("Tray not found");
            }

            existingTray.Name = tray.Name;
            existingTray.Type = tray.Type;
            existingTray.Width = tray.Width;
            existingTray.Height = tray.Height;
            existingTray.Length = tray.Length;
            existingTray.Weight = tray.Weight;
            existingTray.Purpose = tray.Purpose;

            if(existingTray.Type != tray.Type)
            {
                existingTray.Type = tray.Type;
                existingTray.SupportId = await GetSupportTypeAsync(tray.Type);
            }

            await _repo.SaveChangesAsync();

            return new TrayServiceModel
            {
                Id = existingTray.Id,
                Name = existingTray.Name,
                Type = existingTray.Type,
                Width = existingTray.Width,
                Height = existingTray.Height,
                Length = existingTray.Length,
                Weight = existingTray.Weight,
                Purpose = existingTray.Purpose,
                SupportsCount = existingTray.SupportsCount,
            };
        }

        public async Task UploadFromFileAsync(IBrowserFile file)
        {
            List<TrayServiceModel> trays = new List<TrayServiceModel>();
            string? value = null;

            using MemoryStream memoryStream = new MemoryStream();
            await file.OpenReadStream().CopyToAsync(memoryStream);
            memoryStream.Position = 0;
            MemoryStream stream = memoryStream;

            using SpreadsheetDocument document = SpreadsheetDocument.Open(stream, false);

            if (document is not null)
            {
                WorkbookPart workbookPart = document.WorkbookPart;
                WorksheetPart worksheetPart = workbookPart.WorksheetParts.First();
                SheetData sheetData = worksheetPart.Worksheet.Elements<SheetData>().First();

                for (int i = 1; i < sheetData.Elements<Row>().Count(); i++)
                {
                    TrayServiceModel tray = new TrayServiceModel();

                    Row row = sheetData.Elements<Row>().ElementAt(i);
                    // get the row number from outerxml
                    int rowNumber = int.Parse(row.RowIndex);


                    foreach (Cell cell in row.Elements<Cell>())
                    {
                        value = cell.InnerText;

                        if (cell.DataType != null && cell.DataType == CellValues.SharedString)
                        {
                            SharedStringTablePart stringTablePart = workbookPart.GetPartsOfType<SharedStringTablePart>().FirstOrDefault();
                            if (stringTablePart != null)
                                value = stringTablePart.SharedStringTable.ElementAt(int.Parse(value)).InnerText;
                        }

                        if (cell.CellReference == "A" + rowNumber)
                        {
                            tray.Name = value;
                        }
                        else if (cell.CellReference == "B" + rowNumber)
                        {
                            tray.Type = value;
                        }
                        else if (cell.CellReference == "C" + rowNumber)
                        {
                            if (value == null || value == string.Empty)
                            {
                                tray.Width = null;
                            }
                            else
                            {
                                tray.Width = int.Parse(value);
                            }
                        }
                        else if (cell.CellReference == "D" + rowNumber)
                        {
                            if (value == null || value == string.Empty)
                            {
                                tray.Height = null;
                            }
                            else
                            {
                                tray.Height = int.Parse(value);
                            }
                        }
                        else if (cell.CellReference == "E" + rowNumber)
                        {
                            if (value == null || value == string.Empty)
                            {
                                tray.Length = null;
                            }
                            else
                            {
                                tray.Length = double.Parse(value);
                            }
                        }
                        else if (cell.CellReference == "F" + rowNumber)
                        {
                            if (value == null || value == string.Empty)
                            {
                                tray.Weight = null;
                            }
                            else
                            {
                                tray.Weight = double.Parse(value);
                            }
                        }
                        else if (cell.CellReference == "G" + rowNumber)
                        {
                            tray.Purpose = value;
                        }
                    }

                    tray.SupportId = GetSupportTypeAsync(tray.Type).Result;

                    trays.Add(tray);
                }

                foreach (var tray in trays)
                {
                    await CreateTrayAsync(tray);
                }
            }
        }

        private async Task<int> GetSupportTypeAsync(string trayType)
        {
            var supports = await _supportService.GetSupportsAsync();

            var support = trayType.StartsWith("KL") ? 
                supports.FirstOrDefault(s => s.Name.Contains("KL")) :
                          trayType.StartsWith("WSL") ? 
                          supports.FirstOrDefault(s => s.Name.Contains("WSL")) :
                          null;

            if (support == null)
            {
                throw new Exception("Support not found");
            }

            return support.Id;
        }
    }
}
