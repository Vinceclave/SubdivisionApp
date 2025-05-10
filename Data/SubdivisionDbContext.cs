namespace Subdivision.Data
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Diagnostics;
    using Subdivision.Models;

    public class SubdivisionDbContext : DbContext
    {
        public SubdivisionDbContext(DbContextOptions<SubdivisionDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Homeowner> Homeowners { get; set; }
        public DbSet<Staff> Staffs { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Announcement> Announcements { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<Complaint> Complaints { get; set; }
        public DbSet<VisitorPassRequest> VisitorPassRequests { get; set; }
        public DbSet<VisitorPass> VisitorPasses { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<Forum> Forums { get; set; }
        public DbSet<ForumReplies> ForumReplies { get; set; }
        public DbSet<ServiceRequest> ServiceRequests { get; set; }
        public DbSet<EventVisibility> EventVisibilities { get; set; }
        public DbSet<EventCalendar> EventCalendars { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<Facility> Facilities { get; set; }
        public DbSet<CreditDebitCard> CreditDebitCards { get; set; }
        public DbSet<OnlineBanking> OnlineBankings { get; set; }
        public DbSet<Bill> Bills { get; set; }
        public DbSet<Payment> Payments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User entity configuration
            modelBuilder.Entity<User>().HasKey(u => u.LoginId);

            modelBuilder.Entity<User>().HasOne(u => u.Admin).WithOne(a => a.User).HasForeignKey<Admin>(a => a.LoginId).OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<User>().HasOne(u => u.Homeowner).WithOne(h => h.User).HasForeignKey<Homeowner>(h => h.LoginId).OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<User>().HasOne(u => u.Staff).WithOne(s => s.User).HasForeignKey<Staff>(s => s.LoginId).OnDelete(DeleteBehavior.NoAction);

            // Admin entity configuration
            modelBuilder.Entity<Admin>().HasKey(a => a.AdminId);
            modelBuilder.Entity<Admin>().HasMany(a => a.Contacts).WithOne(c => c.Admin).HasForeignKey(c => c.AdminId).OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Admin>().HasMany(a => a.EventsOrganized).WithOne(ec => ec.OrganizedBy).HasForeignKey(ec => ec.OrganizedById).OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Admin>().HasMany(a => a.Reservations).WithOne(r => r.Admin).HasForeignKey(r => r.AdminId).OnDelete(DeleteBehavior.NoAction);

            // Homeowner entity configuration
            modelBuilder.Entity<Homeowner>().HasKey(h => h.HomeownerId);
            modelBuilder.Entity<Homeowner>().HasOne(h => h.User).WithOne(u => u.Homeowner).HasForeignKey<Homeowner>(h => h.LoginId).OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Homeowner>().HasMany(h => h.Feedbacks).WithOne(f => f.Homeowner).HasForeignKey(f => f.HomeownerId);
            modelBuilder.Entity<Homeowner>().HasMany(h => h.Complaints).WithOne(c => c.Homeowner).HasForeignKey(c => c.HomeownerId);
            modelBuilder.Entity<Homeowner>().HasMany(h => h.VisitorPassRequests).WithOne(vpr => vpr.Homeowner).HasForeignKey(vpr => vpr.HomeownerId);
            modelBuilder.Entity<Homeowner>().HasMany(h => h.ServiceRequests).WithOne(sr => sr.Homeowner).HasForeignKey(sr => sr.HomeownerId);
            modelBuilder.Entity<Homeowner>().HasMany(h => h.Vehicles).WithOne(v => v.Homeowner).HasForeignKey(v => v.HomeownerId);
            modelBuilder.Entity<Homeowner>().HasMany(h => h.Reservations).WithOne(r => r.Homeowner).HasForeignKey(r => r.HomeownerId).OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Homeowner>().HasMany(h => h.Bills).WithOne(b => b.Homeowner).HasForeignKey(b => b.HomeownerId).OnDelete(DeleteBehavior.NoAction);

            // Staff entity configuration
            modelBuilder.Entity<Staff>().HasKey(s => s.StaffId);
            modelBuilder.Entity<Staff>().HasOne(s => s.User).WithOne(u => u.Staff).HasForeignKey<Staff>(s => s.LoginId).OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Staff>().HasMany(s => s.ServiceRequests).WithOne(sr => sr.Staff).HasForeignKey(sr => sr.StaffId);

            // Contact entity configuration
            modelBuilder.Entity<Contact>().HasKey(c => c.ContactId);
            modelBuilder.Entity<Contact>().HasOne(c => c.Admin).WithMany(a => a.Contacts).HasForeignKey(c => c.AdminId).OnDelete(DeleteBehavior.NoAction);

            // Announcement entity configuration
            modelBuilder.Entity<Announcement>().HasKey(a => a.AnnouncementId);

            // Feedback entity configuration
            modelBuilder.Entity<Feedback>().HasKey(f => f.FeedbackId);

            // Complaint entity configuration
            modelBuilder.Entity<Complaint>().HasKey(c => c.ComplaintId);

            // VisitorPassRequest entity configuration
            modelBuilder.Entity<VisitorPassRequest>().HasKey(vpr => vpr.VisitorPassRequestId);

            // VisitorPass entity configuration
            modelBuilder.Entity<VisitorPass>().HasKey(vp => vp.VisitorPassId);
            modelBuilder.Entity<VisitorPass>()
                .HasOne(vp => vp.VisitorPassRequest)
                .WithMany(vpr => vpr.VisitorPasses)
                .HasForeignKey(vp => vp.VisitorPassRequestId);
            
            modelBuilder.Entity<VisitorPass>()
                .HasOne(vp => vp.Staff)
                .WithMany()
                .HasForeignKey(vp => vp.LoggedById)
                .OnDelete(DeleteBehavior.NoAction);

            // ServiceRequest entity configuration
            modelBuilder.Entity<ServiceRequest>().HasKey(sr => sr.ServiceRequestId);

            // Vehicle entity configuration
            modelBuilder.Entity<Vehicle>().HasKey(v => v.VehicleId);
            modelBuilder.Entity<Vehicle>().HasOne(v => v.Homeowner).WithMany(h => h.Vehicles).HasForeignKey(v => v.HomeownerId);

            // Forum entity configuration
            modelBuilder.Entity<Forum>().HasKey(f => f.ForumId);

            // ForumReplies entity configuration
            modelBuilder.Entity<ForumReplies>().HasKey(fr => fr.ForumRepliesId);

            // EventVisibility entity configuration
            modelBuilder.Entity<EventVisibility>().HasKey(ev => ev.EventVisibilityId);

            modelBuilder.Entity<EventVisibility>()
                .HasOne(ev => ev.Event)
                .WithMany(ec => ec.EventVisibilities)
                .HasForeignKey(ev => ev.EventId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<EventVisibility>()
                .HasOne(ev => ev.User)
                .WithMany(up => up.EventVisibilities)
                .HasForeignKey(ev => ev.LoginId)
                .OnDelete(DeleteBehavior.NoAction);

            // EventCalendar entity configuration
            modelBuilder.Entity<EventCalendar>().HasKey(ec => ec.EventId);

            modelBuilder.Entity<EventCalendar>()
                .HasOne(ec => ec.OrganizedBy)
                .WithMany(a => a.EventsOrganized)
                .HasForeignKey(ec => ec.OrganizedById)
                .OnDelete(DeleteBehavior.NoAction);

            // Reservation entity configuration
            modelBuilder.Entity<Reservation>().HasKey(r => r.ReservationId);
            modelBuilder.Entity<Reservation>().HasOne(r => r.Facility).WithMany(f => f.Reservations).HasForeignKey(r => r.FacilityId).OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Reservation>().HasOne(r => r.Homeowner).WithMany(h => h.Reservations).HasForeignKey(r => r.HomeownerId).OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Reservation>().HasOne(r => r.Admin).WithMany(a => a.Reservations).HasForeignKey(r => r.AdminId).OnDelete(DeleteBehavior.NoAction);

            // Facility entity configuration
            modelBuilder.Entity<Facility>().HasKey(f => f.FacilityId);
            modelBuilder.Entity<Facility>()
                .HasOne(f => f.Admin)
                .WithMany(a => a.Facilities)  // Adding the navigation property to Admin
                .HasForeignKey(f => f.AdminId)
                .OnDelete(DeleteBehavior.NoAction);

            // CreditDebitCard entity configuration
            modelBuilder.Entity<CreditDebitCard>().HasKey(cdc => cdc.CardId);
            modelBuilder.Entity<CreditDebitCard>().HasMany(cdc => cdc.Payments)
                .WithOne(p => p.CreditDebitCard)
                .HasForeignKey(p => p.CardId)
                .OnDelete(DeleteBehavior.NoAction);

            // OnlineBanking entity configuration
            modelBuilder.Entity<OnlineBanking>().HasKey(ob => ob.OnlineBankingId);
            modelBuilder.Entity<OnlineBanking>().HasMany(ob => ob.Payments)
                .WithOne(p => p.OnlineBanking)
                .HasForeignKey(p => p.OnlineBankingId)
                .OnDelete(DeleteBehavior.NoAction);

            // Bill entity configuration
            modelBuilder.Entity<Bill>().HasKey(b => b.BillId);
            modelBuilder.Entity<Bill>()
                .HasOne(b => b.Homeowner)
                .WithMany(h => h.Bills)
                .HasForeignKey(b => b.HomeownerId)
                .OnDelete(DeleteBehavior.NoAction);
            
            modelBuilder.Entity<Bill>()
                .HasOne(b => b.Admin)
                .WithMany()
                .HasForeignKey(b => b.AdminId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Bill>().Property(b => b.Amount).HasPrecision(18, 2);
            modelBuilder.Entity<Bill>().HasMany(b => b.Payments)
                .WithOne(p => p.Bill)
                .HasForeignKey(p => p.BillId);

            // Payment entity configuration
            modelBuilder.Entity<Payment>().HasKey(p => p.PaymentId);
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Bill)
                .WithMany(b => b.Payments)
                .HasForeignKey(p => p.BillId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Homeowner)
                .WithMany()
                .HasForeignKey(p => p.HomeownerId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Payment>()
                .HasOne(p => p.CreditDebitCard)
                .WithMany(cdc => cdc.Payments)
                .HasForeignKey(p => p.CardId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Payment>()
                .HasOne(p => p.OnlineBanking)
                .WithMany(ob => ob.Payments)
                .HasForeignKey(p => p.OnlineBankingId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Payment>().Property(p => p.AmountPaid)
                .HasPrecision(18, 2);

            // Seed admin user
            modelBuilder.Entity<User>().HasData(new User 
            { 
                LoginId = 1,
                Username = "admin",
                // Hashed "admin123" using SHA256
                Password = "$2a$11$8icRySkuU81x31OMdr7m4emQJNBInJzOzSoe.Okg0uISEsMW4YL5i",
                FirstName = "Admin",
                LastName = "User",
                Address = "Admin Address",
                PhoneNumber = "0000000000",
                Email = "admin@example.com",
                Image = "default.png"
            });

            modelBuilder.Entity<Admin>().HasData(new Admin 
            { 
                AdminId = 1,
                LoginId = 1
            });
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.ConfigureWarnings(warnings => warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
        }
    }
}