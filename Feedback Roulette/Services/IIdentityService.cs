using FeedbackRoulette_ClassLibrary;

namespace Feedback_Roulette.Services;

public interface IIdentityService
{
    public Task<ApplicationUser> GetCurrentUserAsync();
    public Task<ApplicationUser?> GetUserByIdAsync(string userId);
}