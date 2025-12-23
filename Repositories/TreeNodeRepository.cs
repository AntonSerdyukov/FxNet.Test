using FxNet.Test.Data;
using FxNet.Test.Interfaces;
using FxNet.Test.Models;
using Microsoft.EntityFrameworkCore;

namespace FxNet.Test.Repositories
{
    public class TreeNodeRepository(AppDbContext _db) : ITreeNodeRepository
    {
        public async Task<TreeNode?> GetNodeAndTreeByIdAsync(long id)
        {
            return await _db.TreeNodes
                .Include(x => x.Tree)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<TreeNode?> GetNodeByIdAsync(long id)
        {
            return await _db.TreeNodes.FindAsync(id);
        }

        public async Task<bool> ExistsSiblingWithSameNameAsync(long? parentNodeId, string nodeName)
        {
            return await _db.TreeNodes.AnyAsync(x =>
                x.ParentNodeId == parentNodeId &&
                x.Name == nodeName);
        }

        public async Task<bool> ExistsSiblingWithSameNameAsync(long? parentNodeId, string newNodeName, long nodeId)
        {
            return await _db.TreeNodes.AnyAsync(x =>
                x.ParentNodeId == parentNodeId &&
                x.Name == newNodeName &&
                x.Id != nodeId);
        }

        public async Task CreateAsync(TreeNode node)
        {
            await _db.TreeNodes.AddAsync(node);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(TreeNode node)
        {
            _db.TreeNodes.Remove(node);
            await _db.SaveChangesAsync();
        }

        public async Task RenameAsync(TreeNode node, string newName)
        {
            node.Name = newName;
            await _db.SaveChangesAsync();
        }
    }
}
