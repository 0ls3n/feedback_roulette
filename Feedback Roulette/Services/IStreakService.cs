using FeedbackRoulette_ClassLibrary;

namespace Feedback_Roulette.Services
{
    public interface IStreakService
    {
        Task<UserStreak> RecordFeedbackAsync(string userId);
        Task<UserStreak> GetStreakAsync(string userId);
    }
}