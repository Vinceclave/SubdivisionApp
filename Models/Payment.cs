using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Subdivision.Models
{
    public class Payment
    {
        [Key]
        public int PaymentId { get; set; }
        
        [Required, Display(Name = "Bill")]
        [ForeignKey(nameof(Bill))]
        public int BillId { get; set; }
        
        [Required, Display(Name = "Homeowner")]
        [ForeignKey(nameof(Homeowner))]
        public int HomeownerId { get; set; }
        
        [Required, Display(Name = "Amount Paid")]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        [DataType(DataType.Currency)]
        public decimal AmountPaid { get; set; }
        
        [Required, Display(Name = "Payment Mode")]
        [StringLength(50)]
        public string ModeOfPayment { get; set; } = string.Empty;
        
        [Required, Display(Name = "Payment Date")]
        [DataType(DataType.DateTime)]
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = string.Empty;
        
        [ForeignKey(nameof(CreditDebitCard))]
        public int? CardId { get; set; }
        
        [ForeignKey(nameof(OnlineBanking))]
        public int? OnlineBankingId { get; set; }

        public virtual Bill Bill { get; set; } = null!;
        public virtual Homeowner Homeowner { get; set; } = null!;
        public virtual CreditDebitCard? CreditDebitCard { get; set; }
        public virtual OnlineBanking? OnlineBanking { get; set; }
    }
}