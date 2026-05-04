using Microsoft.VisualStudio.TestTools.UnitTesting;
using Feedback_Roulette.Services;
using Feedback_Roulette.Data;
using FeedbackRoulette_ClassLibrary;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Linq;

namespace FeedbackRoulette_UnitTest.Services
{
    [TestClass]
    public class NotificationServiceTests
    {
        private DataContext _context;
        private NotificationService _service;
        private ApplicationUser _testUser;
        private string _databaseName;

        [TestInitialize]
        public void Setup()
        {
            _databaseName = Guid.NewGuid().ToString();
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: _databaseName)
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

            var dbFactory = new TestDbContextFactory(options);
            _service = new NotificationService(dbFactory);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Dispose();
        }

        [TestMethod]
        public async Task CreateNotificationAsync_ShouldCreateNotification()
        {
            await _service.CreateNotificationAsync(_testUser.Id, "Test message", "/test-link");

            var notification = _context.Notifications.FirstOrDefault();
            Assert.IsNotNull(notification);
            Assert.AreEqual("Test message", notification.Message);
            Assert.AreEqual("/test-link", notification.Link);
            Assert.AreEqual(_testUser.Id, notification.ApplicationUserId);
            Assert.IsFalse(notification.IsRead);
        }

        [TestMethod]
        public async Task GetUnreadNotificationsAsync_ShouldReturnOnlyUnread()
        {
            _context.Notifications.Add(new Notification
            {
                ApplicationUserId = _testUser.Id,
                Message = "Unread 1",
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            });
            _context.Notifications.Add(new Notification
            {
                ApplicationUserId = _testUser.Id,
                Message = "Read 1",
                IsRead = true,
                CreatedAt = DateTime.UtcNow
            });
            await _context.SaveChangesAsync();

            var unread = await _service.GetUnreadNotificationsAsync(_testUser.Id);

            Assert.AreEqual(1, unread.Count);
            Assert.AreEqual("Unread 1", unread.First().Message);
        }

        [TestMethod]
        public async Task GetAllNotificationsAsync_ShouldReturnAll()
        {
            _context.Notifications.Add(new Notification
            {
                ApplicationUserId = _testUser.Id,
                Message = "Notification 1",
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            });
            _context.Notifications.Add(new Notification
            {
                ApplicationUserId = _testUser.Id,
                Message = "Notification 2",
                IsRead = true,
                CreatedAt = DateTime.UtcNow
            });
            await _context.SaveChangesAsync();

            var all = await _service.GetAllNotificationsAsync(_testUser.Id);

            Assert.AreEqual(2, all.Count);
        }

        [TestMethod]
        public async Task GetUnreadCountAsync_ShouldReturnCorrectCount()
        {
            _context.Notifications.Add(new Notification
            {
                ApplicationUserId = _testUser.Id,
                Message = "Unread 1",
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            });
            _context.Notifications.Add(new Notification
            {
                ApplicationUserId = _testUser.Id,
                Message = "Unread 2",
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            });
            _context.Notifications.Add(new Notification
            {
                ApplicationUserId = _testUser.Id,
                Message = "Read 1",
                IsRead = true,
                CreatedAt = DateTime.UtcNow
            });
            await _context.SaveChangesAsync();

            var count = await _service.GetUnreadCountAsync(_testUser.Id);

            Assert.AreEqual(2, count);
        }

        [TestMethod]
        public async Task MarkAsReadAsync_ShouldMarkNotificationAsRead()
        {
            var notification = new Notification
            {
                ApplicationUserId = _testUser.Id,
                Message = "Test",
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            await _service.MarkAsReadAsync(notification.Id);

            // Create a new context with same database name to verify the change was saved
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: _databaseName)
                .Options;
            using (var newContext = new DataContext(options))
            {
                var updated = newContext.Notifications.Find(notification.Id);
                Assert.IsTrue(updated.IsRead);
            }
        }

        [TestMethod]
        public async Task MarkAllAsReadAsync_ShouldMarkAllAsRead()
        {
            _context.Notifications.Add(new Notification
            {
                ApplicationUserId = _testUser.Id,
                Message = "Unread 1",
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            });
            _context.Notifications.Add(new Notification
            {
                ApplicationUserId = _testUser.Id,
                Message = "Unread 2",
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            });
            await _context.SaveChangesAsync();

            await _service.MarkAllAsReadAsync(_testUser.Id);

            // Verify using same database name
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: _databaseName)
                .Options;
            using (var newContext = new DataContext(options))
            {
                var unreadCount = await newContext.Notifications.CountAsync(n => !n.IsRead);
                Assert.AreEqual(0, unreadCount);
            }
        }
    }
}
