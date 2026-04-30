using System.Security.Claims;
using FeedbackRoulette_ClassLibrary;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Feedback_Roulette.Services;

public class IdentityService :  IIdentityService
{
        private readonly AuthenticationStateProvider _authStateProvider;
        private readonly UserManager<ApplicationUser> _userManager;
    
        public IdentityService(AuthenticationStateProvider authStateProvider,  UserManager<ApplicationUser> userManager)
        {
            _authStateProvider = authStateProvider;
            _userManager = userManager;
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
            
            ApplicationUser? user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                throw new Exception("User not found");
            }
            else
            {
                return user;
            }
        }
}