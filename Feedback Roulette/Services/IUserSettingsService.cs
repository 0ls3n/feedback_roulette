using FeedbackRoulette_ClassLibrary;
using Microsoft.AspNetCore.Identity;

namespace Feedback_Roulette.Services
{
    public interface IUserSettingsService
    {
        Task<ApplicationUser?> GetCurrentUserAsync(string userId);
        Task UpdateProfileImageAsync(string userId, string profileImageUrl);
        Task<bool> HasEnoughCreditsAsync(string userId, int amount);
        Task<int> GetUserCreditsAsync(string userId);
    }
}
