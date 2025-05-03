using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Subdivision.Models
{
    public enum UserType
    {
        Admin,      // Will be 0
        Staff,      // Will be 1
        Homeowner   // Will be 2
    }

    public class User
    {
        [Key]
        public int LoginId { get; set; }

        [Required, StringLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required, StringLength(100)]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required, StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required, StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required, StringLength(200)]
        public string Address { get; set; } = string.Empty;

        [Required, StringLength(20)]
        [Phone]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required, StringLength(100)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public string Image { get; set; } = string.Empty;

        [Required]
        public UserType UserType { get; set; }

        public virtual Admin? Admin { get; set; }
        public virtual Homeowner? Homeowner { get; set; }
        public virtual Staff? Staff { get; set; }

        public ICollection<EventVisibility> EventVisibilities { get; set; } = new List<EventVisibility>();

    }
}