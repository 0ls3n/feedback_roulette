using FeedbackRoulette_ClassLibrary;

namespace Feedback_Roulette.Data;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class DataContext : IdentityDbContext<ApplicationUser>
{
        public DbSet<Category> Categories { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<FeedbackItem> FeedbackItems { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<UserStreak> UserStreaks { get; set; }
    public DbSet<UserFollow> UserFollows { get; set; }
    
    public DataContext(DbContextOptions<DataContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Music", Description = "Songwriting, mixing, mastering, and performance." },
            new Category { Id = 2, Name = "Programming", Description = "Code reviews, architecture, and logic." },
            new Category { Id = 3, Name = "Design", Description = "UI/UX, graphic design, and branding." }
        );
    }
}