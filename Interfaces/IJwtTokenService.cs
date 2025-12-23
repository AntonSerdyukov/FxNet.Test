using FxNet.Test.Models;

namespace FxNet.Test.Interfaces
{
    public interface IJwtTokenService
    {
        string CreateToken(User user);
    }
}
