using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Subdivision.Models
{
    public class VisitorPassRequest
    {
        [Key]
        public int VisitorPassRequestId { get; set; }
        
        [ForeignKey(nameof(Homeowner))]
        public int HomeownerId { get; set; }
        
        [Required, Display(Name = "Visitor Name")]
        [StringLength(100, MinimumLength = 2)]
        public string VisitorFullName { get; set; } = string.Empty;
        
        [Required, Display(Name = "Purpose")]
        [StringLength(200, MinimumLength = 5)]
        public string PurposeOfVisit { get; set; } = string.Empty;
        
        [Required]
        [StringLength(50)]
        public string TypeOfVehicle { get; set; } = string.Empty;
        
        [Required]
        [StringLength(20)]
        public string PlateNo { get; set; } = string.Empty;
        
        [Required]
        public DateTime DateOfVisit { get; set; }
        
        [Required]
        public TimeSpan TimeOfVisit { get; set; }
        
        [Required]
        [StringLength(20)]
        public string VisitorStatus { get; set; } = string.Empty;

        public virtual Homeowner Homeowner { get; set; } = null!;
        public virtual ICollection<VisitorPass> VisitorPasses { get; set; } = new List<VisitorPass>();
    }
}