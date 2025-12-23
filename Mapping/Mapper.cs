using FxNet.Test.DTO;
using FxNet.Test.Models;

namespace FxNet.Test.Mapping
{
    public static class Mapper
    {
        public static MNode ToDto(this TreeNode node)
        {
            return new MNode
            {
                Id = node.Id,
                Name = node.Name,
                Children = node.Children.Select(ToDto).ToList()
            };
        }
    }
}
