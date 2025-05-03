using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Subdivision.Models
{
    public class Feedback
    {
        [Key]
        public int FeedbackId { get; set; }
        
        [ForeignKey(nameof(Homeowner))]
        public int HomeownerId { get; set; }
        
        [Required, StringLength(1000)]
        public string FeedbackContent { get; set; } = string.Empty;
        
        public DateTime FeedbackDate { get; set; } = DateTime.UtcNow;
        
        [Range(1, 5)]
        public int Rating { get; set; }

        public virtual Homeowner Homeowner { get; set; } = null!;
    }
}