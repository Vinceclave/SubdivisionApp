using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Subdivision.Models
{
    public class Reservation
    {
        [Key]
        public int ReservationId { get; set; }  // Primary Key
        
        [ForeignKey(nameof(Facility))]
        public int FacilityId { get; set; }  // Foreign Key to Facility
        
        [ForeignKey(nameof(Homeowner))]
        public int HomeownerId { get; set; }  // Foreign Key to Homeowner
        
        [ForeignKey(nameof(Admin))]
        public int AdminId { get; set; }  // Foreign Key to Admin
        
        [Range(1, 1000)]
        public int Capacity { get; set; }
        
        public DateTime ReservationTimeIn { get; set; }
        public DateTime ReservationTimeOut { get; set; }
        public DateTime DateOfReservation { get; set; }
        public string Status { get; set; } = string.Empty;

        // Navigation properties
        public virtual Facility Facility { get; set; } = null!;
        public virtual Homeowner Homeowner { get; set; } = null!;
        public virtual Admin Admin { get; set; } = null!;
    }
}