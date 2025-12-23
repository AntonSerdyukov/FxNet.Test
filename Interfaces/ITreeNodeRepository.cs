using FxNet.Test.Models;

namespace FxNet.Test.Interfaces
{
    public interface ITreeNodeRepository
    {
        Task<TreeNode?> GetNodeAndTreeByIdAsync(long id);
        Task<TreeNode?> GetNodeByIdAsync(long id);
        Task<bool> ExistsSiblingWithSameNameAsync(long? parentNodeId, string name);
        Task<bool> ExistsSiblingWithSameNameAsync(long? parentNodeId, string name, long nodeId);
        Task CreateAsync(TreeNode node);
        Task DeleteAsync(TreeNode node);
        Task RenameAsync(TreeNode node, string newName);
    }
}
