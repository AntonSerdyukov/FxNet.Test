using FxNet.Test.DTO;
using FxNet.Test.Exceptions;
using FxNet.Test.Interfaces;
using FxNet.Test.Models;
using Microsoft.AspNetCore.Mvc;

namespace FxNet.Test.Controllers
{
    [ApiController]
    [Route("api.user.partner")]
    public class PartnerController(IUserRepository _repository, IJwtTokenService _jwt) : ControllerBase
    {
        [HttpPost("rememberMe")]
        public async Task<ActionResult<TokenInfo>> RememberMeAsync([FromQuery] string code)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                throw new SecureException("Code must be provided");
            }

            var user = await _repository.GetByCodeAsync(code);

            if (user == null)
            {
                user = new User { Code = code };
                await _repository.CreateAsync(user);
            }

            return new TokenInfo
            {
                Token = _jwt.CreateToken(user)
            };
        }
    }
}
