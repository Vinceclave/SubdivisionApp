using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Subdivision.Models
{
    public class Announcement
    {
        [Key]
        public int AnnouncementId { get; set; }

        [ForeignKey(nameof(Admin))]
        public int AdminId { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        public DateTime DateTimePosted { get; set; } = DateTime.UtcNow;
    }
}
