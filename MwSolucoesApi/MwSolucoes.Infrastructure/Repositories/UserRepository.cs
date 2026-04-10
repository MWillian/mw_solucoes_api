using Microsoft.EntityFrameworkCore;
using MwSolucoes.Domain.Entities;
using MwSolucoes.Domain.Repositories;
using MwSolucoes.Domain.Repositories.Filters;
using MwSolucoes.Domain.ValueObjects;
using MwSolucoes.Infrastructure.Data;

namespace MwSolucoes.Infrastructure.Repositories
{
    internal class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;
        public UserRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task Add(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }
        public async Task<User?> GetByEmail(string email) =>
            await _context.Users.FirstOrDefaultAsync(u => u.Email.Equals(email));
        public async Task<User?> GetById(Guid id) =>
            await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

        public async Task<PagedResult<User>> GetUsers(UserFilters filters)
        {
            var query = _context.Users.AsNoTracking().AsQueryable();


            if (!string.IsNullOrWhiteSpace(filters.Name))
            {
                var normalizedName = filters.Name.Trim();
                query = query.Where(u => EF.Functions.ILike(u.Name, $"%{normalizedName}%"));
            }

            if (!string.IsNullOrWhiteSpace(filters.Email))
            {
                var normalizedEmail = filters.Email.Trim();
                query = query.Where(u => EF.Functions.ILike(u.Email, $"%{normalizedEmail}%"));
            }

            if (filters.IsActive.HasValue)
            {
                query = query.Where(u => u.IsActive == filters.IsActive.Value);
            }

            var totalCount = await query.CountAsync();
            var sortBy = filters.SortBy?.Trim().ToLower() ?? "name";
            var sortDirection = filters.SortDirection?.Trim().ToLower() ?? "asc";

            query = (sortBy, sortDirection) switch
            {
                ("email", "desc") => query.OrderByDescending(u => u.Email),
                ("email", _) => query.OrderBy(u => u.Email),
                ("isactive", "desc") => query.OrderByDescending(u => u.IsActive),
                ("isactive", _) => query.OrderBy(u => u.IsActive),
                (_, "desc") => query.OrderByDescending(u => u.Name),
                _ => query.OrderBy(u => u.Name)
            };

            var page = filters.Page <= 0 ? 1 : filters.Page;
            var pageSize = filters.PageSize <= 0 ? 20 : Math.Min(filters.PageSize, 100);

            var result = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            PagedResult<User> pagedResult = new(result, totalCount, page, pageSize);
            return pagedResult;
        }
        public async Task Update(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
        public async Task<bool> ExistUserWithEmail(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email.Equals(email));
        }
        public async Task<bool> ExistUserWithPhoneNumber(string phoneNumber)
        {
            return await _context.Users.AnyAsync(u => u.PhoneNumber == phoneNumber);
        }
        public async Task<bool> ExistUserByCpf(string cpf)
        {
            return await _context.Users.AnyAsync(u => u.CPF == cpf);
        }

        public async Task<bool> IsActive(Guid id)
        {
            var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
            return user != null && user.IsActive;
        }
    }
}
