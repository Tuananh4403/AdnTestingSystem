using AdnTestingSystem.Repositories.Models;
using Microsoft.EntityFrameworkCore;

namespace AdnTestingSystem.Repositories.Data
{
    public class AdnTestingDbContext : DbContext
    {
        public AdnTestingDbContext(DbContextOptions<AdnTestingDbContext> options) : base(options) { }

        // DbSet
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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Profile)
                .WithOne(p => p.User)
                .HasForeignKey<UserProfile>(p => p.UserId);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.TestResult)
                .WithOne(r => r.Booking)
                .HasForeignKey<TestResult>(r => r.BookingId);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Rating)
                .WithOne(r => r.Booking)
                .HasForeignKey<Rating>(r => r.BookingId);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Transaction)
                .WithOne(t => t.Booking)
                .HasForeignKey<Transaction>(t => t.BookingId);

            modelBuilder.Entity<DnaTestService>()
                .HasMany(s => s.Prices)
                .WithOne(p => p.DnaTestService)
                .HasForeignKey(p => p.DnaTestServiceId);

            modelBuilder.Entity<DnaTestService>()
                .HasMany(s => s.Bookings)
                .WithOne(b => b.DnaTestService)
                .HasForeignKey(b => b.DnaTestServiceId);

            modelBuilder.Entity<Booking>()
                .HasMany(b => b.Samples)
                .WithOne(s => s.Booking)
                .HasForeignKey(s => s.BookingId);

            modelBuilder.Entity<Blog>()
                .HasOne(b => b.Author)
                .WithMany()
                .HasForeignKey(b => b.AuthorId);
        }
    }
}
