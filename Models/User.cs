using System.ComponentModel.DataAnnotations;

namespace FxNet.Test.Models
{
    public class User
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public string Code { get; set; } = null!;
        public ICollection<Tree> Trees { get; set; } = new List<Tree>();
    }
}
