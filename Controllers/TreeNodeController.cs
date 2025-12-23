using FxNet.Test.Exceptions;
using FxNet.Test.Interfaces;
using FxNet.Test.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FxNet.Test.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api.user.tree.node")]
    public class TreeNodeController(ITreeRepository _treeRepository, ITreeNodeRepository _treeNodeRepository) : ControllerBase
    {
        [HttpPost("create")]
        public async Task<IActionResult> CreateAsync(
            [FromQuery] string treeName,
            [FromQuery] long? parentNodeId,
            [FromQuery] string nodeName)
        {
            if (string.IsNullOrWhiteSpace(treeName))
            {
                throw new SecureException("Tree name must be specified");
            }

            if (string.IsNullOrWhiteSpace(nodeName))
            {
                throw new SecureException("Node name must be specified");
            }

            var tree = await _treeRepository.GetTreeByNameAsync(treeName);
            if (tree == null)
            {
                throw new SecureException("Tree not found");
            }

            if (parentNodeId.HasValue)
            {
                var parentNode = await _treeNodeRepository.GetNodeAndTreeByIdAsync(parentNodeId.Value);
                if (parentNode == null)
                {
                    throw new SecureException("Parent node does not exist");
                }
                else if (parentNode.Tree.Name != treeName)
                {
                    throw new SecureException("Child node doesn't belong to the same tree as its parent");
                }
            }

            var exists = await _treeNodeRepository.ExistsSiblingWithSameNameAsync(parentNodeId, nodeName);
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
            await _treeNodeRepository.CreateAsync(node);

            return Ok();
        }

        [HttpPost("delete")]
        public async Task<IActionResult> DeleteAsync([FromQuery] long nodeId)
        {
            var node = await _treeNodeRepository.GetNodeByIdAsync(nodeId);
            if (node == null)
            {
                throw new SecureException("Node not found");
            }

            await _treeNodeRepository.DeleteAsync(node);

            return Ok();
        }

        [HttpPost("rename")]
        public async Task<IActionResult> RenameAsync(
            [FromQuery] long nodeId,
            [FromQuery] string newNodeName)
        {
            var node = await _treeNodeRepository.GetNodeByIdAsync(nodeId);
            if (node == null)
            {
                throw new SecureException("Node not found");
            }

            if (string.IsNullOrWhiteSpace(newNodeName))
            {
                throw new SecureException("New node name must be specified");
            }

            var exists = await _treeNodeRepository.ExistsSiblingWithSameNameAsync(node.ParentNodeId, newNodeName, nodeId);
            if (exists)
            {
                throw new SecureException("Node name must be unique among siblings");
            }

            await _treeNodeRepository.RenameAsync(node, newNodeName);

            return Ok();
        }
    }
}
