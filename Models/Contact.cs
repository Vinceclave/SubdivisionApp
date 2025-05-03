using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Subdivision.Models
{
    public enum ContactCategory
    {
        HOA_Boards,        // Default is 0
        Security,          // Default is 1
        Fire_Department,   // Default is 2
        Medical_Emergency   // Default is 3
    }

    public class Contact
    {
        [Key]
        public int ContactId { get; set; }

        [Required, StringLength(100)]
        [Display(Name = "Contact Person")]
        public string ContactPersonName { get; set; } = string.Empty;

        [Required, StringLength(20)]
        [Phone]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required, StringLength(100)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public ContactCategory Category { get; set; } // Property for category

        [ForeignKey(nameof(Admin))]
        public int AdminId { get; set; }

        public virtual Admin Admin { get; set; } = null!;
    }
}