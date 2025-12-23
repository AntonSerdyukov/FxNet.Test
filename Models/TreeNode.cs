using System.ComponentModel.DataAnnotations;

namespace FxNet.Test.Models
{
    public class TreeNode
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public string Name { get; set; } = null!;

        [Required]
        public long TreeId { get; set; }

        public Tree Tree { get; set; } = null!;
        public long? ParentNodeId { get; set; }
        public TreeNode? ParentNode { get; set; }
        public ICollection<TreeNode> Children { get; set; } = new List<TreeNode>();
    }
}
