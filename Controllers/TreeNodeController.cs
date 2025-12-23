using FxNet.Test.Data;
using FxNet.Test.Exceptions;
using FxNet.Test.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FxNet.Test.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api.user.tree.node")]
    public class TreeNodeController(AppDbContext _db) : ControllerBase
    {
        [HttpPost("create")]
        public async Task<IActionResult> CreateAsync(
            [FromQuery] string treeName,
            [FromQuery] long? parentNodeId,
            [FromQuery] string nodeName)
        {

            if (string.IsNullOrWhiteSpace(nodeName))
            {
                throw new SecureException("Node name must be specified");
            }

            var tree = await _db.Trees.FirstOrDefaultAsync(x => x.Name == treeName);

            if (tree == null)
            {
                throw new SecureException("Tree not found");
            }

            var exists = await _db.TreeNodes.AnyAsync(x =>
                x.TreeId == tree.Id &&
                x.ParentNodeId == parentNodeId &&
                x.Name == nodeName);

            if (exists)
            {
                throw new SecureException("Node name must be unique among siblings");
            }

            var node = new TreeNode
            {
                TreeId = tree.Id,
                ParentNodeId = parentNodeId,
                Name = nodeName
            };

            _db.TreeNodes.Add(node);
            await _db.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("delete")]
        public async Task<IActionResult> DeleteAsync([FromQuery] long nodeId)
        {
            var node = await _db.TreeNodes.FindAsync(nodeId);

            if (node == null)
            {
                throw new SecureException("Node not found");
            }

            _db.TreeNodes.Remove(node);
            await _db.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("rename")]
        public async Task<IActionResult> RenameAsync(
            [FromQuery] long nodeId,
            [FromQuery] string newNodeName)
        {
            if (string.IsNullOrWhiteSpace(newNodeName))
            {
                throw new SecureException("New node name must be specified");
            }

            var node = await _db.TreeNodes.FindAsync(nodeId);

            if (node == null)
            {
                throw new SecureException("Node not found");
            }

            var exists = await _db.TreeNodes.AnyAsync(x =>
                x.TreeId == node.TreeId &&
                x.ParentNodeId == node.ParentNodeId &&
                x.Name == newNodeName &&
                x.Id != nodeId);

            if (exists)
            {
                throw new SecureException("Node name must be unique among siblings");
            }

            node.Name = newNodeName;
            await _db.SaveChangesAsync();

            return Ok();
        }
    }
}
