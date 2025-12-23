using FxNet.Test.Data;
using FxNet.Test.Interfaces;
using FxNet.Test.Models;
using Microsoft.EntityFrameworkCore;

namespace FxNet.Test.Repositories
{
    public class UserRepository(AppDbContext _db) : IUserRepository
    {
        public async Task<User?> GetByCodeAsync(string code)
        {
            return await _db.Users.FirstOrDefaultAsync(u => u.Code == code);
        }

        public async Task CreateAsync(User user)
        {
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
        }
    }
}
