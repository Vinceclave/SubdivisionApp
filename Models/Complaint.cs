using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Subdivision.Models
{
    public class Complaint
    {
        [Key]
        public int ComplaintId { get; set; }

        [ForeignKey(nameof(Homeowner))]
        public int HomeownerId { get; set; }

        [Required, StringLength(2000)]
        [Display(Name = "Complaint")]
        public string ComplaintContent { get; set; } = string.Empty;

        [Required]
        public DateTime ComplaintDate { get; set; } = DateTime.UtcNow;

        [Required, StringLength(20)]
        public string Status { get; set; } = string.Empty;

        public virtual Homeowner Homeowner { get; set; } = null!;
    }
}