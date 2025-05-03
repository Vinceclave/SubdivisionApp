using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Subdivision.Models
{
    public class Facility
    {
        [Key]
        public int FacilityId { get; set; }

        [Required, StringLength(50)]
        [Display(Name = "Type")]
        public string TypeOfFacility { get; set; } = string.Empty;

        [Required, Range(1, 1000)]
        [Display(Name = "Capacity")]
        public int FacilityCapacity { get; set; }

        [Display(Name = "Image")]
        public string FacilityImg { get; set; } = string.Empty;

        [Required, StringLength(100)]
        [Display(Name = "Name")]
        public string FacilityName { get; set; } = string.Empty;

        [Required, StringLength(100)]
        [Display(Name = "Working Days")]
        public string WorkingDays { get; set; } = string.Empty;

        [Required, StringLength(100)]
        [Display(Name = "Working Hours")]
        public string WorkingHours { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Rules & Regulations")]
        public string RulesRegulations { get; set; } = string.Empty;

        [Required, StringLength(20)]
        public string Status { get; set; } = string.Empty;

        [ForeignKey(nameof(Admin))]
        public int? AdminId { get; set; }

        public virtual Admin? Admin { get; set; }

        public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    }
}