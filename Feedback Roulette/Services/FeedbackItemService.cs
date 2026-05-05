using FeedbackRoulette_ClassLibrary;
using Feedback_Roulette.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Feedback_Roulette.Services
{
    public class FeedbackItemService : IFeedbackItemService
    {
        private readonly IDbContextFactory<DataContext> _contextFactory;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _environment;

        public FeedbackItemService(IDbContextFactory<DataContext> contextFactory, UserManager<ApplicationUser> userManager, IWebHostEnvironment environment)
        {
            _contextFactory = contextFactory;
            _userManager = userManager;
            _environment = environment;
        }

        public async Task<List<FeedbackItem>> GetUserSubmissionsAsync(string userId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.FeedbackItems
                .Include(i => i.Category)
                .Include(i => i.Feedbacks)
                .Where(i => i.ApplicationUserId == userId)
                .OrderByDescending(i => i.Id)
                .ToListAsync();
        }

        public async Task<FeedbackItem?> GetSubmissionByIdAsync(int id, string userId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.FeedbackItems
                .Include(i => i.Category)
                .Include(i => i.Feedbacks)
                .FirstOrDefaultAsync(i => i.Id == id && i.ApplicationUserId == userId);
        }

        public async Task<List<FeedbackItem>> GetSubmissionsWithFeedbackAsync(string userId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.FeedbackItems
                .Include(i => i.Category)
                .Include(i => i.Feedbacks)
                .Where(i => i.ApplicationUserId == userId && i.Feedbacks.Any())
                .OrderByDescending(i => i.Feedbacks.Max(f => f.Id))
                .ToListAsync();
        }

        public async Task DeleteSubmissionAsync(int id, string userId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var item = await context.FeedbackItems
                .FirstOrDefaultAsync(i => i.Id == id && i.ApplicationUserId == userId);
            
            if (item != null)
            {
                if (!string.IsNullOrEmpty(item.FileUrl))
                {
                    var filePath = Path.Combine(_environment.WebRootPath, item.FileUrl.TrimStart('/'));
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }
                }

                context.FeedbackItems.Remove(item);
                await context.SaveChangesAsync();
            }
        }

        public async Task<FeedbackItem> CreateSubmissionAsync(string userId, string title, int categoryId, string description, string fileUrl, string fileType, string fileSize)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            
            var feedbackItem = new FeedbackItem
            {
                Title = title,
                Description = description,
                ApplicationUserId = userId,
                CategoryId = categoryId,
                FileUrl = fileUrl,
                FileType = fileType,
                FileSize = fileSize,
                Feedbacks = new List<Feedback>()
            };

            context.FeedbackItems.Add(feedbackItem);
            
            // Deduct credits
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                user.Credits -= 50;
                await _userManager.UpdateAsync(user);
            }
            
            await context.SaveChangesAsync();
            return feedbackItem;
        }

        public async Task<List<FeedbackItem>> GetAvailableItemsForReviewAsync(string userId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            
            // Get IDs of items already reviewed by this user
            var reviewedItemIds = await context.Feedbacks
                .Where(f => f.ApplicationUserId == userId)
                .Select(f => f.FeedbackItemId)
                .ToListAsync();

            // Find items not owned by user and not yet reviewed
            return await context.FeedbackItems
                .Include(i => i.Category)
                .Include(i => i.ApplicationUser)
                .Where(i => i.ApplicationUserId != userId && !reviewedItemIds.Contains(i.Id))
                .ToListAsync();
        }
    }
}
