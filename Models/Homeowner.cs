using System.ComponentModel.DataAnnotations.Schema;

namespace Subdivision.Models
{
    public class Homeowner
    {
        public int HomeownerId { get; set; }  // Primary Key
        
        [ForeignKey("User")] // Specify LoginId as a foreign key
        public int LoginId { get; set; }  // Foreign Key to User

        // Navigation property for Feedbacks
        public ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

        // Navigation property for Complaints
        public ICollection<Complaint> Complaints { get; set; } = new List<Complaint>();
        public ICollection<VisitorPassRequest> VisitorPassRequests { get; set; } = new List<VisitorPassRequest>();

        // Navigation property for ServiceRequests
        public ICollection<ServiceRequest> ServiceRequests { get; set; } = new List<ServiceRequest>();

        public ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
        public ICollection<Forum> Forums { get; set; } = new List<Forum>();
        public ICollection<ForumReplies> ForumReplies { get; set; } = new List<ForumReplies>();
        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();

        public ICollection<Bill> Bills { get; set; } = new List<Bill>();

        public virtual User? User { get; set; } // Marked as nullable
    }
}