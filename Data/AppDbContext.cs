using backend.Model;
using Microsoft.EntityFrameworkCore;


namespace backend.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<Part> Parts { get; set; }
        public DbSet<Sales> Sales { get; set; }
        public DbSet<SalesItem> SalesItems { get; set; }
        public DbSet<Purchase> Purchases { get; set; }
        public DbSet<PurchaseItem> PurchaseItems { get; set; }
        public DbSet<Vendor> Vendors { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<PartRequest> PartRequests { get; set; }
        public DbSet<Notification> Notifications { get; set; } 
        public DbSet<Payment> Payments { get; set; }          

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // The initial migration created these tables without plural names
            // Map them so EF doesn't look for "Appointments", "Reviews", etc.
            modelBuilder.Entity<Appointment>().ToTable("Appointment");
            modelBuilder.Entity<Review>().ToTable("Review");
            modelBuilder.Entity<Notification>().ToTable("Notification");
        }
    }
}