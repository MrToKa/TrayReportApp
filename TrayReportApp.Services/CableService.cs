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
    public class CableService : ICableService
    {
        private readonly ITrayReportAppDbRepository _repo;
        private readonly ICableTypeService _cableTypeService;

        public CableService(ITrayReportAppDbRepository repo, 
            ICableTypeService cableTypeService)
        {
            _repo = repo;
            _cableTypeService = cableTypeService;
        }

        public async Task CreateCableAsync(CableServiceModel cable)
        {
            bool cableExists = await _repo.All<Cable>().AnyAsync(c => c.Tag == cable.Tag && c.Type == cable.Type);

            if (cableExists)
            {
                return;
            }

            Cable newCable = new Cable
            {
                Tag = cable.Tag,
                FromLocation = cable.FromLocation,
                ToLocation = cable.ToLocation,
                Routing = cable.Routing,                
            };

            var cableType = await _cableTypeService.GetCableTypeAsync(cable.Type);

            if (cableType != null)
            {
                newCable.Type = cableType.Type;
                newCable.CableTypeId = cableType.Id;
            }
            else
            {
                newCable.Type = cable.Type;
                newCable.CableTypeId = null;
            }

            await _repo.AddAsync(newCable);
            await _repo.SaveChangesAsync();
        }

        public async Task DeleteCableAsync(int id)
        {
            var cable = await _repo.All<Cable>().FirstOrDefaultAsync(c => c.Id == id);
            if (cable == null)
            {
                return;
            }

            await _repo.DeleteAsync<Cable>(cable.Id);
            await _repo.SaveChangesAsync();
        }

        public async Task<CableServiceModel> GetCableAsync(int id)
        {
            var cable = await _repo.All<Cable>()
                .Include(c => c.CableType)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (cable == null)
            {
                return null;
            }

            var cableServiceModel = new CableServiceModel
            {
                Id = cable.Id,
                Tag = cable.Tag,
                Type = cable.Type,
                FromLocation = cable.FromLocation,
                ToLocation = cable.ToLocation,
                Routing = cable.Routing,
                CableTypeId = cable.CableTypeId
            };

            return cableServiceModel;
        }

        public async Task<List<CableServiceModel>> GetCablesAsync()
        {
            var cables = await _repo.All<Cable>()
                .Include(c => c.CableType)
                .ToListAsync();

            return cables.Select(c => new CableServiceModel
            {
                Id = c.Id,
                Tag = c.Tag,
                Type = c.Type,
                FromLocation = c.FromLocation,
                ToLocation = c.ToLocation,
                Routing = c.Routing,
                CableTypeId = c.CableTypeId
            }).ToList();
        }

        public async Task UpdateCableAsync(CableServiceModel cable)
        {
            var cableToUpdate = await _repo.All<Cable>().FirstOrDefaultAsync(c => c.Id == cable.Id);

            if (cableToUpdate == null)
            {
                return;
            }

            cableToUpdate.Tag = cable.Tag;
            cableToUpdate.FromLocation = cable.FromLocation;
            cableToUpdate.ToLocation = cable.ToLocation;
            cableToUpdate.Routing = cable.Routing;

            if (cable.Type != null && cableToUpdate.Type != cable.Type)
            {
                var cableType = await _cableTypeService.GetCableTypeAsync(cable.Type);
                cableToUpdate.Type = cableType.Type;
                cableToUpdate.CableTypeId = cableType.Id;
            }
            else
            {
                cableToUpdate.Type = cable.Type;
                cableToUpdate.CableTypeId = null;
            }

            await _repo.SaveChangesAsync();
        }

        public async Task UploadFromFileAsync(IBrowserFile file)
        {
            List<CableServiceModel> cables = new List<CableServiceModel>();
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
                    CableServiceModel cable = new CableServiceModel();
                    Row row = sheetData.Elements<Row>().ElementAt(i);
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

                        if(cell.CellReference == "A" + rowNumber)
                        {
                            cable.Tag = value;
                        }
                        else if (cell.CellReference == "B" + rowNumber)
                        {
                            if (value != null && value != string.Empty)
                            {
                                cable.Type = value;
                            }
                            else
                            {
                                cable.Type = null;
                            }
                        }
                        else if (cell.CellReference == "C" + rowNumber)
                        {
                            cable.FromLocation = value;
                        }
                        else if (cell.CellReference == "D" + rowNumber)
                        {
                            cable.ToLocation = value;
                        }
                        else if (cell.CellReference == "E" + rowNumber)
                        {
                            cable.Routing = value;
                        }
                    }

                    if (cable.Type != null)
                    {
                        var cableType = await _cableTypeService.GetCableTypeAsync(cable.Type);
                        cable.CableTypeId = cableType == null ? null : cableType.Id;
                    }

                    cables.Add(cable);
                }

                foreach (var cable in cables)
                {
                    await CreateCableAsync(cable);
                }
            }
        }

        public Task ExportTableEntriesAsync()
        {
            throw new NotImplementedException();
        }

        public Task ExportFilteredTableEntriesAsync(IEnumerable<CableServiceModel> cables)
        {
            throw new NotImplementedException();
        }
    }
}
