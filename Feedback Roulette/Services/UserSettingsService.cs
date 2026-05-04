using FeedbackRoulette_ClassLibrary;
using Microsoft.AspNetCore.Identity;

namespace Feedback_Roulette.Services
{
    public class UserSettingsService : IUserSettingsService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserSettingsService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<ApplicationUser?> GetCurrentUserAsync(string userId)
        {
            return await _userManager.FindByIdAsync(userId);
        }

        public async Task UpdateProfileImageAsync(string userId, string profileImageUrl)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                user.ProfileImageUrl = profileImageUrl;
                await _userManager.UpdateAsync(user);
            }
        }

        public async Task<bool> HasEnoughCreditsAsync(string userId, int amount)
        {
            var user = await _userManager.FindByIdAsync(userId);
            return user != null && user.Credits >= amount;
        }

        public async Task<int> GetUserCreditsAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            return user?.Credits ?? 0;
        }
    }
}
