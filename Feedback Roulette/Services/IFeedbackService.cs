using FeedbackRoulette_ClassLibrary;

namespace Feedback_Roulette.Services
{
    public interface IFeedbackService
    {
        Task<Feedback> SubmitFeedbackAsync(string userId, int feedbackItemId, string? positiveFeedback, string? negativeFeedback, string? suggestion);
        Task<List<Feedback>> GetFeedbackForItemAsync(int itemId, string userId);
        Task<int> GetFeedbackCountForItemAsync(int itemId, string userId);
        Task<List<Feedback>> GetFeedbackByUserAsync(string userId);
        Task<Feedback?> GetFeedbackByIdAsync(int feedbackId, string userId);
    }
}
