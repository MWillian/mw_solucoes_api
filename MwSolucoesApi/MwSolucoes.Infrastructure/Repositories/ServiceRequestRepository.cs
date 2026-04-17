using Microsoft.EntityFrameworkCore;
using MwSolucoes.Domain.Entities;
using MwSolucoes.Domain.Repositories;
using MwSolucoes.Domain.Repositories.Filters;
using MwSolucoes.Infrastructure.Data;

namespace MwSolucoes.Infrastructure.Repositories
{
    public class ServiceRequestRepository : IServiceRequestRepository
    {
        private const string ProtocolUniqueIndexName = "IX_ServiceRequests_Protocol";

        private readonly AppDbContext _context;
        public ServiceRequestRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task Add(ServiceRequest serviceRequest)
        {
            await _context.ServiceRequests.AddAsync(serviceRequest);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> TryAdd(ServiceRequest serviceRequest)
        {
            await _context.ServiceRequests.AddAsync(serviceRequest);

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException ex) when (IsProtocolUniqueConstraintViolation(ex))
            {
                _context.Entry(serviceRequest).State = EntityState.Detached;
                return false;
            }
        }

        public async Task<ServiceRequest?> GetById(Guid id) =>
            await _context.ServiceRequests.FirstOrDefaultAsync(m => m.Id.Equals(id));

        public async Task<ServiceRequest?> GetByProtocol(string protocol) =>
            await _context.ServiceRequests.FirstOrDefaultAsync(m => m.Protocol.Equals(protocol));

        public async Task<PagedResult<ServiceRequest>> GetAll(ServiceRequestFilters filters, Guid? userId)
        {
            var query = _context.ServiceRequests
                .AsNoTracking()
                .AsQueryable();

            if (userId.HasValue)
            {
                query = query.Where(sr => sr.UserId == userId.Value);
            }

            if (filters.Status.HasValue)
            {
                query = query.Where(sr => sr.Status == filters.Status.Value);
            }

            if (filters.CreatedAt.HasValue)
            {
                var createdDate = filters.CreatedAt.Value.Date;
                query = query.Where(sr => sr.CreatedAt.Date == createdDate);
            }

            if (!string.IsNullOrWhiteSpace(filters.Protocol))
            {
                var normalizedProtocol = filters.Protocol.Trim();
                query = query.Where(sr => EF.Functions.ILike(sr.Protocol, $"%{normalizedProtocol}%"));
            }

            if (filters.EquipmentType.HasValue)
            {
                query = query.Where(sr => sr.EquipmentType == filters.EquipmentType.Value);
            }

            if (filters.LaborCost.HasValue)
            {
                query = query.Where(sr => sr.LaborCost == filters.LaborCost.Value);
            }

            if (filters.PartsCost.HasValue)
            {
                query = query.Where(sr => sr.PartsCost == filters.PartsCost.Value);
            }

            var totalCount = await query.CountAsync();
            var sortBy = filters.SortBy?.Trim().ToLower() ?? "createdat";
            var sortDirection = filters.SortDirection?.Trim().ToLower() ?? "desc";

            query = (sortBy, sortDirection) switch
            {
                ("status", "desc") => query.OrderByDescending(sr => sr.Status),
                ("status", _) => query.OrderBy(sr => sr.Status),
                ("protocol", "desc") => query.OrderByDescending(sr => sr.Protocol),
                ("protocol", _) => query.OrderBy(sr => sr.Protocol),
                ("equipmenttype", "desc") => query.OrderByDescending(sr => sr.EquipmentType),
                ("equipmenttype", _) => query.OrderBy(sr => sr.EquipmentType),
                ("laborcost", "desc") => query.OrderByDescending(sr => sr.LaborCost),
                ("laborcost", _) => query.OrderBy(sr => sr.LaborCost),
                ("partscost", "desc") => query.OrderByDescending(sr => sr.PartsCost),
                ("partscost", _) => query.OrderBy(sr => sr.PartsCost),
                ("createdat", "asc") => query.OrderBy(sr => sr.CreatedAt),
                (_, "asc") => query.OrderBy(sr => sr.CreatedAt),
                _ => query.OrderByDescending(sr => sr.CreatedAt)
            };

            var page = filters.Page <= 0 ? 1 : filters.Page;
            var pageSize = filters.PageSize <= 0 ? 20 : Math.Min(filters.PageSize, 100);

            var result = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<ServiceRequest>(result, totalCount, page, pageSize);
        }

        private static bool IsProtocolUniqueConstraintViolation(DbUpdateException exception)
        {
            var message = $"{exception.Message} {exception.InnerException?.Message}";
            return message.Contains(ProtocolUniqueIndexName, StringComparison.OrdinalIgnoreCase);
        }

        public async Task DeleteById(Guid id)
        {
            await _context.ServiceRequests.Where(sr => sr.Id == id).ExecuteDeleteAsync();
        }

        public async Task Update(ServiceRequest serviceRequest)
        {
            _context.ServiceRequests.Update(serviceRequest);
            await _context.SaveChangesAsync();
        }
    }
}
