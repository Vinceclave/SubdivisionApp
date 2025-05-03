using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Subdivision.Models
{
    public class Forum
    {
        [Key]
        public int ForumId { get; set; }
        
        [ForeignKey(nameof(Homeowner))]
        public int? HomeownerId { get; set; }
        
        [ForeignKey(nameof(Staff))]
        public int? StaffId { get; set; }
        
        [ForeignKey(nameof(Admin))]
        public int? AdminId { get; set; }
        
        [Required]
        [StringLength(2000, MinimumLength = 10, ErrorMessage = "Content must be between 10 and 2000 characters")]
        public string Content { get; set; } = string.Empty;
        
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime DateTimePosted { get; set; } = DateTime.UtcNow;
        
        [Required]
        [StringLength(200)]
        public string ForumTitle { get; set; } = string.Empty;

        public string? ImagePath { get; set; }

        public virtual ICollection<ForumReplies> ForumReplies { get; set; } = new List<ForumReplies>();
    }
}