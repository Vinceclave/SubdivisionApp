using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Subdivision.Models
{
    [Table("CreditDebitCard")]
    public class CreditDebitCard
    {
        [Key]
        public int CardId { get; set; }

        [Required, StringLength(100)]
        [Display(Name = "Cardholder Name")]
        public string CardholderName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Expiry Date")]
        [DataType(DataType.Date)]
        public DateTime ExpiryDate { get; set; }

        [Required, StringLength(4)]
        [RegularExpression(@"^\d{3,4}$")]
        public string CVV { get; set; } = string.Empty;

        [Required, StringLength(19)]
        [RegularExpression(@"^\d{16}$")]
        [Display(Name = "Card Number")]
        public string CardNumber { get; set; } = string.Empty;

        public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}