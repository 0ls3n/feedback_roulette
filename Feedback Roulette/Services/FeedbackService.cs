using FeedbackRoulette_ClassLibrary;
using Feedback_Roulette.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Feedback_Roulette.Services
{
    public class FeedbackService : IFeedbackService
    {
        private readonly IDbContextFactory<DataContext> _contextFactory;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly INotificationService _notificationService;

        public FeedbackService(IDbContextFactory<DataContext> contextFactory, UserManager<ApplicationUser> userManager, INotificationService notificationService)
        {
            _contextFactory = contextFactory;
            _userManager = userManager;
            _notificationService = notificationService;
        }

        public async Task<Feedback> SubmitFeedbackAsync(string userId, int feedbackItemId, string? positiveFeedback, string? negativeFeedback, string? suggestion)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            
            var feedback = new Feedback
            {
                ApplicationUserId = userId,
                FeedbackItemId = feedbackItemId,
                HasPositiveFeedback = !string.IsNullOrWhiteSpace(positiveFeedback),
                HasNegativeFeedback = !string.IsNullOrWhiteSpace(negativeFeedback),
                HasSuggestion = !string.IsNullOrWhiteSpace(suggestion),
                PositiveFeedback = positiveFeedback ?? string.Empty,
                NegativeFeedback = negativeFeedback ?? string.Empty,
                Suggestion = suggestion ?? string.Empty
            };

            context.Feedbacks.Add(feedback);
            
            // Reward credits to reviewer
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                user.Credits += 10;
                await _userManager.UpdateAsync(user);
            }
            
            await context.SaveChangesAsync();

            // Create notification for the owner of the feedback item
            var item = await context.FeedbackItems
                .FirstOrDefaultAsync(i => i.Id == feedbackItemId);
            
            if (item != null && !string.IsNullOrEmpty(item.ApplicationUserId))
            {
                var notificationMessage = $"New feedback received on your submission '{item.Title}'";
                var notificationLink = $"/feedback-received/{item.Id}";
                await _notificationService.CreateNotificationAsync(item.ApplicationUserId, notificationMessage, notificationLink);
            }
            
            return feedback;
        }

        public async Task<List<Feedback>> GetFeedbackForItemAsync(int itemId, string userId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var item = await context.FeedbackItems
                .Include(i => i.Feedbacks)
                .FirstOrDefaultAsync(i => i.Id == itemId && i.ApplicationUserId == userId);
            
            return item?.Feedbacks ?? new List<Feedback>();
        }

        public async Task<int> GetFeedbackCountForItemAsync(int itemId, string userId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Feedbacks
                .CountAsync(f => f.FeedbackItemId == itemId);
        }

        public async Task<List<Feedback>> GetFeedbackByUserAsync(string userId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Feedbacks
                .Where(f => f.ApplicationUserId == userId)
                .Include(f => f.FeedbackItem)
                .ThenInclude(fi => fi.Category)
                .Include(f => f.FeedbackItem.ApplicationUser)
                .OrderByDescending(f => f.FeedbackItemId)
                .ToListAsync();
        }

        public async Task<Feedback?> GetFeedbackByIdAsync(int feedbackId, string userId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Feedbacks
                .Where(f => f.Id == feedbackId && f.ApplicationUserId == userId)
                .Include(f => f.FeedbackItem)
                .ThenInclude(fi => fi.Category)
                .Include(f => f.FeedbackItem.ApplicationUser)
                .FirstOrDefaultAsync();
        }
    }
}
