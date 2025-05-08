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
        public int ContactId { get; set; }
        public string ContactPersonName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public ContactCategory Category { get; set; }
        
        [ForeignKey(nameof(Admin))]
        public int AdminId { get; set; }
        public virtual Admin Admin { get; set; } = null!;
    }
}