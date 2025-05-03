using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Subdivision.Models
{
    public class ForumReplies
    {
        [Key]
        public int ForumRepliesId { get; set; }
        
        [ForeignKey(nameof(Forum))]
        public int ForumId { get; set; }
        
        [ForeignKey(nameof(Homeowner))]
        public int? HomeownerId { get; set; }
        
        [ForeignKey(nameof(Staff))]
        public int? StaffId { get; set; }
        
        [ForeignKey(nameof(Admin))]
        public int? AdminId { get; set; }
        
        [Required, StringLength(2000)]
        public string RepliedContent { get; set; } = string.Empty;
        
        [Required]
        public DateTime DateTime { get; set; } = DateTime.UtcNow;
    }
}