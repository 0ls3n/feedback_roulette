using Microsoft.VisualStudio.TestTools.UnitTesting;
using Feedback_Roulette.Services;
using Feedback_Roulette.Data;
using FeedbackRoulette_ClassLibrary;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Linq;

namespace FeedbackRoulette_UnitTest.Services
{
    [TestClass]
    public class FeedbackServiceTests
    {
        private DataContext _context;
        private FeedbackService _service;
        private UserManager<ApplicationUser> _userManager;
        private ApplicationUser _testUser;
        private ApplicationUser _itemOwner;
        private FeedbackItem _testItem;
        private INotificationService _notificationService;
        private IStreakService _streakService;

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
                UserName = "reviewer",
                Email = "reviewer@test.com",
                Credits = 100
            };
            _itemOwner = new ApplicationUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "owner",
                Email = "owner@test.com",
                Credits = 100
            };
            _context.Users.AddRange(_testUser, _itemOwner);

            var category = new Category { Id = 1, Name = "Music", Description = "Test" };
            _context.Categories.Add(category);

            _testItem = new FeedbackItem
            {
                Title = "Test Item",
                ApplicationUserId = _itemOwner.Id,
                CategoryId = category.Id,
                FileUrl = "/test.mp3",
                FileType = "audio/mpeg",
                FileSize = "1 MB",
                Description = "Test description"
            };
            _context.FeedbackItems.Add(_testItem);
            _context.SaveChanges();

            var dbFactory = new TestDbContextFactory(options);
            var userStore = new UserStore<ApplicationUser>(_context);
            _userManager = new UserManager<ApplicationUser>(userStore, null, null, null, null, null, null, null, null);
            
            _notificationService = new NotificationService(dbFactory);
            _streakService = new StreakService(dbFactory);
            _service = new FeedbackService(dbFactory, _userManager, _notificationService, _streakService);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Dispose();
        }

        [TestMethod]
        public async Task SubmitFeedbackAsync_ShouldCreateFeedbackAndRewardCredits()
        {
            var initialCredits = _testUser.Credits;

            var feedback = await _service.SubmitFeedbackAsync(
                _testUser.Id,
                _testItem.Id,
                "Great work!",
                "Needs improvement",
                "Try this instead"
            );

            Assert.IsNotNull(feedback);
            Assert.AreEqual("Great work!", feedback.PositiveFeedback);
            Assert.AreEqual("Needs improvement", feedback.NegativeFeedback);
            Assert.AreEqual("Try this instead", feedback.Suggestion);
            
            var updatedUser = await _context.Users.FindAsync(_testUser.Id);
            Assert.AreEqual(initialCredits + 10, updatedUser.Credits);
        }

        [TestMethod]
        public async Task SubmitFeedbackAsync_ShouldCreateNotificationForOwner()
        {
            await _service.SubmitFeedbackAsync(
                _testUser.Id,
                _testItem.Id,
                "Great work!",
                null,
                null
            );

            var notification = _context.Notifications.FirstOrDefault(n => n.ApplicationUserId == _itemOwner.Id);
            Assert.IsNotNull(notification);
            Assert.IsTrue(notification.Message.Contains("New feedback received"));
        }

        [TestMethod]
        public async Task GetFeedbackForItemAsync_ShouldReturnItemFeedback()
        {
            var feedback = new Feedback
            {
                ApplicationUserId = _testUser.Id,
                FeedbackItemId = _testItem.Id,
                PositiveFeedback = "Good job",
                HasPositiveFeedback = true
            };
            _context.Feedbacks.Add(feedback);
            await _context.SaveChangesAsync();

            var result = await _service.GetFeedbackForItemAsync(_testItem.Id, _itemOwner.Id);

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Good job", result.First().PositiveFeedback);
        }

        [TestMethod]
        public async Task GetFeedbackCountForItemAsync_ShouldReturnCorrectCount()
        {
            _context.Feedbacks.Add(new Feedback
            {
                ApplicationUserId = _testUser.Id,
                FeedbackItemId = _testItem.Id,
                HasPositiveFeedback = true
            });
            _context.Feedbacks.Add(new Feedback
            {
                ApplicationUserId = _testUser.Id,
                FeedbackItemId = _testItem.Id,
                HasNegativeFeedback = true
            });
            await _context.SaveChangesAsync();

            var count = await _service.GetFeedbackCountForItemAsync(_testItem.Id, _itemOwner.Id);

            Assert.AreEqual(2, count);
        }
    }
}
