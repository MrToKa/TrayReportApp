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

        public CableService(ITrayReportAppDbRepository repo)
        {
            _repo = repo;
        }

        public async Task<CableServiceModel> CreateCableAsync(CableServiceModel cable)
        {
            bool cableExists = await _repo.All<Cable>().AnyAsync(c => c.Tag == cable.Tag && c.CableType.Id == cable.Type);

            if (cableExists)
            {
                throw new Exception("Cable already exists");
            }

            Cable newCable = new Cable
            {
                Tag = cable.Tag,
                FromLocation = cable.FromLocation,
                ToLocation = cable.ToLocation,
            };

            if (cable.Type.HasValue)
            {
                newCable.Type = cable.Type.Value;
            }

            await _repo.AddAsync(newCable);
            await _repo.SaveChangesAsync();
            await RoutingMapping(cable, newCable);
            await _repo.SaveChangesAsync();

            return new CableServiceModel
            {
                Id = newCable.Id,
                Tag = newCable.Tag,
                Type = newCable.Type,
                FromLocation = newCable.FromLocation,
                ToLocation = newCable.ToLocation,
                Trays = cable.Trays
            };
        }

        private async Task RoutingMapping(CableServiceModel cable, Cable newCable)
        {
            if (!string.IsNullOrEmpty(cable.Trays))
            {
                var trayNames = cable.Trays
                    .Split('/')
                    .Select(name => name.Trim())
                    .ToList();

                var trays = await _repo.All<Tray>()
                    .Where(t => trayNames.Contains(t.Name))
                    .ToListAsync();

                var foundTrayNames = trays.Select(t => t.Name).ToList();
                var missingTrayNames = trayNames.Except(foundTrayNames).ToList();

                if (missingTrayNames.Any())
                {
                    throw new Exception($"The following trays were not found: {string.Join(", ", missingTrayNames)}");
                }


                foreach (var tray in trays)
                {
                    var trayCableMapping = new TrayCable
                    {
                        TrayId = tray.Id,
                        CableId = newCable.Id
                    };
                    await _repo.AddAsync(trayCableMapping);
                }
            }
        }

        public async Task DeleteCableAsync(int id)
        {
            var cable = await _repo.All<Cable>().FirstOrDefaultAsync(c => c.Id == id);
            if (cable == null)
            {
                throw new Exception("Cable not found");
            }

            await _repo.DeleteAsync<Cable>(cable.Id);
            await _repo.SaveChangesAsync();
        }

        public async Task<CableServiceModel> GetCableAsync(int id)
        {
            var cable = await _repo.All<Cable>()
                .Include(c => c.Routing)
                .Include(c => c.CableType)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (cable == null)
            {
                throw new Exception("Cable not found");
            }

            var trayNames = cable.Routing.Select(t => t.Name).ToList();

            var cableServiceModel = new CableServiceModel
            {
                Id = cable.Id,
                Tag = cable.Tag,
                Type = cable.Type,
                FromLocation = cable.FromLocation,
                ToLocation = cable.ToLocation,
                Trays = string.Join("/", trayNames)
            };

            return cableServiceModel;
        }


        public async Task<List<CableServiceModel>> GetCablesAsync()
        {
            var cables = await _repo.All<Cable>()
                .Include(c => c.Routing)
                .Include(c => c.CableType)
                .ToListAsync();

            return cables.Select(c => new CableServiceModel
            {
                Id = c.Id,
                Tag = c.Tag,
                Type = c.Type,
                FromLocation = c.FromLocation,
                ToLocation = c.ToLocation,
                Trays = string.Join("/", c.Routing.Select(t => t.Name))
            }).ToList();
        }

        public async Task UpdateCableAsync(CableServiceModel cable)
        {
            var cableToUpdate = await _repo.All<Cable>().FirstOrDefaultAsync(c => c.Id == cable.Id);

            if (cableToUpdate == null)
            {
                throw new Exception("Cable not found");
            }

            cableToUpdate.Tag = cable.Tag;
            cableToUpdate.FromLocation = cable.FromLocation;
            cableToUpdate.ToLocation = cable.ToLocation;

            if (cable.Type.HasValue)
            {
                cableToUpdate.Type = cable.Type.Value;
            }

            await RoutingMapping(cable, cableToUpdate);
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
                            cable.Type = int.Parse(value);
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
                            cable.Trays = value;
                        }
                    }

                    cables.Add(cable);
                }

                foreach (var cable in cables)
                {
                    await CreateCableAsync(cable);
                }
            }
        }
    }
}
