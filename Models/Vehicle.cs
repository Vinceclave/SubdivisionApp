using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Subdivision.Models
{
    public class Vehicle
    {
        [Key]
        public int VehicleId { get; set; }  // Primary Key
        
        [ForeignKey(nameof(Homeowner))]
        public int HomeownerId { get; set; }  // Foreign Key to Homeowner
        
        [Required, StringLength(50)]
        public string VehicleType { get; set; } = string.Empty;
        
        public string Brand { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public string PlateNo { get; set; } = string.Empty;
        public string Cor { get; set; } = string.Empty;
        public string VehicleStatus { get; set; } = string.Empty;

        // Navigation property to Homeowner
        public virtual Homeowner Homeowner { get; set; } = null!;
    }
}