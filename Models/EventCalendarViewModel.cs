using System.ComponentModel.DataAnnotations;

namespace Subdivision.Models
{
    public class EventCalendarViewModel
    {
        [Required]
        public string EventTitle { get; set; } = string.Empty;

        [Required]
        public string EventDesc { get; set; } = string.Empty;

        [Required]
        public string EventDateTime { get; set; } = string.Empty;

        [Required]
        public string Visibility { get; set; } = string.Empty;
    }
}