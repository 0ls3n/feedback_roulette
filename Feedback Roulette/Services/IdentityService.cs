using System.Security.Claims;
using FeedbackRoulette_ClassLibrary;
using Microsoft.AspNetCore.Identity;

namespace Feedback_Roulette.Services;

public class IdentityService :  IIdentityService
{
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<ApplicationUser> _userManager;
    
        public IdentityService(IHttpContextAccessor httpContextAccessor,  UserManager<ApplicationUser> userManager)
        {
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }
        
        public async Task<ApplicationUser> GetCurrentUserAsync()
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
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