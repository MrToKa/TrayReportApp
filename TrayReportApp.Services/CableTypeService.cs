using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
using TrayReportApp.Data.Repositories;
using TrayReportApp.Models;
using TrayReportApp.Models.Enum;
using TrayReportApp.Services.Contracts;
using TrayReportApp.Services.Models;

namespace TrayReportApp.Services
{
    public class CableTypeService : ICableTypeService
    {
        private readonly ITrayReportAppDbRepository _repo;

        public CableTypeService(ITrayReportAppDbRepository repo)
        {
            _repo = repo;
        }

        public async Task<CableTypeServiceModel> CreateCableTypeAsync(CableTypeServiceModel cableType)
        {
            bool cableTypeExists = await _repo.All<CableType>().AnyAsync(c => c.Type == cableType.Type);

            if (cableTypeExists)
            {
                return null;
                //throw new Exception("Cable type already exists");
            }

            if (!Enum.TryParse(cableType.Purpose, out Purpose parsedPurpose))
            {
                throw new Exception("Invalid purpose value");
            }

            var newCableType = new CableType
            {
                Purpose = parsedPurpose,
                Type = cableType.Type,
                Diameter = cableType.Diameter,
                Weight = cableType.Weight
            };

            await _repo.AddAsync(newCableType);
            await _repo.SaveChangesAsync();

            return cableType;
        }

        public async Task DeleteCableTypeAsync(int id)
        {
            var cableType = await _repo.All<CableType>().FirstOrDefaultAsync(c => c.Id == id);

            if (cableType == null)
            {
                throw new Exception("Cable type not found");
            }

            await _repo.DeleteAsync<CableType>(cableType.Id);
            await _repo.SaveChangesAsync();
        }

        public async Task<CableTypeServiceModel> GetCableTypeAsync(int id)
        {
            var cableType = await _repo.All<CableType>().FirstOrDefaultAsync(c => c.Id == id);

            if (cableType == null)
            {
                throw new Exception("Cable type not found");
            }

            return new CableTypeServiceModel
            {
                Id = cableType.Id,
                Purpose = cableType.Purpose.ToString(),
                Type = cableType.Type,
                Diameter = cableType.Diameter,
                Weight = cableType.Weight
            };
        }

        public async Task<List<CableTypeServiceModel>> GetCableTypesAsync()
        {
            return await _repo.All<CableType>()
                .Select(c => new CableTypeServiceModel
                {
                    Id = c.Id,
                    Purpose = c.Purpose.ToString(),
                    Type = c.Type,
                    Diameter = c.Diameter,
                    Weight = c.Weight
                })
                .ToListAsync();
        }

        public async Task<CableTypeServiceModel> UpdateCableTypeAsync(CableTypeServiceModel cableType)
        {
            var existingCableType = await _repo.All<CableType>().FirstOrDefaultAsync(c => c.Id == cableType.Id);

            if (existingCableType == null)
            {
                throw new Exception("Cable type not found");
            }

            existingCableType.Purpose = Enum.Parse<Purpose>(cableType.Purpose);
            existingCableType.Type = cableType.Type;
            existingCableType.Diameter = cableType.Diameter;
            existingCableType.Weight = cableType.Weight;
            existingCableType.Type = cableType.Type;
            existingCableType.Diameter = cableType.Diameter;
            existingCableType.Weight = cableType.Weight;

            await _repo.SaveChangesAsync();

            return cableType;
        }

        public async Task UploadFromFileAsync(IBrowserFile file)
        {
            List<CableTypeServiceModel> cableTypes = new List<CableTypeServiceModel>();
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
                    CableTypeServiceModel cableType = new CableTypeServiceModel();
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

                        if(cell.CellReference == "A" + rowNumber)
                        {
                            if (Enum.TryParse(value, true, out Purpose parsedPurpose))
                            {
                                cableType.Purpose = parsedPurpose.ToString();
                            }
                        }
                        else if (cell.CellReference == "B" + rowNumber)
                        {
                            cableType.Type = value;
                        }
                        else if (cell.CellReference == "C" + rowNumber)
                        {
                            cableType.Diameter = double.Parse(value);
                        }
                        else if (cell.CellReference == "D" + rowNumber)
                        {
                            cableType.Weight = double.Parse(value);
                        }
                    }

                    cableTypes.Add(cableType);
                }

                foreach (var cableType in cableTypes)
                {
                    await CreateCableTypeAsync(cableType);
                }
            }
        }

        public async Task<List<string>> GetCablesPurposes()
        {
            return Enum.GetNames(typeof(Purpose)).ToList();
        }
    }
}
