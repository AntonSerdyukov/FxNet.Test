using FxNet.Test.DTO;
using FxNet.Test.Models;

namespace FxNet.Test.Interfaces
{
    public interface ITreeRepository
    {
        Task<Tree?> GetTreeByNameAsync(string name);
        Task<Tree?> GetTreeAndNodesByNameAsync(string name);
        List<MNode> GetTreeRoots(Tree tree);
        Task CreateAsync(Tree tree);
    }
}
