using Microsoft.EntityFrameworkCore;
using MwSolucoes.Domain.Entities;
using MwSolucoes.Domain.Enums;
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
            await _context.ServiceRequests
                .Include(serviceRequest => serviceRequest.Items)
                .FirstOrDefaultAsync(m => m.Id.Equals(id));

        public async Task<ServiceRequest?> GetByProtocol(string protocol) =>
            await _context.ServiceRequests
                .Include(serviceRequest => serviceRequest.Items)
                .FirstOrDefaultAsync(m => m.Protocol.Equals(protocol));

        public async Task<PagedResult<ServiceRequest>> GetAll(ServiceRequestFilters filters, Guid currentUserId, bool isQueue)
        {
            var query = _context.ServiceRequests
                .AsNoTracking()
                .Include(serviceRequest => serviceRequest.Items)
                .AsQueryable();

            if (isQueue)
            {
                query = query.Where(sr => sr.Status == ServiceRequestStatus.Created && sr.TechnicianId == null);
            }
            else
            {
                query = query.Where(sr => sr.UserId == currentUserId || sr.TechnicianId == currentUserId);
                if (filters.Status.HasValue)
                {
                    query = query.Where(sr => sr.Status == filters.Status.Value);
                }
            }


            if (filters.CreatedAt.HasValue)
            {
                var startDate = filters.CreatedAt.Value.Date;
                var endDate = startDate.AddDays(1);
                query = query.Where(sr => sr.CreatedAt.Date >= startDate && sr.CreatedAt < endDate);
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

            var result = await query
                .Skip((filters.Page - 1) * filters.PageSize)
                .Take(filters.PageSize)
                .ToListAsync();

            return new PagedResult<ServiceRequest>(result, totalCount, filters.Page, filters.PageSize);
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
