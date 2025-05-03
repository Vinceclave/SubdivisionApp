using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Subdivision.Models
{
    public class EventCalendar
    {
        [Key]
        public int EventId { get; set; }

        [Required, StringLength(200)]
        [Display(Name = "Event Title")]
        public string EventTitle { get; set; } = string.Empty;

        [Required, StringLength(2000)]
        [Display(Name = "Description")]
        public string EventDesc { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime EventDateTime { get; set; }

        [Required, StringLength(20)]
        public string Visibility { get; set; } = string.Empty;

        [Required]
        public DateTime DateUploaded { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(OrganizedBy))]
        [Display(Name = "Organized By")]
        public int OrganizedById { get; set; }

        public virtual Admin OrganizedBy { get; set; } = null!;
        public virtual ICollection<EventVisibility> EventVisibilities { get; set; } = new List<EventVisibility>();
    }
}