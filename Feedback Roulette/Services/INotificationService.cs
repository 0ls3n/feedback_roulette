using FeedbackRoulette_ClassLibrary;

namespace Feedback_Roulette.Services
{
    public interface INotificationService
    {
        Task<List<Notification>> GetUnreadNotificationsAsync(string userId);
        Task<List<Notification>> GetAllNotificationsAsync(string userId);
        Task<int> GetUnreadCountAsync(string userId);
        Task MarkAsReadAsync(int notificationId);
        Task MarkAllAsReadAsync(string userId);
        Task CreateNotificationAsync(string userId, string message, string? link = null);
    }
}
