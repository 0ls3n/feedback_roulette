using System.Security.Claims;
using FeedbackRoulette_ClassLibrary;
using Feedback_Roulette.Data;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Feedback_Roulette.Services;

public class IdentityService :  IIdentityService
{
        private readonly AuthenticationStateProvider _authStateProvider;
        private readonly IDbContextFactory<DataContext> _contextFactory;
    
        public IdentityService(AuthenticationStateProvider authStateProvider, IDbContextFactory<DataContext> contextFactory)
        {
            _authStateProvider = authStateProvider;
            _contextFactory = contextFactory;
        }
        
        public async Task<ApplicationUser> GetCurrentUserAsync()
        {
            var authState = await _authStateProvider.GetAuthenticationStateAsync();
            var userPrincipal = authState.User;
            
            var userId = userPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                throw new Exception("User not found");
            }
            
            await using var context = await _contextFactory.CreateDbContextAsync();
            ApplicationUser? user = await context.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                throw new Exception("User not found");
            }
            else
            {
                return user;
            }
        }

        public async Task<ApplicationUser?> GetUserByIdAsync(string userId)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        }
}