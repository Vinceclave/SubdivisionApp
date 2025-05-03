using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Subdivision.Models
{
    public class Admin
    {
        [Key]
        public int AdminId { get; set; }

        [ForeignKey(nameof(User))]
        public int LoginId { get; set; }

        public virtual User? User { get; set; }

        public virtual ICollection<Contact> Contacts { get; set; } = new List<Contact>();
        public virtual ICollection<Announcement> Announcements { get; set; } = new List<Announcement>();
        public virtual ICollection<Forum> Forums { get; set; } = new List<Forum>();
        public virtual ICollection<ForumReplies> ForumReplies { get; set; } = new List<ForumReplies>();
        public virtual ICollection<EventCalendar> EventsOrganized { get; set; } = new List<EventCalendar>();
        public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
        public virtual ICollection<Facility> Facilities { get; set; } = new List<Facility>();
    }
}