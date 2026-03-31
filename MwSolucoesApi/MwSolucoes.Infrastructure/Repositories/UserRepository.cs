using Microsoft.EntityFrameworkCore;
using MwSolucoes.Domain.Entities;
using MwSolucoes.Domain.Repositories;
using MwSolucoes.Infrastructure.Data;
using System.ComponentModel;

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
        public async Task Update(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
        public async Task<bool> ExistUserWithEmail(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email.Equals(email));
        }
        public async Task<bool> ExistUserWithPhoneNumber(string phoneNumber) =>
            await _context.Users.AnyAsync(u => u.PhoneNumber.Equals(phoneNumber));
    }
}
