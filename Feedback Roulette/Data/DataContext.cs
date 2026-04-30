using FeedbackRoulette_ClassLibrary;

namespace Feedback_Roulette.Data;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class DataContext : IdentityDbContext<ApplicationUser>
{
    public DbSet<Category> Categories { get; set; }
    public DbSet<Feedback> Feedbacks { get; set; }
    public DbSet<FeedbackItem> FeedbackItems { get; set; }
    
    public DataContext(DbContextOptions<DataContext> options)
        : base(options)
    {
    }
}