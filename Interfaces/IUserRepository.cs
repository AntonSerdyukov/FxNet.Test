using FxNet.Test.Models;

namespace FxNet.Test.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByCodeAsync(string code);
        Task CreateAsync(User user);
    }
}
