using FeedbackRoulette_ClassLibrary;

namespace Feedback_Roulette.Services
{
    public interface IFollowService
    {
        Task FollowUserAsync(string followerUserId, string followedUserId);
        Task UnfollowUserAsync(string followerUserId, string followedUserId);
        Task<bool> IsFollowingAsync(string followerUserId, string followedUserId);
        Task<int> GetFollowerCountAsync(string userId);
        Task<int> GetFollowingCountAsync(string userId);
        Task<List<ApplicationUser>> GetFollowersAsync(string userId);
        Task<List<ApplicationUser>> GetFollowingAsync(string userId);
    }
}
