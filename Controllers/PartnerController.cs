using FxNet.Test.Data;
using FxNet.Test.DTO;
using FxNet.Test.Exceptions;
using FxNet.Test.Interfaces;
using FxNet.Test.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FxNet.Test.Controllers
{
    [ApiController]
    [Route("api.user.partner")]
    public class PartnerController(AppDbContext _db, IJwtTokenService _jwt) : ControllerBase
    {
        [HttpPost("rememberMe")]
        public async Task<ActionResult<TokenInfo>> RememberMeAsync([FromQuery] string code)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                throw new SecureException("Code must be provided");
            }

            var user = await _db.Users.FirstOrDefaultAsync(x => x.Code == code);

            if (user == null)
            {
                user = new User { Code = code };
                _db.Users.Add(user);
                await _db.SaveChangesAsync();
            }

            return new TokenInfo
            {
                Token = _jwt.CreateToken(user)
            };
        }
    }
}
