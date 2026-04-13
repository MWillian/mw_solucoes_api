using Microsoft.EntityFrameworkCore;
using MwSolucoes.Domain.Entities;
using MwSolucoes.Domain.Repositories;
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

        private static bool IsProtocolUniqueConstraintViolation(DbUpdateException exception)
        {
            var message = $"{exception.Message} {exception.InnerException?.Message}";
            return message.Contains(ProtocolUniqueIndexName, StringComparison.OrdinalIgnoreCase);
        }
    }
}
