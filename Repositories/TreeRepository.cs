using FxNet.Test.Data;
using FxNet.Test.DTO;
using FxNet.Test.Interfaces;
using FxNet.Test.Models;
using Microsoft.EntityFrameworkCore;

namespace FxNet.Test.Repositories
{
    public class TreeRepository(AppDbContext _db) : ITreeRepository
    {
        public async Task<Tree?> GetTreeByNameAsync(string name)
        {
            return await _db.Trees.FirstOrDefaultAsync(x => x.Name == name);
        }

        public async Task<Tree?> GetTreeAndNodesByNameAsync(string name)
        {
           return await _db.Trees
                .Include(x => x.Nodes)
                    .ThenInclude(x => x.Children)
                .FirstOrDefaultAsync(x => x.Name == name);
        }

        public List<MNode> GetTreeRoots(Tree tree)
        {
            return tree.Nodes
                .Where(x => x.ParentNodeId == null)
                .Select(Mapping.Mapper.ToDto)
                .ToList();
        }

        public async Task CreateAsync(Tree tree)
        {
            await _db.Trees.AddAsync(tree);
            await _db.SaveChangesAsync();
        }
    }
}
