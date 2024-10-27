using Microsoft.EntityFrameworkCore;
using TrayReportApp.Data.Repositories;
using TrayReportApp.Models;
using TrayReportApp.Services.Contracts;
using TrayReportApp.Services.Models;

namespace TrayReportApp.Services
{
    public class SupportService : ISupportService
    {
        private readonly ITrayReportAppDbRepository _repo;

        public SupportService(ITrayReportAppDbRepository repo)
        {
            _repo = repo;
        }

        public async Task<SupportServiceModel> CreateSupportAsync(SupportServiceModel support)
        {
            bool supportExists = await _repo.All<Support>().AnyAsync(s => s.Name == support.Name && s.Type == support.Type);

            if (supportExists)
            {
                throw new Exception("Support already exists");
            }

            var newSupport = new Support
            {
                Name = support.Name,
                Type = support.Type,
                Weight = support.Weight,
                Distance = support.Distance
            };

            await this._repo.AddAsync(newSupport);
            await this._repo.SaveChangesAsync();

            return new SupportServiceModel
            {
                Name = newSupport.Name,
                Type = newSupport.Type,
                Weight = newSupport.Weight,
                Distance = newSupport.Distance
            };
        }

        public async Task DeleteSupportAsync(int id)
        {
            var support = this._repo.All<Support>().FirstOrDefault(s => s.Id == id);
            if (support == null)
            {
                throw new Exception("Support not found");
            }

            await _repo.DeleteAsync<Support>(support.Id);
            await _repo.SaveChangesAsync();
        }

        public async Task<SupportServiceModel> GetSupportAsync(int id)
        {
            var support = await _repo.GetByIdAsync<Support>(id);
            if (support == null)
            {
                throw new Exception("Support not found");
            }

            return new SupportServiceModel
            {
                Id = support.Id,
                Name = support.Name,
                Type = support.Type,
                Weight = support.Weight,
                Distance = support.Distance
            };

        }

        public async Task<List<SupportServiceModel>> GetSupportsAsync()
        {
            var supports = await this._repo.All<Support>()
                .ToListAsync();
            return supports.Select(s => new SupportServiceModel
            {
                Id = s.Id,
                Name = s.Name,
                Type = s.Type,
                Weight = s.Weight,
                Distance = s.Distance
            }).ToList();
        }

        public async Task<SupportServiceModel> UpdateSupportAsync(SupportServiceModel support)
        {
            var existingSupport = await this._repo.GetByIdAsync<Support>(support.Id);
            if (existingSupport == null)
            {
                throw new Exception("Support not found");
            }

            existingSupport.Name = support.Name;
            existingSupport.Type = support.Type;
            existingSupport.Weight = support.Weight;
            existingSupport.Distance = support.Distance;

            _repo.Update(existingSupport);
            await _repo.SaveChangesAsync();

            return new SupportServiceModel
            {
                Id = existingSupport.Id,
                Name = existingSupport.Name,
                Type = existingSupport.Type,
                Weight = existingSupport.Weight,
                Distance = existingSupport.Distance
            };
        }
    }
}
