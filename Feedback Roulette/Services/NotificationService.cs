using FeedbackRoulette_ClassLibrary;
using Feedback_Roulette.Data;
using Microsoft.EntityFrameworkCore;

namespace Feedback_Roulette.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IDbContextFactory<DataContext> _contextFactory;

        public NotificationService(IDbContextFactory<DataContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<List<Notification>> GetUnreadNotificationsAsync(string userId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Notifications
                .Where(n => n.ApplicationUserId == userId && !n.IsRead)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Notification>> GetAllNotificationsAsync(string userId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Notifications
                .Where(n => n.ApplicationUserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task<int> GetUnreadCountAsync(string userId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Notifications
                .CountAsync(n => n.ApplicationUserId == userId && !n.IsRead);
        }

        public async Task MarkAsReadAsync(int notificationId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var notification = await context.Notifications.FindAsync(notificationId);
            if (notification != null)
            {
                notification.IsRead = true;
                await context.SaveChangesAsync();
            }
        }

        public async Task MarkAllAsReadAsync(string userId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var notifications = await context.Notifications
                .Where(n => n.ApplicationUserId == userId && !n.IsRead)
                .ToListAsync();

            foreach (var notification in notifications)
            {
                notification.IsRead = true;
            }

            await context.SaveChangesAsync();
        }

        public async Task CreateNotificationAsync(string userId, string message, string? link = null)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var notification = new Notification
            {
                ApplicationUserId = userId,
                Message = message,
                Link = link,
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            };

            context.Notifications.Add(notification);
            await context.SaveChangesAsync();
        }
    }
}
