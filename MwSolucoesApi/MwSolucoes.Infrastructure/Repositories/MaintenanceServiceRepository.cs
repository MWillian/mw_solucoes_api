using Microsoft.EntityFrameworkCore;
using MwSolucoes.Domain.Entities;
using MwSolucoes.Domain.Repositories;
using MwSolucoes.Domain.Repositories.Filters;
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

        public async Task<PagedResult<MaintenanceService>> GetAll(MaintenanceServiceFilters filters)
        {
            var query = _context.MaintenanceServices.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(filters.Name))
            {
                var normalizedName = filters.Name.Trim();
                query = query.Where(s => EF.Functions.ILike(s.Name, $"%{normalizedName}%"));
            }

            if (filters.Price.HasValue)
            {
                query = query.Where(s => s.Price == filters.Price.Value);
            }

            if (filters.IsActive.HasValue)
            {
                query = query.Where(s => s.IsActive == filters.IsActive.Value);
            }

            if (filters.Category.HasValue)
            {
                query = query.Where(s => s.Category == filters.Category.Value);
            }

            var totalCount = await query.CountAsync();
            var sortBy = filters.SortBy?.Trim().ToLower() ?? "name";
            var sortDirection = filters.SortDirection?.Trim().ToLower() ?? "asc";

            query = (sortBy, sortDirection) switch
            {
                ("price", "desc") => query.OrderByDescending(s => s.Price),
                ("price", _) => query.OrderBy(s => s.Price),
                ("isactive", "desc") => query.OrderByDescending(s => s.IsActive),
                ("isactive", _) => query.OrderBy(s => s.IsActive),
                ("category", "desc") => query.OrderByDescending(s => s.Category),
                ("category", _) => query.OrderBy(s => s.Category),
                (_, "desc") => query.OrderByDescending(s => s.Name),
                _ => query.OrderBy(s => s.Name)
            };

            var page = filters.Page <= 0 ? 1 : filters.Page;
            var pageSize = filters.PageSize <= 0 ? 20 : Math.Min(filters.PageSize, 100);

            var result = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<MaintenanceService>(result, totalCount, page, pageSize);
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
            var normalizedName = name.Trim();
            return await _context.MaintenanceServices
                .AsNoTracking()
                .FirstOrDefaultAsync(s => EF.Functions.ILike(s.Name, normalizedName));
        }
    }
}
