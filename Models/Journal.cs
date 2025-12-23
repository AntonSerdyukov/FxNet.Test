using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;

namespace FxNet.Test.Models
{
    public class Journal
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public string Text { get; set; } = null!;

        [Required]
        public long EventId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
