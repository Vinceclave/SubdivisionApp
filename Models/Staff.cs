using System.ComponentModel.DataAnnotations.Schema;

namespace Subdivision.Models
{
    public enum StaffPosition
    {
        Security,
        Maintenance,
        Housekeeping
    }

    public class Staff
    {
        public int StaffId { get; set; }
        
        [ForeignKey("User")] // Specify LoginId as a foreign key
        public int LoginId { get; set; }
        
        public StaffPosition Position { get; set; }
        public virtual User? User { get; set; } // Marked as nullable

        public ICollection<VisitorPass> VisitorPasses { get; set; } = new List<VisitorPass>();

        // Navigation property for ServiceRequests
        public ICollection<ServiceRequest> ServiceRequests { get; set; } = new List<ServiceRequest>();
        public ICollection<Forum> Forums { get; set; } = new List<Forum>();
        public ICollection<ForumReplies> ForumReplies { get; set; } = new List<ForumReplies>();

    }
}