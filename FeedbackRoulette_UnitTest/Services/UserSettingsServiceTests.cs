using Microsoft.VisualStudio.TestTools.UnitTesting;
using Feedback_Roulette.Services;
using Feedback_Roulette.Data;
using FeedbackRoulette_ClassLibrary;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FeedbackRoulette_UnitTest.Services
{
    [TestClass]
    public class UserSettingsServiceTests
    {
        private DataContext _context;
        private ApplicationUser _testUser;
        private UserManager<ApplicationUser> _userManager;
        private UserSettingsService _service;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new DataContext(options);
            
            _testUser = new ApplicationUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "testuser",
                Email = "test@test.com",
                Credits = 100
            };
            _context.Users.Add(_testUser);
            _context.SaveChanges();

            var userStore = new UserStore<ApplicationUser>(_context);
            _userManager = new UserManager<ApplicationUser>(userStore, null, null, null, null, null, null, null, null);
            
            _service = new UserSettingsService(_userManager);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Dispose();
        }

        [TestMethod]
        public async Task GetCurrentUserAsync_ShouldReturnUser()
        {
            var result = await _service.GetCurrentUserAsync(_testUser.Id);

            Assert.IsNotNull(result);
            Assert.AreEqual("testuser", result.UserName);
        }

        [TestMethod]
        public async Task GetCurrentUserAsync_ShouldReturnNullForInvalidId()
        {
            var result = await _service.GetCurrentUserAsync("invalid-id");

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task UpdateProfileImageAsync_ShouldUpdateImageUrl()
        {
            var newImageUrl = "/uploads/profile/newimage.jpg";

            await _service.UpdateProfileImageAsync(_testUser.Id, newImageUrl);

            var updatedUser = await _userManager.FindByIdAsync(_testUser.Id);
            Assert.AreEqual(newImageUrl, updatedUser.ProfileImageUrl);
        }

        [TestMethod]
        public async Task HasEnoughCreditsAsync_ShouldReturnTrueWhenEnough()
        {
            var result = await _service.HasEnoughCreditsAsync(_testUser.Id, 50);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task HasEnoughCreditsAsync_ShouldReturnFalseWhenNotEnough()
        {
            var result = await _service.HasEnoughCreditsAsync(_testUser.Id, 150);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task GetUserCreditsAsync_ShouldReturnCorrectCredits()
        {
            var credits = await _service.GetUserCreditsAsync(_testUser.Id);

            Assert.AreEqual(100, credits);
        }
    }
}
