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

        public TrayService(ITrayReportAppDbRepository repo,
            ISupportService supportService)
        {
            _repo = repo;
            _supportService = supportService;
        }

        public async Task<TrayServiceModel> CreateTrayAsync(TrayServiceModel tray)
        {
            bool trayExists = await _repo.All<Tray>().AnyAsync(t => t.Name == tray.Name && t.Type == tray.Type);

            if (trayExists)
            {
                throw new Exception("Tray already exists");
            }

            int supportId = await _supportService.GetSupportsAsync()
                .ContinueWith(t => t.Result.FirstOrDefault().Id);

            Tray newTray = new Tray
            {
                Name = tray.Name,
                Type = tray.Type,
                Width = tray.Width,
                Height = tray.Height,
                Length = tray.Length,
                Weight = tray.Weight,
                Purpose = tray.Purpose,
                SupportsCount = tray.SupportsCount,
                FreeSpace = tray.FreeSpace,
                FreePercentage = tray.FreePercentage,
                SupportId = supportId
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
                SupportsCount = newTray.SupportsCount,
                FreeSpace = newTray.FreeSpace,
                FreePercentage = newTray.FreePercentage,
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

        public async Task<TrayServiceModel> GetTrayAsync(int id)
        {
            var tray = await _repo.GetByIdAsync<Tray>(id);
            if (tray == null)
            {
                throw new Exception("Tray not found");
            }

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
                SupportsCount = tray.SupportsCount,
                FreeSpace = tray.FreeSpace,
                FreePercentage = tray.FreePercentage,
                SupportId = tray.SupportId,
                Cables = tray.Cables.Select(c => new TrayCableServiceModel
                {
                    CableId = c.Id,
                    TrayId = tray.Id
                }).ToList()
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
                SupportsCount = t.SupportsCount,
                FreeSpace = t.FreeSpace,
                FreePercentage = t.FreePercentage,
                SupportId = t.SupportId,
                Cables = t.Cables.Select(c => new TrayCableServiceModel
                {
                    CableId = c.Id,
                    TrayId = t.Id
                }).ToList()
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
            existingTray.SupportsCount = tray.SupportsCount;
            existingTray.FreeSpace = tray.FreeSpace;
            existingTray.FreePercentage = tray.FreePercentage;
            existingTray.SupportId = tray.SupportId;

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
                FreeSpace = existingTray.FreeSpace,
                FreePercentage = existingTray.FreePercentage,
                SupportId = existingTray.SupportId
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

                    var trayWithSupport = AddSupport(tray);

                    trays.Add(await trayWithSupport);
                }

                foreach (var tray in trays)
                {
                    await CreateTrayAsync(tray);
                }
            }
        }

        private async Task<TrayServiceModel> AddSupport(TrayServiceModel tray)
        {
            var supports = await _supportService.GetSupportsAsync();

            if (tray.Type.StartsWith("KL"))
            {
                var support = supports.Where(s => s.Name.Contains("KL")).FirstOrDefault();
                tray.SupportId = support.Id;
            }
            else if (tray.Type.StartsWith("WSL"))
            {
                var support = supports.Where(s => s.Name.Contains("WSL")).FirstOrDefault();
                tray.SupportId = support.Id;
            }
            else
            {
                throw new Exception("Support not found");
            }

            return tray;
        }
    }
}
