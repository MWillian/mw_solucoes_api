using Microsoft.EntityFrameworkCore;
using MwSolucoes.Domain.Entities;
using MwSolucoes.Domain.Repositories;
using MwSolucoes.Infrastructure.Data;

namespace MwSolucoes.Infrastructure.Repositories
{
    public class MaintenanceServiceRepository : IMaintenanceServiceRepository
    {
        private readonly AppDbContext _context;
        public MaintenanceServiceRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task Add(MaintenanceService service)
        {
            await _context.MaintenanceServices.AddAsync(service);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(MaintenanceService service)
        {
            _context.MaintenanceServices.Remove(service);
            await _context.SaveChangesAsync();
        }

        public Task<PagedResult<MaintenanceService>> GetAll(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<MaintenanceService?> GetById(int id)
        {
            return await _context.MaintenanceServices.AsNoTracking().FirstOrDefaultAsync(s => s.Id.Equals(id));
        }

        public async Task Update(MaintenanceService service)
        {
            _context.MaintenanceServices.Update(service);
            await _context.SaveChangesAsync();
        }
        public async Task<MaintenanceService?> GetByName(string name)
        {
            return await _context.MaintenanceServices
                .AsNoTracking()
                .FirstOrDefaultAsync(s => EF.Functions.ILike(s.Name, name));
        }
    }
}
