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
            bool supportExists = await _repo.All<Support>().AnyAsync(s => s.Name == support.Name);

            if (supportExists)
            {
                return null;
            }

            var newSupport = new Support
            {
                Name = support.Name,
                Weight = support.Weight,
                Distance = support.Distance
            };

            await this._repo.AddAsync(newSupport);
            await this._repo.SaveChangesAsync();

            return new SupportServiceModel
            {
                Name = newSupport.Name,
                Weight = newSupport.Weight,
                Distance = newSupport.Distance
            };
        }

        public async Task DeleteSupportAsync(int id)
        {
            var support = await _repo.All<Support>().FirstOrDefaultAsync(s => s.Id == id);
            if (support == null)
            {
                return;
            }

            await _repo.DeleteAsync<Support>(support.Id);
            await _repo.SaveChangesAsync();
        }

        public async Task<SupportServiceModel> GetSupportAsync(string name)
        {
            var support = await _repo.All<Support>()
                .FirstOrDefaultAsync(s => s.Name.Contains(name));
            if (support == null)
            {
                return null;
            }

            return new SupportServiceModel
            {
                Id = support.Id,
                Name = support.Name,
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
                Weight = s.Weight,
                Distance = s.Distance
            }).ToList();
        }

        public async Task<SupportServiceModel> UpdateSupportAsync(SupportServiceModel support)
        {
            var existingSupport = await this._repo.GetByIdAsync<Support>(support.Id);
            if (existingSupport == null)
            {
                return null;
            }

            existingSupport.Name = support.Name;
            existingSupport.Weight = support.Weight;
            existingSupport.Distance = support.Distance;

            _repo.Update(existingSupport);
            await _repo.SaveChangesAsync();

            return new SupportServiceModel
            {
                Id = existingSupport.Id,
                Name = existingSupport.Name,
                Weight = existingSupport.Weight,
                Distance = existingSupport.Distance
            };
        }
    }
}
