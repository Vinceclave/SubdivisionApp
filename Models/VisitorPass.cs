using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Subdivision.Models
{
    public class VisitorPass
    {
        [Key]
        public int VisitorPassId { get; set; }
        
        [Display(Name = "Visitor Pass Request")]
        [ForeignKey(nameof(VisitorPassRequest))]
        public int VisitorPassRequestId { get; set; }
        
        [Required, Display(Name = "Time In")]
        [DataType(DataType.DateTime)]
        public DateTime TimeIn { get; set; }
        
        [Display(Name = "Time Out")]
        [DataType(DataType.DateTime)]
        public DateTime? TimeOut { get; set; }
        
        [Required, Display(Name = "Status")]
        [StringLength(20)]
        public string PassStatus { get; set; } = "Active";
        
        [Required, Display(Name = "Logged By")]
        [ForeignKey(nameof(Staff))]
        public int LoggedById { get; set; }

        [Display(Name = "Logged By Staff")]
        public virtual Staff Staff { get; set; } = null!;
        
        public virtual VisitorPassRequest VisitorPassRequest { get; set; } = null!;
    }
}