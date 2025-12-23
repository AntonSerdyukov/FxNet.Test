using System.ComponentModel.DataAnnotations;

namespace FxNet.Test.Models
{
    public class Tree
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public string Name { get; set; } = null!;

        [Required]
        public long UserId { get; set; }
        public User User { get; set; } = null!;
        public ICollection<TreeNode> Nodes { get; set; } = new List<TreeNode>();
    }
}
