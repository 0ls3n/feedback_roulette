using Microsoft.AspNetCore.Identity;

namespace FeedbackRoulette_ClassLibrary;

public class ApplicationUser : IdentityUser
{
    public int Credits { get; set; } = 100; // Default credits for new users
    public string? ProfileImageUrl { get; set; }
}