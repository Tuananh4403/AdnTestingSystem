using AdnTestingSystem.Repositories.Models;
using Microsoft.EntityFrameworkCore;

namespace AdnTestingSystem.Repositories.Data
{
    public class AdnTestingDbContext : DbContext
    {
        public AdnTestingDbContext(DbContextOptions<AdnTestingDbContext> options) : base(options) { }

        // DbSet declarations
        public DbSet<User> Users => Set<User>();
        public DbSet<UserProfile> UserProfiles => Set<UserProfile>();
        public DbSet<DnaTestService> DnaTestServices => Set<DnaTestService>();
        public DbSet<ServicePrice> ServicePrices => Set<ServicePrice>();
        public DbSet<Booking> Bookings => Set<Booking>();
        public DbSet<Sample> Samples => Set<Sample>();
        public DbSet<TestResult> TestResults => Set<TestResult>();
        public DbSet<Rating> Ratings => Set<Rating>();
        public DbSet<Transaction> Transactions => Set<Transaction>();
        public DbSet<Blog> Blogs => Set<Blog>();
        public DbSet<BookingAttachment> BookingAttachments => Set<BookingAttachment>();
        public DbSet<SampleReceipt> SampleReceipts { get; set; }
        public DbSet<SampleReceiptDetail> SampleReceiptDetails { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User ↔ UserProfile (1:1)
            modelBuilder.Entity<User>()
                .HasOne(u => u.Profile)
                .WithOne(p => p.User)
                .HasForeignKey<UserProfile>(p => p.UserId);

            // Booking ↔ TestResult (1:1)
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.TestResult)
                .WithOne(r => r.Booking)
                .HasForeignKey<TestResult>(r => r.BookingId);

            // Booking ↔ Rating (1:1)
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Rating)
                .WithOne(r => r.Booking)
                .HasForeignKey<Rating>(r => r.BookingId);

            // Booking ↔ Transaction (1:1)
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Transaction)
                .WithOne(t => t.Booking)
                .HasForeignKey<Transaction>(t => t.BookingId);

            // DnaTestService ↔ ServicePrices (1:N)
            modelBuilder.Entity<DnaTestService>()
                .HasMany(s => s.Prices)
                .WithOne(p => p.DnaTestService)
                .HasForeignKey(p => p.DnaTestServiceId);

            // DnaTestService ↔ Bookings (1:N)
            modelBuilder.Entity<DnaTestService>()
                .HasMany(s => s.Bookings)
                .WithOne(b => b.DnaTestService)
                .HasForeignKey(b => b.DnaTestServiceId);

            // Booking ↔ Samples (1:N)
            modelBuilder.Entity<Booking>()
                .HasMany(b => b.Samples)
                .WithOne(s => s.Booking)
                .HasForeignKey(s => s.BookingId);

            // Blog ↔ User (Author) (1:N)
            modelBuilder.Entity<Blog>()
                .HasOne(b => b.Author)
                .WithMany()
                .HasForeignKey(b => b.AuthorId);

            // Booking ↔ BookingAttachments (1:N)
            modelBuilder.Entity<BookingAttachment>()
                .HasOne(ba => ba.Booking)
                .WithMany(b => b.BookingAttachments)
                .HasForeignKey(ba => ba.BookingId)
                .OnDelete(DeleteBehavior.Cascade); 

            // BookingAttachment ↔ User (UploadedBy = Staff) (N:1)
            modelBuilder.Entity<BookingAttachment>()
                .HasOne(ba => ba.Staff)
                .WithMany()
                .HasForeignKey(ba => ba.UploadedBy)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SampleReceipt>()
                .HasMany(r => r.SampleDetails)
                .WithOne(d => d.SampleReceipt)
                .HasForeignKey(d => d.SampleReceiptId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TestResult>()
               .HasMany(r => r.LocusResults)
               .WithOne(d => d.TestResult)
               .HasForeignKey(d => d.TestResultId)
               .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
