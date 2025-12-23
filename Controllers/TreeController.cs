using FxNet.Test.Data;
using FxNet.Test.DTO;
using FxNet.Test.Exceptions;
using FxNet.Test.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FxNet.Test.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api.user.tree")]
    public class TreeController(AppDbContext _db) : ControllerBase
    {
        private long UserId => long.Parse(User.FindFirst("uid")!.Value);

        [HttpPost("get")]
        public async Task<ActionResult<MNode>> GetAsync([FromQuery] string treeName)
        {
            if (string.IsNullOrWhiteSpace(treeName))
            {
                throw new SecureException("Tree name must be specified");
            }

            var tree = await _db.Trees
                .Include(x => x.Nodes)
                    .ThenInclude(x => x.Children)
                .FirstOrDefaultAsync(x => x.Name == treeName);

            if (tree == null)
            {
                tree = new Tree
                {
                    Name = treeName,
                    UserId = UserId
                };

                _db.Trees.Add(tree);
                await _db.SaveChangesAsync();

                return Ok(new MNode
                {
                    Id = tree.Id,
                    Name = tree.Name,
                    Children = new()
                });
            }

            var rootNodes = tree.Nodes
                .Where(x => x.ParentNodeId == null)
                .Select(Mapping.Mapper.ToDto)
                .ToList();

            return Ok(new MNode
            {
                Id = tree.Id,
                Name = tree.Name,
                Children = rootNodes
            });
        }
    }
}
