using FeedbackRoulette_ClassLibrary;
using Feedback_Roulette.Data;
using Microsoft.EntityFrameworkCore;

namespace Feedback_Roulette.Services
{
    public class FollowService : IFollowService
    {
        private readonly IDbContextFactory<DataContext> _contextFactory;

        public FollowService(IDbContextFactory<DataContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task FollowUserAsync(string followerUserId, string followedUserId)
        {
            if (followerUserId == followedUserId)
                return;

            using var context = await _contextFactory.CreateDbContextAsync();
            var exists = await context.UserFollows
                .AnyAsync(f => f.FollowerUserId == followerUserId && f.FollowedUserId == followedUserId);
            if (exists)
                return;

            context.UserFollows.Add(new UserFollow
            {
                FollowerUserId = followerUserId,
                FollowedUserId = followedUserId,
                CreatedAt = DateTime.UtcNow
            });
            await context.SaveChangesAsync();
        }

        public async Task UnfollowUserAsync(string followerUserId, string followedUserId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var follow = await context.UserFollows
                .FirstOrDefaultAsync(f => f.FollowerUserId == followerUserId && f.FollowedUserId == followedUserId);
            if (follow != null)
            {
                context.UserFollows.Remove(follow);
                await context.SaveChangesAsync();
            }
        }

        public async Task<bool> IsFollowingAsync(string followerUserId, string followedUserId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.UserFollows
                .AnyAsync(f => f.FollowerUserId == followerUserId && f.FollowedUserId == followedUserId);
        }

        public async Task<int> GetFollowerCountAsync(string userId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.UserFollows
                .CountAsync(f => f.FollowedUserId == userId);
        }

        public async Task<int> GetFollowingCountAsync(string userId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.UserFollows
                .CountAsync(f => f.FollowerUserId == userId);
        }

        public async Task<List<ApplicationUser>> GetFollowersAsync(string userId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.UserFollows
                .Where(f => f.FollowedUserId == userId)
                .Include(f => f.FollowerUser)
                .Select(f => f.FollowerUser)
                .ToListAsync();
        }

        public async Task<List<ApplicationUser>> GetFollowingAsync(string userId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.UserFollows
                .Where(f => f.FollowerUserId == userId)
                .Include(f => f.FollowedUser)
                .Select(f => f.FollowedUser)
                .ToListAsync();
        }
    }
}
