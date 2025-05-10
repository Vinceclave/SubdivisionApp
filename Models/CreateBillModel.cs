using System.ComponentModel.DataAnnotations;

namespace Subdivision.Models
{
    public class CreateBillModel
    {
        [Required]
        public int HomeownerId { get; set; }

        [Required]
        [StringLength(50)]
        public string BillType { get; set; } = string.Empty;

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }

        [Required]
        public DateTime DueDate { get; set; }
    }
}