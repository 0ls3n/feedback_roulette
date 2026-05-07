using Microsoft.VisualStudio.TestTools.UnitTesting;
using Feedback_Roulette.Services;
using Feedback_Roulette.Data;
using FeedbackRoulette_ClassLibrary;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;

namespace FeedbackRoulette_UnitTest.Services
{
    [TestClass]
    public class FeedbackItemServiceTests
    {
        private DataContext _context;
        private FeedbackItemService _service;
        private UserManager<ApplicationUser> _userManager;
        private ApplicationUser _testUser;
        private Category _testCategory;

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

            _testCategory = new Category { Id = 1, Name = "Music", Description = "Test" };
            _context.Categories.Add(_testCategory);
            _context.SaveChanges();

            var dbFactory = new TestDbContextFactory(options);
            var userStore = new UserStore<ApplicationUser>(_context);
            _userManager = new UserManager<ApplicationUser>(userStore, null, null, null, null, null, null, null, null);
            
            var environment = new TestWebHostEnvironment();
            _service = new FeedbackItemService(dbFactory, _userManager, environment);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Dispose();
        }

        [TestMethod]
        public async Task GetUserSubmissionsAsync_ShouldReturnUserSubmissions()
        {
            _context.FeedbackItems.Add(new FeedbackItem
            {
                Title = "Submission 1",
                ApplicationUserId = _testUser.Id,
                CategoryId = _testCategory.Id,
                FileUrl = "/test1.mp3",
                FileType = "audio/mpeg",
                FileSize = "1 MB",
                Description = "Test description"
            });
            _context.FeedbackItems.Add(new FeedbackItem
            {
                Title = "Submission 2",
                ApplicationUserId = _testUser.Id,
                CategoryId = _testCategory.Id,
                FileUrl = "/test2.mp3",
                FileType = "audio/mpeg",
                FileSize = "2 MB",
                Description = "Test description"
            });
            await _context.SaveChangesAsync();

            var submissions = await _service.GetUserSubmissionsAsync(_testUser.Id);

            Assert.AreEqual(2, submissions.Count);
        }

        [TestMethod]
        public async Task GetSubmissionByIdAsync_ShouldReturnCorrectSubmission()
        {
            var item = new FeedbackItem
            {
                Title = "Test Submission",
                ApplicationUserId = _testUser.Id,
                CategoryId = _testCategory.Id,
                FileUrl = "/test.mp3",
                FileType = "audio/mpeg",
                FileSize = "1 MB",
                Description = "Test description"
            };
            _context.FeedbackItems.Add(item);
            await _context.SaveChangesAsync();

            var result = await _service.GetSubmissionByIdAsync(item.Id, _testUser.Id);

            Assert.IsNotNull(result);
            Assert.AreEqual("Test Submission", result.Title);
        }

        [TestMethod]
        public async Task GetSubmissionByIdAsync_ShouldReturnNullForWrongUser()
        {
            var otherUser = new ApplicationUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "other",
                Email = "other@test.com"
            };
            _context.Users.Add(otherUser);
            
            var item = new FeedbackItem
            {
                Title = "Test Submission",
                ApplicationUserId = otherUser.Id,
                CategoryId = _testCategory.Id,
                FileUrl = "/test.mp3",
                FileType = "audio/mpeg",
                FileSize = "1 MB",
                Description = "Test description"
            };
            _context.FeedbackItems.Add(item);
            await _context.SaveChangesAsync();

            var result = await _service.GetSubmissionByIdAsync(item.Id, _testUser.Id);

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task DeleteSubmissionAsync_ShouldDeleteSubmission()
        {
            var item = new FeedbackItem
            {
                Title = "To Delete",
                ApplicationUserId = _testUser.Id,
                CategoryId = _testCategory.Id,
                FileUrl = "/test.mp3",
                FileType = "audio/mpeg",
                FileSize = "1 MB",
                Description = "Test description"
            };
            _context.FeedbackItems.Add(item);
            await _context.SaveChangesAsync();

            await _service.DeleteSubmissionAsync(item.Id, _testUser.Id);

            // Create a new context to verify deletion was saved
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            using (var newContext = new DataContext(options))
            {
                var deleted = newContext.FeedbackItems.Find(item.Id);
                Assert.IsNull(deleted);
            }
        }

        [TestMethod]
        public async Task CreateSubmissionAsync_ShouldCreateAndDeductCredits()
        {
            var initialCredits = _testUser.Credits;

            var result = await _service.CreateSubmissionAsync(
                _testUser.Id,
                "New Submission",
                _testCategory.Id,
                "Test description",
                "/uploads/test.mp3",
                "audio/mpeg",
                "1 MB"
            );

            Assert.IsNotNull(result);
            Assert.AreEqual("New Submission", result.Title);
            
            var updatedUser = await _context.Users.FindAsync(_testUser.Id);
            Assert.AreEqual(initialCredits - 50, updatedUser.Credits);
        }
    }

    internal class TestWebHostEnvironment : IWebHostEnvironment
    {
        public string EnvironmentName { get; set; } = "Development";
        public string ApplicationName { get; set; } = "TestApp";
        public string ContentRootPath { get; set; } = "";
        public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
        public string WebRootPath { get; set; } = "";
        public IFileProvider WebRootFileProvider { get; set; } = new NullFileProvider();
    }
}
