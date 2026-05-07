using Feedback_Roulette.Data;
using FeedbackRoulette_ClassLibrary;
using Microsoft.EntityFrameworkCore;

namespace Feedback_Roulette.Services
{
    public class StreakService : IStreakService
    {
        private readonly IDbContextFactory<DataContext> _contextFactory;

        public StreakService(IDbContextFactory<DataContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<UserStreak> RecordFeedbackAsync(string userId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            var streak = await context.UserStreaks
                .FirstOrDefaultAsync(s => s.ApplicationUserId == userId);

            var today = DateTime.UtcNow.Date;
            var yesterday = today.AddDays(-1);

            if (streak == null)
            {
                streak = new UserStreak
                {
                    ApplicationUserId = userId,
                    CurrentStreak = 1,
                    LongestStreak = 1,
                    LastFeedbackDate = today,
                    StreakStartedAt = today
                };
                context.UserStreaks.Add(streak);
            }
            else if (streak.LastFeedbackDate.Date == today)
            {
                // Already gave feedback today, no streak change
            }
            else if (streak.LastFeedbackDate.Date == yesterday)
            {
                // Consecutive day
                streak.CurrentStreak++;
                streak.LastFeedbackDate = today;
                if (streak.CurrentStreak > streak.LongestStreak)
                {
                    streak.LongestStreak = streak.CurrentStreak;
                }
            }
            else
            {
                // Streak broken
                streak.CurrentStreak = 1;
                streak.LastFeedbackDate = today;
                streak.StreakStartedAt = today;
            }

            await context.SaveChangesAsync();
            return streak;
        }

        public async Task<UserStreak> GetStreakAsync(string userId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            var streak = await context.UserStreaks
                .FirstOrDefaultAsync(s => s.ApplicationUserId == userId);

            if (streak == null)
            {
                return new UserStreak
                {
                    ApplicationUserId = userId,
                    CurrentStreak = 0,
                    LongestStreak = 0
                };
            }

            // Check if streak should be reset (last feedback was > 1 day ago)
            var today = DateTime.UtcNow.Date;
            var yesterday = today.AddDays(-1);

            if (streak.LastFeedbackDate.Date < yesterday)
            {
                streak.CurrentStreak = 0;
                context.UserStreaks.Update(streak);
                await context.SaveChangesAsync();
            }

            return streak;
        }
    }
}