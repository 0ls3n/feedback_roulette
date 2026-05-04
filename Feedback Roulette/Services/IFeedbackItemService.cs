using FeedbackRoulette_ClassLibrary;
using Microsoft.AspNetCore.Identity;

namespace Feedback_Roulette.Services
{
    public interface IFeedbackItemService
    {
        Task<List<FeedbackItem>> GetUserSubmissionsAsync(string userId);
        Task<FeedbackItem?> GetSubmissionByIdAsync(int id, string userId);
        Task<List<FeedbackItem>> GetSubmissionsWithFeedbackAsync(string userId);
        Task DeleteSubmissionAsync(int id, string userId);
        Task<FeedbackItem> CreateSubmissionAsync(string userId, string title, int categoryId, string description, string fileUrl, string fileType, string fileSize);
        Task<List<FeedbackItem>> GetAvailableItemsForReviewAsync(string userId);
    }
}
