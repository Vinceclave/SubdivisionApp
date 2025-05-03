using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Subdivision.Models
{
    public class ServiceRequest
    {
        [Key]
        public int ServiceRequestId { get; set; }  // Primary Key

        [ForeignKey(nameof(Homeowner))]
        public int HomeownerId { get; set; }  // Foreign Key to Homeowner

        [ForeignKey(nameof(Staff))]
        public int? StaffId { get; set; }  // Foreign Key to Staff (nullable if not yet assigned)

        [Required, StringLength(100)]
        public string RequestType { get; set; } = string.Empty;

        public string Priority { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime RequestDateTime { get; set; }
        public string ServiceStatus { get; set; } = "Pending";

        // Navigation properties
        public virtual Homeowner Homeowner { get; set; } = null!;
        public virtual Staff? Staff { get; set; }

        public void UpdateStatus(string newStatus)
        {
            if (ServiceStatus == "Completed")
            {
                throw new InvalidOperationException("Cannot modify a completed service request.");
            }

            if (newStatus == "In Progress" && ServiceStatus != "Pending")
            {
                throw new InvalidOperationException("Can only start a service request that is pending.");
            }

            if (newStatus == "Completed" && ServiceStatus != "In Progress")
            {
                throw new InvalidOperationException("Can only complete a service request that is in progress.");
            }

            ServiceStatus = newStatus;
        }
    }

 
}