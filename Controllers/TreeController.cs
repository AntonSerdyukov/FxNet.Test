using FxNet.Test.DTO;
using FxNet.Test.Exceptions;
using FxNet.Test.Interfaces;
using FxNet.Test.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FxNet.Test.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api.user.tree")]
    public class TreeController(ITreeRepository _repository) : ControllerBase
    {
        private long UserId => long.Parse(User.FindFirst("uid")!.Value);

        [HttpPost("get")]
        public async Task<ActionResult<MNode>> GetAsync([FromQuery] string treeName)
        {
            if (string.IsNullOrWhiteSpace(treeName))
            {
                throw new SecureException("Tree name must be specified");
            }

            var tree = await _repository.GetTreeAndNodesByNameAsync(treeName);
            if (tree == null)
            {
                tree = new Tree
                {
                    Name = treeName,
                    UserId = UserId
                };

                await _repository.CreateAsync(tree);

                return Ok(new MNode
                {
                    Id = tree.Id,
                    Name = tree.Name,
                    Children = new()
                });
            }

            var rootNodes = _repository.GetTreeRoots(tree);

            return Ok(new MNode
            {
                Id = tree.Id,
                Name = tree.Name,
                Children = rootNodes
            });
        }
    }
}
