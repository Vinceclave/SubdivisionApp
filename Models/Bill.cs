using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Subdivision.Models
{
    public class Bill
    {
        [Key]
        public int BillId { get; set; }
        
        [Display(Name = "Homeowner")]
        [ForeignKey(nameof(Homeowner))]
        public int HomeownerId { get; set; }
        
        [Display(Name = "Admin")]
        [ForeignKey(nameof(Admin))]
        public int AdminId { get; set; }
        
        [Required, Display(Name = "Bill Type")]
        [StringLength(50)]
        public string BillType { get; set; } = string.Empty;
        
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        [DataType(DataType.Currency)]
        public decimal Amount { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, double.MaxValue)]
        [DataType(DataType.Currency)]
        public decimal AmountPaid { get; set; }
        
        [Required, Display(Name = "Due Date")]
        [DataType(DataType.Date)]
        public DateTime DueDate { get; set; }
        
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime DateCreated { get; set; }
        
        [Required]
        [StringLength(20)]
        public string Status { get; set; } = string.Empty;

        public virtual Homeowner Homeowner { get; set; } = null!;
        public virtual Admin Admin { get; set; } = null!;
        public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}