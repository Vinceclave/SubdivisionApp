namespace Subdivision.Models
{
    public class EventVisibility
    {
        public int EventVisibilityId { get; set; }  // Primary Key
        public int EventId { get; set; }  // Foreign Key to EventCalendar
        public int LoginId { get; set; }  // Foreign Key to User

        // Navigation properties
        public EventCalendar Event { get; set; } = null!;
        public User User { get; set; } = null!;
    }
}