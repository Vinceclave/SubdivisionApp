using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Subdivision.Models
{
    [Table("OnlineBanking")]
    public class OnlineBanking
    {
        [Key]
        public int OnlineBankingId { get; set; }
        
        [Required, StringLength(100)]
        public string BankName { get; set; } = string.Empty;
        
        [Required, StringLength(100)]
        public string AccountHolderName { get; set; } = string.Empty;
        
        [Required, StringLength(50)]
        public string AccountNumber { get; set; } = string.Empty;
        
        [Required, StringLength(50)]
        public string TransactionRefNumber { get; set; } = string.Empty;

        public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}