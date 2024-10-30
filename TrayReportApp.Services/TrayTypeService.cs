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
    public class TrayTypeService : ITrayTypeService
    {
        private readonly ITrayReportAppDbRepository _repo;
        private readonly ISupportService _supportService;

        public TrayTypeService(ITrayReportAppDbRepository repo,
            ISupportService supportService)
        {
            _repo = repo;
            _supportService = supportService;
        }

        public async Task<TrayTypeServiceModel> CreateTrayTypeAsync(TrayTypeServiceModel trayType)
        {
            var trayTypeExists = await _repo.All<TrayType>().AnyAsync(t => t.Type == trayType.Type);

            if (trayTypeExists)
            {
                return null;
            }

            Support support = await _repo.All<Support>().FirstOrDefaultAsync(s => s.Id == trayType.SupportId);

            if (support == null) 
            {
                throw new Exception("Support not found! Check your input data!");
            }

            TrayType newTrayType = new TrayType
            {
                Type = trayType.Type,
                Width = trayType.Width,
                Height = trayType.Height,
                Length = trayType.Length,
                Weight = trayType.Weight,
                SupportId = support.Id,
                Supports = support,
            };

            await _repo.AddAsync(newTrayType);
            await _repo.SaveChangesAsync();

            return new TrayTypeServiceModel
            {
                Type = newTrayType.Type,
                Width = newTrayType.Width,
                Height = newTrayType.Height,
                Length = newTrayType.Length,
                Weight = newTrayType.Weight,
                SupportId = newTrayType.SupportId,
            };
        }

        public async Task DeleteTrayTypeAsync(int id)
        {
            var trayType = await _repo.All<TrayType>().FirstOrDefaultAsync(t => t.Id == id);

            if (trayType == null)
            {
                return;
            }

            await _repo.DeleteAsync<TrayType>(trayType.Id);
            await _repo.SaveChangesAsync();
        }

        public async Task<TrayTypeServiceModel> GetTrayTypeAsync(int id)
        {
            var trayType = await _repo.All<TrayType>()
                .Include(t => t.Supports)
                .Where(t => t.Id == id)
                .FirstOrDefaultAsync();

            if (trayType == null)
            {
                return null;
            }

            return new TrayTypeServiceModel
            {
                Type = trayType.Type,
                Width = trayType.Width,
                Height = trayType.Height,
                Length = trayType.Length,
                Weight = trayType.Weight,
                SupportId = trayType.SupportId,
            };
        }

        public async Task<List<TrayTypeServiceModel>> GetTrayTypesAsync()
        {
            return await _repo.All<TrayType>()
                .Include(t => t.Supports)
                .Select(t => new TrayTypeServiceModel
                {
                    Id = t.Id,
                    Type = t.Type,
                    Width = t.Width,
                    Height = t.Height,
                    Length = t.Length,
                    Weight = t.Weight,
                    SupportId = t.SupportId,
                })
                .ToListAsync();
        }

        public async Task<TrayTypeServiceModel> UpdateTrayTypeAsync(TrayTypeServiceModel trayType)
        {
            var existingTrayType = await _repo.All<TrayType>()
                .FirstOrDefaultAsync(t => t.Id == trayType.Id);

            if (existingTrayType == null)
            {
                return null;
            }

            existingTrayType.Type = trayType.Type;
            existingTrayType.Width = trayType.Width;
            existingTrayType.Height = trayType.Height;
            existingTrayType.Length = trayType.Length;
            existingTrayType.Weight = trayType.Weight;
            existingTrayType.SupportId = trayType.SupportId;

            await _repo.SaveChangesAsync();

            return new TrayTypeServiceModel
            {
                Type = existingTrayType.Type,
                Width = existingTrayType.Width,
                Height = existingTrayType.Height,
                Length = existingTrayType.Length,
                Weight = existingTrayType.Weight,
                SupportId = existingTrayType.SupportId,
            };
        }

        public async Task UploadFromFileAsync(IBrowserFile file)
        {
            List<TrayTypeServiceModel> trayTypes = new List<TrayTypeServiceModel>();
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
                    TrayTypeServiceModel trayType = new TrayTypeServiceModel();
                    Row row = sheetData.Elements<Row>().ElementAt(i);
                    int rowNumber = int.Parse(row.RowIndex);

                    foreach (Cell cell in row.Elements<Cell>())
                    {
                        value = cell.InnerText;

                        if (cell.DataType != null && cell.DataType == CellValues.SharedString)
                        {
                            SharedStringTablePart stringTablePart = workbookPart.SharedStringTablePart;
                            value = stringTablePart.SharedStringTable.ElementAt(int.Parse(value)).InnerText;
                        }

                        if (cell.CellReference == "A" + rowNumber)
                        {
                            trayType.Type = value;
                        }
                        else if (cell.CellReference == "B" + rowNumber)
                        {
                            trayType.Width = int.Parse(value);
                        }
                        else if (cell.CellReference == "C" + rowNumber)
                        {
                            trayType.Height = int.Parse(value);
                        }
                        else if (cell.CellReference == "D" + rowNumber)
                        {
                            if (value == null || value == string.Empty)
                            {
                                trayType.Length = null;
                            }
                            else
                            {
                                trayType.Length = int.Parse(value);
                            }
                        }
                        else if (cell.CellReference == "E" + rowNumber)
                        {
                            trayType.Weight = double.Parse(value);
                        }
                        else if (cell.CellReference == "F" + rowNumber)
                        {
                            SupportServiceModel supportType = await GetSupportTypeAsync(value);

                            if (supportType == null)
                            {
                                throw new Exception($"Support type {value} not found.");
                            }

                            trayType.SupportId = supportType.Id;
                        }
                    }

                    trayTypes.Add(trayType);
                }

                foreach (var trayType in trayTypes)
                {
                    await CreateTrayTypeAsync(trayType);
                }
            }
        }
        private async Task<SupportServiceModel> GetSupportTypeAsync(string trayType)
        {
            SupportServiceModel? supportType = await _supportService.GetSupportAsync(trayType);

            if (supportType == null)
            {
                return null;
            }

            return supportType;
        }
    }
}
